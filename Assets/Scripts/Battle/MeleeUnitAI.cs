using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Handles the AI of melee units during a units fight.
/// Melee units work in the following way : 
/// - Unit gets attributed a target when instantiated
/// - Unit moves towards that target (mainly on X axis)
/// - A "hop" effect is applied during movement to make it look better
/// - When the unit finally collides with its target, it applies the shifumi rules to kill or get killed by the target.
/// </summary>
public class MeleeUnitAI : UnitAI
{
    [Header("Melee SFX")]
    public AudioClip[] hitArmoredTargetClips;
    public AudioClip[] hitFleshTargetClips;

    private void Update()
    {
        // unit only does something if alive, makes sens right?
        if (base.IsAlive)
        {
            // Demo mode : walk towards a certain point
            if (DemoMode)
            {
                base.Move(base.targetPos);
                if (this.transform.position.x >= base.targetPos.x)
                {
                    Destroy(this.gameObject);
                }
            }
            // Fight mode : walk towards and enemy target
            else if (base.targetEnemy.IsAlive) // if we're alive and enemy target is alive
            {
                base.Move(base.targetEnemy.transform.position);
            }
            else if (base.unitsFightMgr.playerUnitAIs.Count == 0 || base.unitsFightMgr.enemyUnitAIs.Count == 0)
            {
                if (!base.turnedBack)
                {
                    base.TurnBack();
                }
                base.Move(base.spawnPos);
            }
        }
    }

    /// <summary>
    /// Check collision with a unit, apply the shifumi rules.
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (this.transform.parent != collision.transform.parent)
        {
            PlaySFXFromCollision(collision);
            ActFromCollisionAfterSFX(collision);
        }
    }

    private void PlaySFXFromCollision(Collider2D collision)
    {
        // sword hits armor
        if (collision.CompareTag("Knight") || collision.CompareTag("Shield") || collision.CompareTag("Spearman"))
        {
            fightAudioSource.clip = hitArmoredTargetClips[Random.Range(0, hitArmoredTargetClips.Length)];
            fightAudioSource.Play();
        }
        // sword hits flesh
        else if (collision.CompareTag("Archer") || collision.CompareTag("Mage"))
        {
            fightAudioSource.clip = hitFleshTargetClips[Random.Range(0, hitFleshTargetClips.Length)];
            fightAudioSource.Play();
        }
    }

    private void ActFromCollisionAfterSFX(Collider2D collision)
    {
        // Knight/spearman/shield WINS
        if ((this.transform.CompareTag("Knight") && (collision.transform.CompareTag("Archer") || collision.transform.CompareTag("Mage"))) // knight wins
         || (this.transform.CompareTag("Spearman") && (collision.transform.CompareTag("Knight") || collision.transform.CompareTag("Shield"))) // spearman wins
         || (this.transform.CompareTag("Shield") && (collision.transform.CompareTag("Knight") || collision.transform.CompareTag("Archer"))) // shield wins
         || this.transform.CompareTag(collision.transform.tag)) // draw
        {
            collision.gameObject.GetComponent<UnitAI>().Die();
        }
        else if (collision.transform.CompareTag("Mage") || collision.transform.CompareTag("Archer")) 
        {
            this.gameObject.GetComponent<UnitAI>().Die();
        }
    }
}
