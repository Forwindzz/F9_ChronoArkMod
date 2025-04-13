using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Library_SpriteStudio6.CallBack;
using static Library_SpriteStudio6.Script;

public class SS6AnimControl : MonoBehaviour
{
    [SerializeField]
    private Script_SpriteStudio6_Root root;
    [SerializeField]
    private BlendAnimatorSpriteStudio blendHelper;

    public Script_SpriteStudio6_Root SpriteRoot => root; 
    public BlendAnimatorSpriteStudio BlendHelper => blendHelper; 

    private Dictionary<string,int> animNameToIndex = new Dictionary<string,int>();
    // Start is called before the first frame update
    void Start()
    {
        if(root==null)
        {
            root = this.GetComponent<Script_SpriteStudio6_Root>();
        }
        if (blendHelper == null)
        {
            blendHelper = this.GetComponent<BlendAnimatorSpriteStudio>();
        }
        Init();
        AddListener();
    }

    private void OnDestroy()
    {
        RemoveListener();
    }

    private void AddListener()
    {
        root.FunctionPlayEnd += OnPlayEnd;
    }

    private void RemoveListener()
    {
        root.FunctionPlayEnd -= OnPlayEnd;
        if(finishBehavior!=null)
        {
            root.FunctionPlayEnd -= finishBehavior;
        }
    }

    private void Init()
    {
        // Init mapping 

        blendHelper.Init();


        Library_SpriteStudio6.Data.Animation[] tableAnimation = root.DataAnimation.TableAnimation;
        int length = tableAnimation.Length;
        for (int i=0;i<length;i++)
        {
            Library_SpriteStudio6.Data.Animation animationInfo = tableAnimation[i];
            animNameToIndex[animationInfo.Name] = i;
        }

        nextConfig = new PlayConfig(INVALID, 0, true, null);
        currentAnimation = INVALID;
        isTransitioning = false;

    }


    [SerializeField]
    private int trackNum = 0;

    private int currentAnimation = INVALID;

    // transition state
    private bool isTransitioning = false;
    private PlayConfig nextConfig;
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
        public FunctionPlayEnd finishBehavior;

        public PlayConfig(int playAnimationIndex, int playtimes, bool enableTransition, FunctionPlayEnd finishBehavior)
        {
            this.playAnimationIndex = playAnimationIndex;
            this.playtimes = playtimes;
            this.enableTransition = enableTransition;
            this.finishBehavior = finishBehavior;
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
            config.finishBehavior
            );
    }

    public bool IsTransitioning => nextConfig != null;

    public bool IsValidAnimationIndex(int index)
    {
        return index >= 0 && index < root.DataAnimation.TableAnimation.Length;
    }

    public bool SwitchToAnimation(
        string newAnimationName,
        int playTimes = PlayConfig.PLAY_LOOP,
        bool enableTransition = true,
        FunctionPlayEnd finishBehavior = null)
    {
        return SwitchToAnimation(
            GetAnimationIndex(newAnimationName),
            playTimes,
            enableTransition,
            finishBehavior
            );
    }

    public bool SwitchToAnimation(
        int newAnimationIndex, 
        int playTimes = PlayConfig.PLAY_LOOP, 
        bool enableTransition=true, 
        FunctionPlayEnd finishBehavior = null)
    {
        // For a formal animation cotroller, FSM is better, but we just want to keep it simple...
        if (
            currentAnimation == newAnimationIndex ||
            IsTransitioning && nextConfig.playAnimationIndex == newAnimationIndex ||
            !IsValidAnimationIndex(newAnimationIndex)
            )
        {
            return false;
        }

        // first time, from initial state -> newAnimationIndex, or no transition
        if (currentAnimation == INVALID || !enableTransition)
        {
            return DirectPlay(newAnimationIndex, playTimes);
        }
        else
        {
            //Debug.Log($"Transition Play {currentAnimation} -> {newAnimationIndex}");
            // not first time, transition from one animation to the other
            currentAnimation = blendHelper.AnimationIndex;
            isTransitioning = true;
            nextConfig.playAnimationIndex = newAnimationIndex;
            nextConfig.enableTransition = enableTransition;
            nextConfig.finishBehavior = finishBehavior;
            nextConfig.playtimes = playTimes;
            blendHelper.GenerateTransitionAnimation(newAnimationIndex);
            blendHelper.PlayTransitionAnimation();
        }
        return true;
    }

    private bool DirectPlay(
        int newAnimationIndex,
        int playTimes = PlayConfig.PLAY_LOOP)
    {
        //Debug.Log($"Play {newAnimationIndex}");
        currentAnimation = newAnimationIndex;
        nextConfig.playAnimationIndex = INVALID;
        isTransitioning = false;
        bool result= root.AnimationPlay(trackNum, currentAnimation, playTimes);
        if(result)
        {
            if (finishBehavior != null && playTimes > 0) 
            {
                root.FunctionPlayEnd += OnFinishAnimTrigger;
            }
        }
        return result;
    }

    private bool OnFinishAnimTrigger(Script_SpriteStudio6_Root scriptRoot, GameObject objectControl)
    {
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
            DirectPlay(nextConfig.playAnimationIndex, nextConfig.playtimes);
        }
        return true;
    }
}
