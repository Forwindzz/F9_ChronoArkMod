using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class FadeInSprite : MonoBehaviour
{
    public SpriteRenderer render;
    public float time = 1.0f;
    public AnimationCurve curve;
    private float leftTime = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        leftTime = time;
        render.color = new Color(1, 1, 1, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        leftTime -= Time.deltaTime;
        float factor = curve.Evaluate(Mathf.Clamp01(1.0f - leftTime / time));
        render.color = new Color(1, 1, 1, factor);
        if(factor>=1)
        {
            this.enabled = false;
        }
    }
}
