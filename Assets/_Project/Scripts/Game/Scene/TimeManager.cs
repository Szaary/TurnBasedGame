using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

// Time management based on:
// https://www.gamedeveloper.com/design/slow-mo-tips-and-tricks
// https://stackoverflow.com/questions/32707355/how-do-you-animate-unity-time-timescale-using-an-easing-function-with-dotween

public class TimeManager : MonoBehaviour
{
    private readonly Dictionary<IUseTimescale, float> timescaleExclusions = new();

    [SerializeField] private float currentlySetTimescale = 1;
    [SerializeField] private float slowDownAnimationTime = 0.3f;
    public void AddToTimescaleExclusions(IUseTimescale user, float relativeToRealTime = 1)
    {
        var relative = Mathf.Clamp(relativeToRealTime, 0f, 1f);
        if (timescaleExclusions.ContainsKey(user)) timescaleExclusions[user] = relative;
        else timescaleExclusions.Add(user, relative);
    }

    public void Slow(float requestedTimeScale)
    {
        DOTween.To(value => Time.timeScale = value, currentlySetTimescale, requestedTimeScale, slowDownAnimationTime).OnUpdate(() =>
        {
            if (Time.timeScale < 0.001f)
                Time.timeScale = 0;
        }).SetEase(Ease.InCubic);
        currentlySetTimescale = requestedTimeScale;
    }

    public void Return()
    {
        DOTween.To(value => Time.timeScale = value, currentlySetTimescale, 1, slowDownAnimationTime).OnUpdate(() =>
        {
            if (Time.timeScale > 1.001f)
                Time.timeScale = 1;
            if (Time.timeScale < 0.999f)
                Time.timeScale = 1;
        }).SetEase(Ease.InCubic);
        currentlySetTimescale = 1;
    }


    public float GetDeltaTime(IUseTimescale user)
    {
        if (timescaleExclusions.ContainsKey(user)) return timescaleExclusions[user];
        return Time.deltaTime;
    }

    /// <summary>
    /// WARNING: Use only when necessary, subject will be not be affected by any other systems slowing time.
    /// </summary>
    /// <returns></returns>
    public float GetUnscaledDeltaTime()
    {
        return Time.unscaledDeltaTime;
    }
}

public interface IUseTimescale
{
}