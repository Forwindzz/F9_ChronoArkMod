using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using static Library_SpriteStudio6.Data.Animation;
using static Library_SpriteStudio6.Data.Animation.Attribute;
using static Library_SpriteStudio6.Data.Animation.PackAttribute;
using static Library_SpriteStudio6.Script;
using Debug = UnityEngine.Debug;

public class BlendAnimatorSpriteStudio : MonoBehaviour
{

    private class PoseInfo
    {
        public Pose[] poses; //pose information, store as an array, the tree relationship can be get through root.DataAnimation.TableParts[i].IDParent
        public int trackIndex; //other parameters
        public Library_SpriteStudio6.Data.Animation.PackAttribute.ArgumentContainer container; // other parameters
    };

    [SerializeField]
    internal Script_SpriteStudio6_Root root;
    [SerializeField]
    internal Transform bodyTransform;

    [Header("Transition Animation Info")]
    [SerializeField]
    private string animationName = "t_a";

    [SerializeField]
    private int framePerSecond = 20;
    [SerializeField]
    private int defaultAnimationIndex = 0;
    [SerializeField]
    private int generateFrameCount = 10;
    [SerializeField]
    private int trackNum = 0;

    [Header("Enhance Interp")]

    public DepthInterpInfoList depthInterpInfos;

    private bool isInited = false;

    // cache pose
    private PoseInfo defaultPose;
    private PoseInfo fromPoseCache = new PoseInfo();
    private PoseInfo toPoseCache = new PoseInfo();
    private PoseInfo[] toPoseAnimationCaches;
    // cache animation
    private Library_SpriteStudio6.Data.Animation cacheAnimation;
    private int animationIndex = -1;
    // depth cache
    private int[] treeDepthCache = null;
    private int maxDepth = -1;
    // cache arguments
    Library_SpriteStudio6.Data.Animation.PackAttribute.ArgumentContainer argumentContainer;
    private float[] weightCache;


    private struct Pose
    {
        public Vector3 localPos;
        public Vector2 localScale;
        public Vector3 localRotation;

        public void ApplyToTransform(Transform targetTransform)
        {
            targetTransform.localPosition = localPos;
            targetTransform.localScale = new Vector3(localScale.x, localScale.y, 1.0f);
            targetTransform.localRotation = Quaternion.Euler(localRotation);
        }
    }

