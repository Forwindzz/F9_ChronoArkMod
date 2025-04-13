using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipAnimControl : MonoBehaviour
{

    // state
    private bool isYFlip = false;
    public bool IsYFlip => isYFlip;

    // animation set up
    public float animationTime = 1.0f;
    public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    // control
    private Coroutine coYFlip = null;
    private bool animatingYFlip = false;

    private void Start()
    {
        isYFlip = transform.eulerAngles.y >= 90;
    }

    public void SetYFlip(bool newYFlip)
    {
        if (newYFlip == isYFlip)
        {
            return;
        }
        StopYFlipAnimation();
        isYFlip = newYFlip;
        UpdateYFlip();
    }

    private void UpdateYFlip()
    {
        Vector3 localEulerAngles = transform.localEulerAngles;
        transform.localEulerAngles = new Vector3(localEulerAngles.x, isYFlip ? 180 : 0, localEulerAngles.z);
    }

    public void SwitchToYFlipAnimated(bool newYFlip)
    {
        if(newYFlip== isYFlip)
        {
            return;
        }
        StopYFlipAnimation();
        animatingYFlip = true;
        isYFlip = newYFlip;
        coYFlip = StartCoroutine(FlipYAnimation());
    }

    public void StopYFlipAnimation()
    {
        if (coYFlip != null && animatingYFlip)
        {
            StopCoroutine(coYFlip);
        }
    }

    private IEnumerator FlipYAnimation()
    {
        float startValue = NormalizeAngle(transform.localEulerAngles.y);
        float toValue = isYFlip ? 180 : 0;
        float totalTime = 0.0f;
        float lastTime = Time.time;
        float animationTimeDiv = 1.0f / animationTime;
        while(totalTime<animationTime)
        {
            float curTime = Time.time;
            totalTime += curTime - lastTime;
            lastTime = curTime;
            float factor = totalTime * animationTimeDiv;
            float newValue = Mathf.Lerp(startValue, toValue, factor);
            Vector3 localEulerAngles = transform.localEulerAngles;
            transform.localEulerAngles = new Vector3(localEulerAngles.x, newValue, localEulerAngles.z);
            yield return new WaitForEndOfFrame();
        }
        UpdateYFlip();
        animatingYFlip = false;
        coYFlip = null;
    }

    private static float NormalizeAngle(float angle)
    {
        float v = Mathf.Repeat(angle, 360f);
        if(v>180.0f)
        {
            return v - 360.0f;
        }
        return v;
    }
}
