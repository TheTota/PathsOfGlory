using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitProjectile : MonoBehaviour
{
    private float projectileSpeed = 30f;
    private Rigidbody2D rb2d;

    public UnitType ProjectileOrigin { get; set; }
    public float shotTime;

    // Start is called before the first frame update
    void Start()
    {
        if (ProjectileOrigin == UnitType.Mages)
        {
            projectileSpeed /= 1.5f;
        }

        rb2d = GetComponent<Rigidbody2D>();
        rb2d.AddForce(-transform.right * projectileSpeed, ForceMode2D.Impulse);
        shotTime = Time.time;

        Destroy(this.gameObject, 2.5f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Time.time - shotTime >= .1f)
        {
            // ARCHER SHOOTING
            if (ProjectileOrigin == UnitType.Archers)
            {
                // ARCHER WINS/DRAW
                if (collision.transform.CompareTag("Mage") || collision.transform.CompareTag("Spearman") || collision.transform.CompareTag("Archer"))
                {
                    collision.transform.GetComponent<UnitAI>().Die();
                    Destroy(this.gameObject);
                }
                else
                {
                    // block arrow on target
                    this.rb2d.Sleep();
                    this.transform.parent = collision.transform;
                }
            }

            // MAGE SHOOTING
            else if (ProjectileOrigin == UnitType.Mages)
            {
                // MAGE WINS/DRAW
                if (collision.transform.CompareTag("Spearman") || collision.transform.CompareTag("Shield") || collision.transform.CompareTag("Mage"))
                {
                    collision.transform.GetComponent<UnitAI>().Die();
                    Destroy(this.gameObject);
                }
                else
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }
}