    public void Init()
    {
        if (isInited)
        {
            return;
        }

        //Stopwatch stopwatch = new Stopwatch();
        //stopwatch.Start();

        isInited = true;
        Script_SpriteStudio6_DataAnimation dataAnimation = root.DataAnimation;
        Library_SpriteStudio6.Data.Animation[] tableAnimations = dataAnimation.TableAnimation;

        Dictionary<string, Pose> dictionary = RecordAllPoses(bodyTransform.gameObject);
        //sort out by the pos:
        defaultPose = new PoseInfo();
        Library_SpriteStudio6.Data.Parts.Animation[] tableParts = dataAnimation.TableParts;
        var poses = defaultPose.poses = new Pose[tableParts.Length];
        for (int i = 0; i < tableParts.Length; i++)
        {
            if (dictionary.ContainsKey(tableParts[i].Name))
            {
                poses[i] = dictionary[tableParts[i].Name];
                //Debug.LogWarning($"key {tableParts[i].Name} @ {i} => {poses[i].localPos}, {poses[i].localRotation}");
            }
            else
            {
                poses[i] = new Pose();
                //Debug.LogWarning($"Unknown key {tableParts[i].Name} @ {i}");
            }
        }

        //initialize the cacheAnimation data
        // the control information (which index to fetch, which frame to fetch) is assigned in advance
        // the value is copied from some animation
        animationIndex = root.DataAnimation.IndexGetAnimation(this.animationName);
        if (animationIndex >= 0)
        {
            cacheAnimation = root.DataAnimation.TableAnimation[animationIndex];
        }
        else
        {
            cacheAnimation = new Library_SpriteStudio6.Data.Animation();
            animationIndex = root.DataAnimation.TableAnimation.Length;
            root.DataAnimation.TableAnimation = root.DataAnimation.TableAnimation.Add(cacheAnimation);
        }
        cacheAnimation.FramePerSecond = framePerSecond;
        cacheAnimation.CountFrame = generateFrameCount;
        cacheAnimation.FrameValidStart = 0;
        cacheAnimation.FrameValidEnd = generateFrameCount - 1;
        cacheAnimation.DepthIK = 3;
        cacheAnimation.Name = animationName;
        cacheAnimation.CountFrameValid = generateFrameCount;
        int partLen = root.DataAnimation.TableParts.Length;
        int frameCount = generateFrameCount;
        int frameCount_m1 = generateFrameCount + 1;
        Library_SpriteStudio6.Data.Animation defaultAnimation = root.DataAnimation.TableAnimation[defaultAnimationIndex];
        Library_SpriteStudio6.Data.Parts.Animation[] tablePartsList = root.DataAnimation.TableParts;

        // record depth for parts, root is depth 0
        treeDepthCache = new int[partLen];
        for (int i = 0; i < partLen; i++)
        {
            treeDepthCache[i] = 0;
        }
        // I need memset!

        // for the first time, create a cache 
        cacheAnimation.TableParts = new Library_SpriteStudio6.Data.Animation.Parts[partLen];
        for (int i = 0; i < partLen; i++)
        {

            //deal with tree depth:
            int parentID = tablePartsList[i].IDParent;
            if(parentID>=0)
            {
                treeDepthCache[i] = treeDepthCache[parentID] + 1;
            }
           

            Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVector3 positionInfo = new Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVector3();
            positionInfo.TableValue = new Vector3[frameCount];
            positionInfo.TableCodeValue = new Library_SpriteStudio6.Data.Animation.PackAttribute.CodeValueContainer[1];
            positionInfo.TableCodeValue[0].TableCode = new int[frameCount_m1];
            positionInfo.TypePack = KindTypePack.STANDARD_CPE;
            for (int j = 0; j < frameCount_m1; j++)
            {
                // bit >>15 is index (the index of TableValue)
                // low 15 bit is frame key
                positionInfo.TableCodeValue[0].TableCode[j] = (j == frameCount) ? j : (j | (j << 15));
            }

            Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVector3 rotationInfo = new Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVector3();
            rotationInfo.TableValue = new Vector3[frameCount];
            rotationInfo.TableCodeValue = new Library_SpriteStudio6.Data.Animation.PackAttribute.CodeValueContainer[1];
            rotationInfo.TableCodeValue[0].TableCode = new int[frameCount_m1];
            rotationInfo.TypePack = KindTypePack.STANDARD_CPE;
            for (int j = 0; j < frameCount_m1; j++)
            {
                rotationInfo.TableCodeValue[0].TableCode[j] = (j == frameCount) ? j : (j | (j << 15));
            }

            Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVector2 scalingInfo = new Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVector2();
            scalingInfo.TableValue = new Vector2[frameCount];
            scalingInfo.TableCodeValue = new Library_SpriteStudio6.Data.Animation.PackAttribute.CodeValueContainer[1];
            scalingInfo.TableCodeValue[0].TableCode = new int[frameCount_m1];
            scalingInfo.TypePack = KindTypePack.STANDARD_CPE;
            for (int j = 0; j < frameCount_m1; j++)
            {
                scalingInfo.TableCodeValue[0].TableCode[j] = (j == frameCount) ? j : (j | (j << 15));
            }

            // copy other values
            ref Parts defaultParts = ref defaultAnimation.TableParts[i];

            var ScalingLocal = defaultParts.ScalingLocal;
            var Status = defaultParts.Status;
            var Cell = defaultParts.Cell;
            var RateOpacity = defaultParts.RateOpacity;
            var Priority = defaultParts.Priority;
            var PartsColor = defaultParts.PartsColor;
            var VertexCorrection = defaultParts.VertexCorrection;
            var OffsetPivot = defaultParts.OffsetPivot;
            var PositionAnchor = defaultParts.PositionAnchor;
            var SizeForce = defaultParts.SizeForce;
            var PositionTexture = defaultParts.PositionTexture;
            var RotationTexture = defaultParts.RotationTexture;
            var ScalingTexture = defaultParts.ScalingTexture;
            var RadiusCollision = defaultParts.RadiusCollision;
            var UserData = defaultParts.UserData;
            var Instance = defaultParts.Instance;
            var Effect = defaultParts.Effect;

            cacheAnimation.TableParts[i] = new Library_SpriteStudio6.Data.Animation.Parts()
            {
                Position = positionInfo,
                Rotation = rotationInfo,
                Scaling = scalingInfo,

                ScalingLocal = ScalingLocal,
                Status = Status,
                Cell = Cell,
                RateOpacity = RateOpacity,
                Priority = Priority,
                PartsColor = PartsColor,
                VertexCorrection = VertexCorrection,
                OffsetPivot = OffsetPivot,
                PositionAnchor = PositionAnchor,
                SizeForce = SizeForce,
                PositionTexture = PositionTexture,
                RotationTexture = RotationTexture,
                ScalingTexture = ScalingTexture,
                RadiusCollision = RadiusCollision,
                UserData = UserData,
                Instance = Instance,
                Effect = Effect,
                StatusParts = defaultParts.StatusParts,
            };
        }

        for (int i = 0; i < partLen; i++)
        {
            maxDepth = Mathf.Max(treeDepthCache[i], maxDepth);
        }

        fromPoseCache = new PoseInfo();
        fromPoseCache.poses = new Pose[partLen];

        if (root.StatusIsValid)
        {
            dataAnimation.BootUpInterfaceAttribute(); //force to boot again
            InitTrackPoseCache();
        }
        else
        {
            StartCoroutine(WaitForInitialization());
        }
        
        //stopwatch.Stop();
        //Debug.Log($"Gen Animation Execution Time: {stopwatch.Elapsed.Milliseconds} ms, Tick: {stopwatch.Elapsed.Ticks}");
        //Debug.Log("Cache animation complete!");
    }

