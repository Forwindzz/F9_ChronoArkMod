using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Library_SpriteStudio6.CallBack;

public class SS6AnimControl : MonoBehaviour
{
    [SerializeField]
    private Script_SpriteStudio6_Root root;
    [SerializeField]
    private BlendAnimatorSpriteStudio blendHelper;

    public Script_SpriteStudio6_Root SpriteRoot { get => root; set => SetSpriteRoot(value); }

    public BlendAnimatorSpriteStudio BlendHelper { get => blendHelper; set => SetBlendHelper(value); }

    private Dictionary<string, int> animNameToIndex = new Dictionary<string, int>();

    public bool IsInited => startInited;

    private bool startInited = false;
    // Start is called before the first frame update
    void Start()
    {
        if (root == null)
        {
            root = this.GetComponent<Script_SpriteStudio6_Root>();
        }
        if (blendHelper == null)
        {
            blendHelper = this.GetComponent<BlendAnimatorSpriteStudio>();
        }
        if (!startInited)
        {
            Init();
            AddListener();
        }
    }

    public void Init(BlendAnimatorSpriteStudio newBlendHelper, Script_SpriteStudio6_Root newRoot)
    {
        RemoveListener();
        this.blendHelper = newBlendHelper;
        this.root = newRoot;
        Init();
        AddListener();
    }

    private void SetBlendHelper(BlendAnimatorSpriteStudio value)
    {
        if (value != blendHelper)
        {
            blendHelper = value;
            Init();
        }
    }

    private void SetSpriteRoot(Script_SpriteStudio6_Root value)
    {
        if (value != root)
        {
            RemoveListener();
            root = value;
            AddListener();
            Init();
        }
    }

    private void OnDestroy()
    {
        RemoveListener();
    }

    private void AddListener()
    {
        if (root != null)
        {
            root.FunctionPlayEnd += OnPlayEnd;
        }
    }

    private void RemoveListener()
    {
        if (root != null)
        {
            root.FunctionPlayEnd -= OnPlayEnd;
            if (finishBehavior != null)
            {
                root.FunctionPlayEnd -= finishBehavior;
            }
        }
    }

    private void Init()
    {
        // Init mapping 

        if (root == null || blendHelper == null)
        {
            startInited = false;
            return;
        }

        blendHelper.Init();

        animNameToIndex.Clear();
        Library_SpriteStudio6.Data.Animation[] tableAnimation = root.DataAnimation.TableAnimation;
        int length = tableAnimation.Length;
        for (int i = 0; i < length; i++)
        {
            Library_SpriteStudio6.Data.Animation animationInfo = tableAnimation[i];
            animNameToIndex[animationInfo.Name] = i;
        }

        nextConfig = new PlayConfig(INVALID, 0, true, 1.0f, null);
        curConfig.playAnimationIndex = INVALID;
        isTransitioning = false;

        startInited = true;
    }


    [SerializeField]
    private int trackNum = 0;

    //private int currentAnimation = INVALID;

    // transition state
    private bool isTransitioning = false;
    private PlayConfig nextConfig = new PlayConfig(INVALID, 0, true, 1.0f, null);
    private PlayConfig curConfig = new PlayConfig(INVALID, 0, true, 1.0f, null);
    // cache behavior
    private FunctionPlayEnd finishBehavior = null;

    public class PlayConfig
    {
        public const int INVALID_INDEX = -1;
        public const int PLAY_LOOP = 0;
        public const int PLAY_ONCE = 1;

        public int playAnimationIndex;
        public int playtimes = PLAY_LOOP;
        public bool enableTransition = true;
        public float playSpeed = 1.0f;
        public FunctionPlayEnd finishBehavior;

        public PlayConfig(int playAnimationIndex, int playtimes, bool enableTransition, float playSpeed, FunctionPlayEnd finishBehavior)
        {
            this.playAnimationIndex = playAnimationIndex;
            this.playtimes = playtimes;
            this.enableTransition = enableTransition;
            this.finishBehavior = finishBehavior;
            this.playSpeed = playSpeed;
        }
    }

    private const int INVALID = -1;


    public int GetAnimationIndex(string name)
    {
        if (animNameToIndex.TryGetValue(name, out int index))
        {
            return index;
        }
        return INVALID;
    }

    public bool SwitchToAnimation(PlayConfig config)
    {
        return SwitchToAnimation(
            config.playAnimationIndex,
            config.playtimes,
            config.enableTransition,
            config.playSpeed,
            config.finishBehavior
            );
    }

