using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeUnitAI : UnitAI
{
    public GameObject arrowPrefab;
    public GameObject fireballPrefab;

    private Vector2 targetPos;
    private float offsetX;
    private const float MAGES_OFFSET_X = 7f;
    private const float ARCHERS_OFFSET_X = 4f;

    private bool isInShootingPosition;
    private bool goingRight;

    private void Start()
    {
        // set offset 
        if (ut == UnitType.Mages)
        {
            offsetX = MAGES_OFFSET_X;
        }
        else if (ut == UnitType.Archers)
        {
            offsetX = ARCHERS_OFFSET_X;
        }

        isInShootingPosition = false;
        SetShootingPositionTarget();
    }

    private void SetShootingPositionTarget()
    {
        Camera mainCam = Camera.main;
        float targetXPos;

        // going to the right
        if (mainCam.WorldToScreenPoint(this.transform.position).x < 0f)
        {
            targetXPos = this.transform.position.x + offsetX;
            goingRight = true;
        }
        else // going to the left
        {
            targetXPos = this.transform.position.x - offsetX;
            goingRight = false;
        }

        targetPos = new Vector2(targetXPos, this.transform.position.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInShootingPosition)
        {
            if (target != null && transform.position.x != targetPos.x)
            {
                // Move our position a step closer to the target.
                float step = base.speed * Time.deltaTime; // calculate distance to move
                Vector3 pos = Vector2.MoveTowards(transform.position, targetPos, step);
                transform.position = pos + transform.up * Mathf.Sin(Time.time * base.hopFrequency) * base.hopMagnitude;
            }
            else
            {
                StartCoroutine(ShootTarget(2f));
                isInShootingPosition = true;
            }
        }

    }

    private IEnumerator ShootTarget(float delayBetweenShots)
    {
        yield return new WaitForSeconds(.5f);
        while (this.target != null)
        {
            Shoot();
            yield return new WaitForSeconds(delayBetweenShots);
        }
    }

    private void Shoot()
    {
        GameObject proj;

        // set rotation
        Quaternion rot;
        if (goingRight)
        {
            rot = Quaternion.Euler(new Vector3(0f, 0f, 180f));
        }
        else
        {
            rot = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        }

        // instantiate projectile
        if (ut == UnitType.Archers) // is archer
        {
            proj = Instantiate(arrowPrefab, this.transform.position, rot);
        }
        else // is mage
        {
            proj = Instantiate(fireballPrefab, this.transform.position, rot);
        }

        proj.GetComponent<UnitProjectile>().ProjectileOrigin = ut;
    }
}
