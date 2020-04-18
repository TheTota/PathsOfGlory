using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeUnitAI : UnitAI
{
    private const float SPEARMAN_SPEED = 5f;
    private const float SHIELD_SPEED = 4f;
    private const float KNIGHT_SPEED = 8f;

    private float speed;

    private void Awake()
    {
        if (ut == UnitType.Knights)
        {
            speed = KNIGHT_SPEED;
        }
        else if (ut == UnitType.Spearmen)
        {
            speed = SPEARMAN_SPEED;
        }
        else if (ut == UnitType.Shields)
        {
            speed = SHIELD_SPEED;
        }
    }

    private void Update()
    {
        if (target != null)
        {
            // Move our position a step closer to the target.
            float step = speed * Time.deltaTime; // calculate distance to move
            transform.position = Vector2.MoveTowards(transform.position, base.target.transform.position, step);
        }
    }

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