    IEnumerator WaitForInitialization()
    {
        while (!root.StatusIsValid)
        {
            yield return new WaitForSeconds(0.05f);
        }
        InitTrackPoseCache();
        yield break;
    }

    //This has to be initalized after root
    private void InitTrackPoseCache()
    {
        if(toPoseAnimationCaches!=null)
        {
            return;
        }
        Script_SpriteStudio6_DataAnimation dataAnimation = root.DataAnimation;
        Library_SpriteStudio6.Data.Animation[] tableAnimations = dataAnimation.TableAnimation;
        toPoseAnimationCaches = new PoseInfo[tableAnimations.Length];
        for (int i = 0; i < tableAnimations.Length; i++)
        {
            toPoseAnimationCaches[i] = new PoseInfo();
            toPoseAnimationCaches[i] = GetTrackPose(i, trackNum, 0, toPoseAnimationCaches[i]);
        }
    }

    private static Dictionary<string, Pose> RecordAllPoses(GameObject root)
    {
        Dictionary<string, Pose> poses = new Dictionary<string, Pose>();
        TraverseRecursive(root.transform, poses);
        return poses;
    }

    private static void TraverseRecursive(Transform current, Dictionary<string, Pose> dict)
    {
        dict[current.name] = new Pose()
        {
            localPos = current.localPosition,
            localScale = current.localScale,
            localRotation = current.localRotation.eulerAngles
        };
        foreach (Transform child in current)
        {
            TraverseRecursive(child, dict);
        }
    }

    public int AnimationIndex => animationIndex;

    public void PlayTransitionAnimation()
    {
        if (!root.AnimationPlay(trackNum, animationIndex, 1)) 
        {
            Debug.LogWarning("Cannot play transition animation!");
        }
    }

