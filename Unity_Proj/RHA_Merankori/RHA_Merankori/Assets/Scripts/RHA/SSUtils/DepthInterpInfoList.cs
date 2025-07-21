using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DepthInterpInfo
{
    [SerializeField]
    public AnimationCurve interpolateCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField]
    public float depthPowMult = 6.0f;
    [SerializeField]
    public float depthPowAdd = 0.25f;
}

[CreateAssetMenu(fileName = "Data", menuName = "SO/DepthInterpInfoList")]
public class DepthInterpInfoList : ScriptableObject
{
    public DepthInterpInfo[] interpList = null;
}
