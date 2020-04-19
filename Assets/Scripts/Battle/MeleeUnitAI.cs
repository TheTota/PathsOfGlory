using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeUnitAI : UnitAI
{
    private void Update()
    {
        if (target != null)
        {
            // Move our position a step closer to the target.
            float step = base.speed * Time.deltaTime; // calculate distance to move
            Vector3 pos = Vector2.MoveTowards(transform.position, base.target.transform.position, step);
            transform.position = pos + transform.up * Mathf.Sin(Time.time * base.hopFrequency) * base.hopMagnitude;
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
