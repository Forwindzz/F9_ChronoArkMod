using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShieldBarControl : MonoBehaviour
{
    public ShieldIconControl imageTemplate;
    public ShieldAnimSetting settings;
    public TextAnim textAnim;

    private float alphaMult = 1.0f;

    private List<ShieldIconControl> images = new List<ShieldIconControl>();
    private List<ShieldIconControl> destoryingImage = new List<ShieldIconControl>();
    private List<ShieldIconControl> showingImage = new List<ShieldIconControl>();

    private int currentCount = 0;
    public int CurrentCount { get => currentCount; set => SetCount(value); }

    private float existingDelayTime = 0.0f;
    private float lastTime = 0.0f;


    private void RemoveShield(ShieldIconControl info, float delay)
    {
        info.delayTime = delay+settings.baseDestoryDelay;
        if(info.IsShowing)
        {
            showingImage.Remove(info);
        }
        destoryingImage.Add(info);
        images.Remove(info);
        info.OnFinishDestoryAnimation += OnFinishDestoryAnimation;
        info.DestoryAnimation();
    }

    private void OnFinishDestoryAnimation(ShieldIconControl control)
    {
        control.StopAnimCoroutines();
        GameObject.Destroy(control.gameObject);
    }

    private void AddShield(Vector3 pos, float delay)
    {
        ShieldIconControl newShieldIcon = Instantiate(imageTemplate, transform);
        images.Add(newShieldIcon);
        showingImage.Add(newShieldIcon);
        newShieldIcon.delayTime = delay + settings.baseShowDelay;
        newShieldIcon.SetInitPos(pos);
        newShieldIcon.OnFinishShowAnimation += OnFinishShowAnimation;
        newShieldIcon.ShowAnimation();
        newShieldIcon.SetTargetAlphaImmediate(alphaMult);
    }

    private void OnFinishShowAnimation(ShieldIconControl control)
    {
        showingImage.Remove(control);
    }

    public void SetCount(int count, string text="")
    {
        count = Mathf.Max(count, 0);
        if(count==currentCount)
        {
            return;
        }
        float curTime = Time.time;
        float deltaTime = curTime - lastTime;
        float baseDelay = Mathf.Max(0.0f, existingDelayTime - deltaTime);
        float speedUpFactor = settings.speedUpFactor / (Mathf.Sqrt(baseDelay) + settings.speedUpFactor);
        bool isReducing = count < currentCount;
        if (isReducing)
        {
            int processedCount = 0;
            while(images.Count>count)
            {
                float totalDelayTime = baseDelay + processedCount * settings.destoryAnimationSeqGapTime;
                totalDelayTime *= speedUpFactor;
                speedUpFactor = settings.speedUpFactor / (Mathf.Sqrt(totalDelayTime) + settings.speedUpFactor);
                RemoveShield(images[images.Count - 1], totalDelayTime);
                processedCount++;
                existingDelayTime = totalDelayTime;
            }
        }
        else 
        {
            int processedCount = 0;
            while (images.Count < count)
            {
                float totalDelayTime = baseDelay + settings.showAnimationSeqGapTime * processedCount;
                totalDelayTime *= speedUpFactor;
                speedUpFactor = settings.speedUpFactor / (Mathf.Sqrt(totalDelayTime) + settings.speedUpFactor);
                AddShield(GetNextImagePos(), totalDelayTime);
                processedCount++;
                existingDelayTime = totalDelayTime;
            }
        }
        currentCount = count;

        float delayTime = isReducing ? settings.baseDestoryDelay : settings.baseShowDelay;
        lastTime = Time.time;

        StartCoroutine(UpdateText(text, delayTime));
    }


    private IEnumerator UpdateText(string textContent, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        if (currentCount == 0)
        {
            textAnim.text.text = textContent;
            textAnim.targetAlpha = 0;
        }
        else
        {
            textAnim.text.text = textContent;
            textAnim.targetAlpha = Mathf.Clamp01(0.2f * currentCount) * alphaMult;
        }
        textAnim.targetPos = GetNextImagePos() + settings.baseTextOffset;
        textAnim.StartAnimation();
    }

    private Vector3 GetNextImagePos()
    {
        return images.Count * settings.gapPos + settings.initPos + (images.Count % 2 == 0 ? Vector3.zero : settings.crossPosDelta);
    }

    public void SetAlphaMult(float alpha)
    {
        if(alpha==alphaMult)
        {
            return;
        }
        alphaMult = alpha;
        foreach(var image in images)
        {
            image.SetTargetAlpha(alpha);
        }
        textAnim.targetAlpha = alpha;
        textAnim.StartAnimation();
    }

    // test:
    /*
    [Range(0,20)]
    public int tempCount;
    [Range(0,1.0f)]
    public float alphaValueMult;

    // Update is called once per frame
    void Update()
    {
        SetCount(tempCount, tempCount.ToString()+"/20");
        SetAlphaMult(alphaValueMult);
    }
    
    */
}
