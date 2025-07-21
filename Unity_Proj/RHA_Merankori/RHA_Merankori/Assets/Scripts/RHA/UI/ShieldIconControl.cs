using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShieldIconControl : MonoBehaviour
{
    public ShieldAnimSetting settings;
    public Image image;
    public Sprite destoryStateSprite;
    public float delayTime;
    private bool isShowing = false;
    private bool isDestorying = false;
    private bool isChangingAlpha = false;
    public bool IsChangingAlpha => isChangingAlpha;
    public bool IsShowing => isShowing;
    public bool IsDestorying => isDestorying;
    public float targetAlpha = 1.0f;
    private float currentAlphaMult = 1.0f;

    private float currentAlphaProgress = 0.0f;
    // -1: destory pos, 0: init Pos, 1: show pos
    private float currentPosProgress = 0.0f;
    private Vector3 initPos = Vector3.zero;

    private Coroutine coroutine;
    private Coroutine coroutineFadeAlpha;

    public event Action<ShieldIconControl> OnFinishDestoryAnimation;
    public event Action<ShieldIconControl> OnFinishShowAnimation;

    public void SetInitPos(Vector3 pos)
    {
        initPos = pos;
    }

    private void CheckCoroutine()
    {
        if (isDestorying || isShowing)
        {
            isDestorying = false;
            isShowing = false;
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }
    }

    public void ShowAnimation()
    {
        if(isShowing)
        {
            return;
        }
        CheckCoroutine();
        isShowing = true;
        coroutine = StartCoroutine(ShowLoop());
    }

    public void DestoryAnimation()
    {
        if(isDestorying)
        {
            return;
        }
        CheckCoroutine();
        isDestorying = true;
        coroutine = StartCoroutine(DestoryAnimLoop());
    }

    public void SetTargetAlphaImmediate(float alpha)
    {
        this.targetAlpha = alpha;
        this.currentAlphaMult = alpha;
    }

    public void SetTargetAlpha(float alpha)
    {
        if(this.targetAlpha==alpha)
        {
            return;
        }
        this.targetAlpha = alpha;
        this.isChangingAlpha = true;
        if(coroutineFadeAlpha!=null)
        {
            return;
        }
        coroutineFadeAlpha = StartCoroutine(FadeLoop());
    }

    private IEnumerator FadeLoop()
    {
        if (delayTime > 0)
        {
            yield return new WaitForSeconds(delayTime);
            delayTime = 0.0f;
        }
        while(currentAlphaMult != targetAlpha)
        {
            float delta = Time.deltaTime * settings.targetAlphaSpeed;
            float alphaDelta = Mathf.Abs(currentAlphaMult - targetAlpha);
            if(alphaDelta>delta)
            {
                if(currentAlphaMult > targetAlpha)
                {
                    currentAlphaMult -= delta;
                }
                else
                {
                    currentAlphaMult += delta;
                }
                SetAlpha(settings.alphaCurve.Evaluate(currentAlphaProgress));
                //Debug.Log($"{currentAlphaMult} +- {delta} -> {targetAlpha}");
            }
            else
            {
                currentAlphaMult = targetAlpha;
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        SetAlpha(settings.alphaCurve.Evaluate(currentAlphaProgress));
        coroutineFadeAlpha = null;
        this.isChangingAlpha = false;
    }

    private IEnumerator DestoryAnimLoop()
    {

        if (delayTime > 0)
        {
            yield return new WaitForSeconds(delayTime);
            delayTime = 0.0f;
        }
        image.sprite = destoryStateSprite;
        while (currentAlphaProgress > 0.0f || currentPosProgress > -1.0f)
        {
            delayTime += Time.deltaTime;
            float delta = Time.deltaTime * settings.destoryAniSpeed;
            currentAlphaProgress -= delta;
            currentPosProgress -= delta;
            currentAlphaProgress = Mathf.Max(currentAlphaProgress, 0.0f);
            currentPosProgress = Mathf.Max(currentPosProgress, -1.0f);
            float nextAlpha = settings.alphaCurve.Evaluate(currentAlphaProgress);
            SetAlpha(nextAlpha);
            TweakColor(nextAlpha);
            SetPos(settings.posCurve.Evaluate(currentPosProgress));
            yield return new WaitForEndOfFrame();
        }
        isDestorying = false;
        coroutine = null;
        OnFinishDestoryAnimation?.Invoke(this);
    }

    private IEnumerator ShowLoop()
    {
        if (delayTime > 0)
        {
            yield return new WaitForSeconds(delayTime);
            delayTime = 0.0f;
        }
        while (currentAlphaProgress < 1.0f || currentPosProgress > 0.0f)
        {
            delayTime += Time.deltaTime;
            float delta = Time.deltaTime * settings.destoryAniSpeed;
            currentAlphaProgress += delta;
            currentPosProgress -= delta;
            currentAlphaProgress = Mathf.Min(currentAlphaProgress, 1.0f);
            currentPosProgress = Mathf.Max(currentPosProgress, 0.0f);
            float nextAlpha = settings.alphaCurve.Evaluate(currentAlphaProgress);
            SetAlpha(nextAlpha);
            SetPos(settings.posCurve.Evaluate(currentPosProgress));
            yield return new WaitForEndOfFrame();
        }
        isShowing = false;
        coroutine = null;
        OnFinishShowAnimation?.Invoke(this);
    }

    public void StopAnimCoroutines()
    {
        if(coroutineFadeAlpha!=null)
        {
            StopCoroutine(coroutineFadeAlpha);
        }
        if(coroutine!=null)
        {
            StopCoroutine(coroutine);
        }
    }


    public void SetAlpha(float alpha)
    {
        Color c = image.color;
        c.a = Mathf.Clamp01(alpha * currentAlphaMult);
        image.color = c;
    }

    public void TweakColor(float gray)
    {
        image.color = new Color(0.25f+0.75f*gray, gray, gray, image.color.a);
    }

    private void SetPos(float factor)
    {
        if(factor>0.0f)
        {
            this.transform.localPosition = Vector3.Lerp(initPos, initPos+settings.showPosOffset, factor);
        }
        else
        {
            this.transform.localPosition = Vector3.Lerp(initPos, initPos+settings.destoryPosOffset, -factor);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        currentPosProgress = 1.0f;
        currentAlphaProgress = 0.0f;
        SetAlpha(0.0f);
    }
}
