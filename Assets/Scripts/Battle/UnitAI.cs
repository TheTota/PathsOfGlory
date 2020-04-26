using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Mother class for the AI of a unit used during a units fight, multiple times during a battle.
/// </summary>
public class UnitAI : MonoBehaviour
{
    [Header("Unit death")]
    public GameObject[] limbs;
    public GameObject[] limbsDead;

    // useful infos
    public bool IsAlive { get; set; }
    protected Vector3 spawnPos;
    protected UnitsFightManager unitsFightMgr;
    protected UnitType ut;
    protected UnitAI targetEnemy;
    protected Vector3 targetPos;

    protected AudioSource audioSource;

    // units movement and hop effect (better looking than linear movement)
    protected float speed;
    protected float hopFrequency = 30f;
    protected float hopMagnitude = .01f;
    protected bool turnedBack;

    // units move speed constants
    private const float SPEARMAN_SPEED = 2f;
    private const float SHIELD_SPEED = 2f;
    private const float KNIGHT_SPEED = 5f;
    private const float MAGE_SPEED = 3f;
    private const float ARCHER_SPEED = 3f;

    public bool DemoMode { get; internal set; }

    /// <summary>
    /// Set speed with constant value depending on the underlying unit.
    /// </summary>
    private void Awake()
    {
        spawnPos = this.transform.position;
        turnedBack = false;
        audioSource = GetComponent<AudioSource>();

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
        else if (ut == UnitType.Mages)
        {
            speed = MAGE_SPEED;
        }
        else if (ut == UnitType.Archers)
        {
            speed = ARCHER_SPEED;
        }
    }

    /// <summary>
    /// Init some useful variables of the unit.
    /// </summary>
    public void Init(UnitsFightManager ufm, UnitType ut, bool isDemoMode)
    {
        this.unitsFightMgr = ufm;
        this.ut = ut;

        // demo mode for intro screen
        this.DemoMode = isDemoMode;
        if (isDemoMode)
        {
            this.IsAlive = true;
            this.speed /= 2f;
            this.hopFrequency /= 1.5f;
        }
        else
        {
            StartCoroutine(SetAliveAfterRandomTime(0f, .07f));
        }
    }

    private IEnumerator SetAliveAfterRandomTime(float minSecBound, float maxSecBound)
    {
        yield return new WaitForSeconds(Random.Range(minSecBound, maxSecBound));
        this.IsAlive = true;
    }

    public void SetTargetEnemy(UnitAI target)
    {
        this.targetEnemy = target;
    }

    /// <summary>
    /// Assigns a target to this unit.
    /// </summary>
    /// <param name="target"></param>
    public void SetTargetPos(Vector3 target)
    {
        this.targetPos = target;
    }

    /// <summary>
    /// Turns the unit around and makes it go faster!
    /// </summary>
    protected void TurnBack()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1f, transform.localScale.y, transform.localScale.z);
        turnedBack = true;
        speed *= 1.5f;
        hopFrequency *= 1.5f;
    }

    /// <summary>
    /// Moves the unit towards a target position, while apply a "hop" effect.
    /// </summary>
    protected void Move(Vector3 target)
    {
        // Move our position a step closer to the target.
        float step = speed * Time.deltaTime; // calculate distance to move
        Vector3 pos = Vector2.MoveTowards(transform.position, target, step);
        transform.position = pos + transform.up * Mathf.Sin(Time.time * hopFrequency) * hopMagnitude;
    }

    /// <summary>
    /// Kills this unit, properly.
    /// </summary>
    public void Die()
    {
        // stop moving
        this.IsAlive = false;

        // inform units fight manager
        unitsFightMgr.RemoveUnitFromList(this);

        // change sprites to dead sprites
        for (int i = 0; i < limbs.Length; i++)
        {
            limbs[i].SetActive(false);
            limbsDead[i].SetActive(true);
        }

        // disable collider
        GetComponent<BoxCollider2D>().enabled = false;

        // if not knight, take dead position
        if (ut != UnitType.Knights)
        {
            this.transform.rotation = Quaternion.Euler(0f, 0f, this.transform.localScale.x > 0 ? -90f : 90f);

            if (ut == UnitType.Spearmen)
            {
                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - .16f, this.transform.position.z);
            }
            else if (ut == UnitType.Shields)
            {
                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - .4f, this.transform.position.z);
            }
            else if (ut == UnitType.Mages)
            {
                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - 0.434f, this.transform.position.z);
            }
            else if (ut == UnitType.Archers)
            {
                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - 0.329f, this.transform.position.z);
            }
        }
    }
}
