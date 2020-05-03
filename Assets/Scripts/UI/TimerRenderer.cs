using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerRenderer : MonoBehaviour
{
    [SerializeField]
    private RectTransform flag;
    [SerializeField]
    private RectTransform line;
    [SerializeField]
    private RectTransform target;

    private float seconds;
    private float startPosX, targetPosX;
    private float t = 0.0f;

    public bool TimeIsUp { get; set; }

    private void Awake()
    {
        startPosX = flag.anchoredPosition.x;
        targetPosX = target.anchoredPosition.x;
    }

    /// <summary>
    /// Renders the timer in real time.
    /// </summary>
    void Update()
    {
        if (!this.TimeIsUp)
        {
            float newX = Mathf.Lerp(startPosX, targetPosX, t);
            flag.anchoredPosition = new Vector2(newX, flag.anchoredPosition.y);
            line.offsetMin = new Vector2(newX + 30f, line.offsetMin.y);

            t += Time.deltaTime / seconds;

            if (flag.anchoredPosition.x == targetPosX)
            {
                this.TimeIsUp = true;
            }
        }
    }

    /// <summary>
    /// Starts rendering the timer.
    /// </summary>
    /// <param name="time">Render time.</param>
    public void StartRenderingTimer(float time)
    {
        this.seconds = time;
        this.TimeIsUp = false;
    }

    /// <summary>
    /// Stops rendering the timer : resets positions and stuff.
    /// </summary>
    public void ResetTimer()
    {
        t = 0.0f;
        // reset flag pos
        flag.anchoredPosition = new Vector2(startPosX, flag.anchoredPosition.y);
    }

    public void SkipTimer()
    {
        this.seconds = .5f;
    }
}
