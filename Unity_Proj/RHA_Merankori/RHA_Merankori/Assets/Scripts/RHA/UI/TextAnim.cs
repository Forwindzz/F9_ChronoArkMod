using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextAnim : MonoBehaviour
{
    public Text text;
    public Vector3 targetPos;
    public float targetAlpha;

    public float moveSpeed = 1.0f;
    public float alphaSpeed = 1.0f;

    private Coroutine coroutine;

    public void StartAnimation()
    {
        if (coroutine == null)
        {
            coroutine = StartCoroutine(AnimationLoop());
        }
    }

    private IEnumerator AnimationLoop()
    {
        bool posFinish = false;
        bool alphaFinish = false;
        while(!posFinish || !alphaFinish)
        {
            yield return new WaitForEndOfFrame();
            float posDelta = Time.deltaTime * moveSpeed;
            Vector3 curPos = text.transform.localPosition;
            Vector3 delta = targetPos - curPos;
            float distance= delta.magnitude;
            posDelta *= distance;

            if (distance <= posDelta)
            {
                text.transform.localPosition = targetPos;
                posFinish = true;
            }
            else
            {
                text.transform.localPosition += delta * (posDelta / distance);
                posFinish = false;
            }

            float alphaDelta = Time.deltaTime * alphaSpeed;
            float curAlpha = text.color.a;
            float alphaDeltaAbs = Mathf.Abs(curAlpha - targetAlpha);
            alphaDelta *= alphaDeltaAbs;
            if (alphaDeltaAbs <= alphaDelta)
            {
                SetAlpha(targetAlpha);
                alphaFinish = true;
            }
            else
            {
                if (curAlpha < targetAlpha)
                {
                    SetAlpha(curAlpha + alphaDelta);
                }
                else
                {
                    SetAlpha(curAlpha - alphaDelta);
                }
                alphaFinish = false;
            }
        }
        coroutine = null;
    }

    private void SetAlpha(float alpha)
    {
        Color color = text.color;
        color.a = alpha;
        text.color = color;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetAlpha(0);
    }

}
