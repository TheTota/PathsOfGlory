using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeUnitAI : UnitAI
{
    private float speed = 5f;

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
        // Archer wins
        else if (this.transform.CompareTag("Archer") && (collision.transform.CompareTag("Mage") || collision.transform.CompareTag("Spearman")))
        {
            collision.gameObject.GetComponent<UnitAI>().Die();
        }
        // Mage wins
        else if (this.transform.CompareTag("Mage") && (collision.transform.CompareTag("Spearman") || collision.transform.CompareTag("Shield")))
        {
            collision.gameObject.GetComponent<UnitAI>().Die();
        }
    }
}
