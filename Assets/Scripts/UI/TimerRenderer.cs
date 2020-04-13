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

    private bool canRenderTimer = false;

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
        if (this.canRenderTimer)
        {
            float newX = Mathf.Lerp(startPosX, targetPosX, t);
            flag.anchoredPosition = new Vector2(newX, flag.anchoredPosition.y);
            line.offsetMin = new Vector2(newX + 30f, line.offsetMin.y);

            t += (1f / seconds) * Time.deltaTime;
        }
    }

    /// <summary>
    /// Starts rendering the timer.
    /// </summary>
    /// <param name="time">Render time.</param>
    internal void StartRenderingTimer(float time)
    {
        this.seconds = time;
        this.canRenderTimer = true;
    }

    /// <summary>
    /// Stops rendering the timer : resets positions and stuff.
    /// </summary>
    internal void StopRenderingTimer()
    {
        this.canRenderTimer = false;
        t = 0.0f;

        // reset flag pos
        flag.anchoredPosition = new Vector2(startPosX, flag.anchoredPosition.y);
    }
}
