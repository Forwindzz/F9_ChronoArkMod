using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldAnimSetting : MonoBehaviour
{

    public float destoryAniSpeed = 3.0f;
    public float destoryAnimationSeqGapTime = 0.5f;

    public float showAniSpeed = 1.0f;
    public float showAnimationSeqGapTime = 0.5f;

    public float targetAlphaSpeed = 1.0f;

    public float baseShowDelay = 2.0f;
    public float baseDestoryDelay = 2.0f;

    public float speedUpFactor = 1.0f;

    public Vector3 initPos = Vector3.zero;
    public Vector3 gapPos = Vector3.zero;
    public Vector3 crossPosDelta = Vector3.zero;

    public Vector3 baseTextOffset = Vector3.zero;


    public Vector3 showPosOffset = Vector3.up;
    public Vector3 destoryPosOffset = Vector3.down;
    public AnimationCurve alphaCurve = AnimationCurve.EaseInOut(0,0,1,1);
    public AnimationCurve posCurve = AnimationCurve.EaseInOut(-1,-1,1,1);

}
