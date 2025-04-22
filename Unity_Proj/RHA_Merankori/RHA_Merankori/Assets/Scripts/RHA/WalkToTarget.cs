using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class WalkToTarget : MonoBehaviour
{
    public const string Anim_Attack = "a";
    public const string Anim_Defenc = "d";
    public const string Anim_Escape = "e";
    public const string Anim_Run = "r";
    public const string Anim_Spare = "s";

    public SS6AnimControl animControl;
    public FlipAnimControl flipAnimControl;
    public Transform followPoint;
    public float stopRangeSqr = 1.0f;
    public float runSpeed = 0.05f;

    private void Start()
    {
        if (animControl == null)
        {
            animControl = GetComponent<SS6AnimControl>();
        }
        if (flipAnimControl == null)
        {
            flipAnimControl = GetComponent<FlipAnimControl>();
        }
    }

    private bool attack = false;

    private void FixedUpdate()
    {
        Vector3 delta = followPoint.position - this.transform.position;
        float sqrDistance = delta.sqrMagnitude;
        if (sqrDistance > stopRangeSqr)
        {
            animControl.SwitchToAnimation(Anim_Run);
            float speed = Mathf.Sqrt(Mathf.Abs(runSpeed) / 0.05f);
            animControl.SetPlaySpeed(speed);
            float distance = Mathf.Sqrt(sqrDistance);
            float runDistance = Mathf.Min(runSpeed, distance);
            this.transform.position += runDistance * delta.normalized;
            attack = false;
        }
        else
        {
            if(!attack)
            {
                animControl.SwitchToAnimation(Anim_Spare, 2, true, 1.0f, (a, b) => { animControl.SwitchToAnimation(0); attack = true; return true; });
            }
           
        }

        if (delta.x >= 0)
        {
            flipAnimControl.SwitchToYFlipAnimated(false);
        }
        else
        {
            flipAnimControl.SwitchToYFlipAnimated(true);
        }
    }
}