    public void GenerateTransitionAnimation(int targetAnimationIndex)
    {
        //Stopwatch stopwatch = new Stopwatch();
        //stopwatch.Start();

        //Debug.Log("Gen Transition Animation!");
        fromPoseCache = GetCurrentPose(fromPoseCache);
        toPoseCache = this.toPoseAnimationCaches[targetAnimationIndex];
        //Debug.Log($"GetTrackPose (Cache) {targetAnimationIndex} F{0}");
        GenerateAnimation(fromPoseCache, toPoseCache, generateFrameCount, defaultAnimationIndex, depthInterpInfos.interpList[targetAnimationIndex]);
        //Debug.Log("Gen Finished!");

        //stopwatch.Stop();
        //Debug.Log($"Gen Animation Execution Time: {stopwatch.Elapsed.Milliseconds} ms, Tick: {stopwatch.Elapsed.Ticks}");
    }

    private void GenerateAnimation(PoseInfo fromPose, PoseInfo toPose, int frameCount, int copyAnimationIndex, DepthInterpInfo interpInfo)
    {
        int jointCount = fromPose.poses.Length;
        //List<PoseInfo> result = new List<PoseInfo>();

        //var partsTable = root.DataAnimation.TableParts;

        int partLen = root.DataAnimation.TableParts.Length;

        Library_SpriteStudio6.Data.Animation defaultAnimation = root.DataAnimation.TableAnimation[copyAnimationIndex];
        float f_div = frameCount >= 1 ? (1.0f / (frameCount - 1)) : 1;
        if (weightCache == null || weightCache.Length < frameCount) 
        {
            weightCache = new float[frameCount];
        }
        for (int i = 0; i < partLen; i++)
        {
            ref Parts parts = ref cacheAnimation.TableParts[i];
            ref Pose fromPosePart = ref fromPose.poses[i];
            ref Pose toPosePart = ref toPose.poses[i];
            var posInfo = parts.Position;
            var rotInfo = parts.Rotation;
            var scalingInfo = parts.Scaling;
            // This makes the root body first come to the target pose, and then its children.
            float depthPower =
                    ((treeDepthCache[i] / maxDepth) * interpInfo.depthPowMult + interpInfo.depthPowAdd);
            for (int f = 0; f < frameCount; f++)
            {
                weightCache[f] = Mathf.Pow(interpInfo.interpolateCurve.Evaluate(f_div * f), depthPower);
            }

            Vector3 fromPos = fromPosePart.localPos;
            Vector3 toPos = toPosePart.localPos;
            for (int f = 0; f < frameCount; f++)
            {
                posInfo.TableValue[f] = Vector3.LerpUnclamped(
                    fromPos, 
                    toPos,
                    weightCache[f]);
            }

            Quaternion fromRot = Quaternion.Euler(fromPosePart.localRotation);
            Quaternion toRot = Quaternion.Euler(toPosePart.localRotation);
            for (int f = 0; f < frameCount; f++)
            {
                rotInfo.TableValue[f] = Quaternion.LerpUnclamped(
                    fromRot,
                    toRot,
                    weightCache[f])
                    .eulerAngles;
            }

            Vector3 fromScale = fromPosePart.localScale;
            Vector3 toScale = toPosePart.localScale;
            for (int f = 0; f < frameCount; f++)
            {
                scalingInfo.TableValue[f] = Vector2.LerpUnclamped(
                    fromScale,
                    toScale,
                    weightCache[f]);
            }
            //should we copy or interpolated?
            /*
            ref Parts defaultParts = ref defaultAnimation.TableParts[i];
            parts.Scaling = defaultParts.Scaling;
            parts.Status = defaultParts.Status;
            parts.Cell = defaultParts.Cell;
            parts.RateOpacity = defaultParts.RateOpacity;
            parts.Priority = defaultParts.Priority;
            parts.PartsColor = defaultParts.PartsColor;
            parts.VertexCorrection = defaultParts.VertexCorrection;
            parts.OffsetPivot = defaultParts.OffsetPivot;
            parts.PositionAnchor = defaultParts.PositionAnchor;
            parts.SizeForce = defaultParts.SizeForce;
            parts.PositionTexture = defaultParts.PositionTexture;
            parts.RotationTexture = defaultParts.RotationTexture;
            parts.ScalingTexture = defaultParts.ScalingTexture;
            parts.RadiusCollision = defaultParts.RadiusCollision;
            parts.UserData = defaultParts.UserData;
            parts.Instance = defaultParts.Instance;
            parts.Effect = defaultParts.Effect;
            */
        }


        

    }