    public bool IsTransitioning => isTransitioning;

    public bool IsValidAnimationIndex(int index)
    {
        return index >= 0 && index < root.DataAnimation.TableAnimation.Length;
    }

    public bool SwitchToAnimation(
        string newAnimationName,
        int playTimes = PlayConfig.PLAY_LOOP,
        bool enableTransition = true,
        float playSpeed = 1.0f,
        FunctionPlayEnd finishBehavior = null)
    {
        return SwitchToAnimation(
            GetAnimationIndex(newAnimationName),
            playTimes,
            enableTransition,
            playSpeed,
            finishBehavior
            );
    }

    public bool SwitchToAnimation(
        int newAnimationIndex,
        int playTimes = PlayConfig.PLAY_LOOP,
        bool enableTransition = true,
        float playSpeed=1.0f,
        FunctionPlayEnd finishBehavior = null)
    {
        // For a formal animation cotroller, FSM is better, but we just want to keep it simple...
        if (
            curConfig.playAnimationIndex == newAnimationIndex ||
            IsTransitioning && nextConfig.playAnimationIndex == newAnimationIndex ||
            !IsValidAnimationIndex(newAnimationIndex)
            )
        {
            return false;
        }

        // first time, from initial state -> newAnimationIndex, or no transition
        if (curConfig.playAnimationIndex == INVALID || !enableTransition)
        {
            return DirectPlay(newAnimationIndex, playTimes, playSpeed, finishBehavior);
        }
        else
        {
            //Debug.Log($"Transition Play {currentAnimation} -> {newAnimationIndex}");
            // not first time, transition from one animation to the other
            curConfig.playAnimationIndex = blendHelper.AnimationIndex;
            isTransitioning = true;
            nextConfig.playAnimationIndex = newAnimationIndex;
            nextConfig.enableTransition = enableTransition;
            nextConfig.finishBehavior = finishBehavior;
            nextConfig.playtimes = playTimes;
            nextConfig.playSpeed = playSpeed;
            blendHelper.GenerateTransitionAnimation(newAnimationIndex);
            blendHelper.PlayTransitionAnimation();
        }
        return true;
    }

    public void SetPlaySpeed(float newPlaySpeed)
    {
        if(curConfig.playAnimationIndex==INVALID)
        {
            return;
        }
        root.RateTimeSet(trackNum, newPlaySpeed);
    }

    private bool DirectPlay(
        int newAnimationIndex,
        int playTimes = PlayConfig.PLAY_LOOP,
        float playSpeed = 1.0f,
        FunctionPlayEnd newFinishBehavior = null)
    {
        //Debug.Log($"Play {newAnimationIndex}");
        curConfig.playAnimationIndex = newAnimationIndex;
        nextConfig.playAnimationIndex = INVALID;
        isTransitioning = false;
        int frameSpeed = (int)(root.DataAnimation.TableAnimation[newAnimationIndex].FramePerSecond * playSpeed);
        bool result = root.AnimationPlay(
            trackNum,
            curConfig.playAnimationIndex, 
            playTimes,
            int.MinValue, 
            float.NaN,
            Library_SpriteStudio6.KindStylePlay.NO_CHANGE,
            null,
            int.MinValue,
            null,
            int.MaxValue,
            frameSpeed
            );
        if (result)
        {
            if(finishBehavior!=null)
            {
                root.FunctionPlayEnd -= finishBehavior;
            }
            finishBehavior = newFinishBehavior;
            if (finishBehavior != null && playTimes > 0)
            {
                root.FunctionPlayEnd += OnFinishAnimTrigger;
            }
        }
        else
        {
            Debug.Log($"Play SS6 Animation failed. Index <{newAnimationIndex}> for <{root}> ({root.StatusIsValid}) @ <{this.name}>");
        }
        return result;
    }

    private bool OnFinishAnimTrigger(Script_SpriteStudio6_Root scriptRoot, GameObject objectControl)
    {
        if(finishBehavior==null)
        {
            return true;
        }
        bool result = finishBehavior(scriptRoot, objectControl);
        root.FunctionPlayEnd -= finishBehavior; //remove the listener
        finishBehavior = null;
        return result;
    }

    private bool OnPlayEnd(Script_SpriteStudio6_Root scriptRoot, GameObject objectControl)
    {
        //Debug.Log($"Play End!");
        if (isTransitioning)
        {
            DirectPlay(nextConfig.playAnimationIndex, nextConfig.playtimes, nextConfig.playSpeed, nextConfig.finishBehavior);
        }
        return true;
    }
}
