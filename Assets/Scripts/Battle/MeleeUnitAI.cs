using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private void Update()
    {
        // Demo mode : walk towards a certain point
        if (DemoMode)
        {
            Move(base.targetPos);
            if (this.transform.position.x >= base.targetPos.x)
            {
                Destroy(this.gameObject);
            }
        }
        // Fight mode : walk towards and enemy target
        else if (base.targetEnemy != null)
        {
            Move(base.targetEnemy.transform.position);
        }
    }



    /// <summary>
    /// Moves the unit towards its target, while applying a hop effect.
    /// </summary>
    private void Move(Vector3 target)
    {
        // Move our position a step closer to the target.
        float step = base.speed * Time.deltaTime; // calculate distance to move
        Vector3 pos = Vector2.MoveTowards(transform.position, target, step);
        transform.position = pos + transform.up * Mathf.Sin(Time.time * base.hopFrequency) * base.hopMagnitude;
    }

    /// <summary>
    /// Check collision with a unit, apply the shifumi rules.
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Knight wins
        if (this.transform.CompareTag("Knight") && (collision.transform.CompareTag("Archer") || collision.transform.CompareTag("Mage")))
        {
            collision.gameObject.GetComponent<UnitAI>().Die();
        }
        // Spearman wins
        else if (this.transform.CompareTag("Spearman") && (collision.transform.CompareTag("Knight") || collision.transform.CompareTag("Shield")))
        {
            collision.gameObject.GetComponent<UnitAI>().Die();
        }
        // Shield wins
        else if (this.transform.CompareTag("Shield") && (collision.transform.CompareTag("Knight") || collision.transform.CompareTag("Archer")))
        {
            collision.gameObject.GetComponent<UnitAI>().Die();
        }
        // DRAW
        else if (this.transform.CompareTag(collision.transform.tag) && this.transform.parent != collision.transform.parent)
        {
            collision.gameObject.GetComponent<UnitAI>().Die();
            base.Die();
        }
    }
}