    private PoseInfo GetCurrentPose(PoseInfo outPosInfo)
    {
        var tableControlTracks = root.TableControlTrack;
        if(tableControlTracks.Length==0)
        {
            return null;
        }
        var curContainer = tableControlTracks[trackNum].ArgumentContainer;
        return GetTrackPose(curContainer.IndexAnimation, trackNum, curContainer.Frame, outPosInfo);
    }

    private PoseInfo GetTrackPose(int animationIndex, int trackIndex, int frameIndex, PoseInfo outPosInfo)
    {
        //Debug.Log($"GetTrackPose {animationIndex} F{frameIndex}");
        Library_SpriteStudio6.Data.Animation animation = root.DataAnimation.TableAnimation[animationIndex];
        var tableControlTracks = root.TableControlTrack;
        // make them different, so that we can get the value, but not skip the value
        argumentContainer.Frame = frameIndex;
        argumentContainer.IndexAnimation = animationIndex;
        argumentContainer.FramePrevious = frameIndex - 1;
        int initFrameKey = -1;// frameIndex == 0 ? 1 : 0;
        int outFrameKey = initFrameKey;

        Vector3 outPos = Vector3.zero;
        Vector3 outRot = Vector3.zero;
        Vector2 outScale = Vector2.zero;

        int partCount = animation.TableParts.Length;
        outPosInfo.trackIndex = trackIndex;
        outPosInfo.container = argumentContainer;
        if(outPosInfo.poses==null || outPosInfo.poses.Length!= partCount)
        {
            outPosInfo.poses = new Pose[partCount];
        }
        Pose[] poses = outPosInfo.poses;
        Pose[] defaultPoseInfo = defaultPose.poses;

        for (int i = 0; i < partCount; i++)
        {
            argumentContainer.IDParts = i;
            Library_SpriteStudio6.Data.Animation.Parts tablePart = animation.TableParts[i];

            outFrameKey = initFrameKey;
            Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVector3 positionInfo = tablePart.Position;

            if(positionInfo == null ||
                positionInfo.Function == null || 
                !positionInfo.Function.ValueGet(ref outPos, ref outFrameKey, positionInfo, ref argumentContainer))
            {
                outPos = defaultPoseInfo[i].localPos;
            }

            outFrameKey = initFrameKey;
            Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVector3 rotationInfo = tablePart.Rotation;
            if (rotationInfo == null ||
                rotationInfo.Function == null||
                !rotationInfo.Function.ValueGet(ref outRot, ref outFrameKey, rotationInfo, ref argumentContainer))
            {
                outRot = defaultPoseInfo[i].localRotation;
            }

            outFrameKey = initFrameKey;
            Library_SpriteStudio6.Data.Animation.PackAttribute.ContainerVector2 scalingInfo = tablePart.Scaling;
            if (scalingInfo == null ||
                scalingInfo.Function == null||
                !scalingInfo.Function.ValueGet(ref outScale, ref outFrameKey, scalingInfo, ref argumentContainer))
            {
                outScale = defaultPoseInfo[i].localScale;
            }

            poses[i] = new Pose()
            {
                localPos = outPos,
                localScale = outScale,
                localRotation = outRot
            };
        }

        return outPosInfo;
    }
}

