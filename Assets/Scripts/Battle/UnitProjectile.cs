using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script associated to a projectile sent by a range unit during a units fight.
/// It handles the movement of the projectile, as well as its collision.
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class UnitProjectile : MonoBehaviour
{
    [SerializeField]
    private AudioSource targetEffectiveHitSound;
    [SerializeField]
    private AudioSource targetIneffectiveHitSound;

    // physics vars for projectile movement
    private float projectileSpeed = 30f;
    private Rigidbody2D rb2d;
    private BoxCollider2D bc2d;

    // useful vars for projectile behaviours
    public UnitType ProjectileOrigin { get; set; }
    private float shotTime;

    // Start is called before the first frame update
    void Start()
    {
        bc2d = GetComponent<BoxCollider2D>();

        // make the mage's fireballs slower than the arrows
        if (ProjectileOrigin == UnitType.Mages)
        {
            projectileSpeed /= 1.5f;
        }

        // shoot the projectile
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.AddForce(-transform.right * projectileSpeed, ForceMode2D.Impulse);
        shotTime = Time.time;

        // auto destroy it in a bit 
        Destroy(this.gameObject, 3f);
    }

    /// <summary>
    /// Handles the collision of the projectile and the actions to take depending on who it comes from
    /// and who got touched by it.
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Wait for the projectile to be out of its own unit's collider
        if (Time.time - shotTime >= .1f)
        {
            this.bc2d.enabled = false;

            // ARCHER SHOOTING
            if (ProjectileOrigin == UnitType.Archers)
            {
                // block arrow on target
                this.rb2d.Sleep();
                this.transform.parent = collision.transform;

                // ARCHER WINS/DRAW
                if (collision.transform.CompareTag("Mage") || collision.transform.CompareTag("Spearman") || collision.transform.CompareTag("Archer"))
                {
                    // play gg hit sound
                    targetEffectiveHitSound.Play();

                    collision.transform.GetComponent<UnitAI>().Die();
                }
                else // ARCHER NOT MEANT TO WIN
                {
                    // play fail hit sound
                    targetIneffectiveHitSound.Play();
                }
            }

            // MAGE SHOOTING
            else if (ProjectileOrigin == UnitType.Mages)
            {
                // MAGE WINS/DRAW
                if (!collision.transform.CompareTag("Knight"))
                {
                    // play gg hit sound
                    targetEffectiveHitSound.Play();

                    collision.transform.GetComponent<UnitAI>().Die();
                }
                else // MAGE NOT MEANT TO WIN
                {
                    // play fail hit sound
                    targetIneffectiveHitSound.Play();
                }

                transform.Find("GFX").GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }
}
