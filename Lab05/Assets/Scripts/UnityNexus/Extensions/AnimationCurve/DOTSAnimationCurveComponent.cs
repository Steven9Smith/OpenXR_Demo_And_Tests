using Unity.Mathematics;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using System;
using Unity.Burst;

namespace UnityNexus.Extensions
{
    public class DOTSAnimationCurveComponent
    {
        public AnimationCurve animationCurve;
        public DOTSAnimationCurve ToDOTSAnimationCurve(Allocator builderAllocator,Allocator blobAllocator)
        {
            return DOTSAnimationCurveAuthoring.CreateDOTSAnimationCurve(animationCurve, builderAllocator, blobAllocator);
        }
    }
    public class DOTSAnimationCurveAuthoring
    {
        public static DOTSAnimationCurve CreateDOTSAnimationCurve(AnimationCurve curve,Allocator builderAllocator,Allocator blobAllocator)
        {
            using var blobBuilder = new BlobBuilder(builderAllocator);
            ref var DACBlobArray = ref blobBuilder.ConstructRoot<DOTSAnimationCurveBlobArray>();
            var valuesArr = blobBuilder.Allocate(ref DACBlobArray.values, curve.keys.Length);
            var keysArr = blobBuilder.Allocate(ref DACBlobArray.keys, curve.keys.Length);

            for(int i = 0; i < curve.keys.Length; i++)
            {
                var point = (float)i / (curve.keys.Length - 1);
                var value = curve.Evaluate(point);
                valuesArr[i] = value;
                keysArr[i] = new DOTSKeyframe(curve.keys[i]);
            }

            var blobAssetReference = blobBuilder.CreateBlobAssetReference<DOTSAnimationCurveBlobArray>(blobAllocator);

            return new DOTSAnimationCurve
            {
                value = blobAssetReference
            };
        }
        public static DOTSAnimationCurve CreateDOTSAnimationCurve(Keyframe[] keys, Allocator builderAllocator, Allocator blobAllocator)
        {
            AnimationCurve c = new AnimationCurve(keys);
            return CreateDOTSAnimationCurve(c,builderAllocator,blobAllocator);
        }
        [BurstCompile]
        public static void Set(ref DOTSAnimationCurve curve, DOTSAnimationCurve otherCurve)
        {
            curve.Dispose();
            curve.value = otherCurve.value;
        }
        public static void Set(ref DOTSAnimationCurve curve, AnimationCurve otherCurve)
        {
            curve.Dispose();
            curve = CreateDOTSAnimationCurve(otherCurve,Allocator.Temp,Allocator.Persistent);
        }
        public static void Set(ref DOTSAnimationCurve curve, Keyframe[] keys)
        {
            curve.Dispose();
            curve = CreateDOTSAnimationCurve(keys, Allocator.Temp, Allocator.Persistent);
        }
    }
    public struct DOTSAnimationCurve : IComponentData
    {
        public BlobAssetReference<DOTSAnimationCurveBlobArray> value;
        public readonly float Evaluate(float time) => value.Value.Evaluate(time);

        public void Dispose()
        {
            if (value.IsCreated)
                value.Dispose();
        }
    }

    public struct DOTSAnimationCurveBlobArray
    {
        public BlobArray<DOTSKeyframe> keys;
        public BlobArray<float> values;

        public float Evaluate(float time)
        {
            var approxSampleIndex = (values.Length - 1) * time;
            var sampleIndexBelow = (int)math.floor(approxSampleIndex);
            if (sampleIndexBelow >= values.Length - 1)
                return values[values.Length - 1];
            var indexRemainder = approxSampleIndex - sampleIndexBelow;
            return math.lerp(values[sampleIndexBelow], values[sampleIndexBelow + 1], indexRemainder);
        }
    }
    [System.Serializable]
    // A single keyframe that can be injected into an animation curve.
    public struct DOTSKeyframe : IComponentData
    {
        float m_Time;
        float m_Value;
        float m_InTangent;
        float m_OutTangent;

        int m_TangentMode;

        int m_WeightedMode;

        float m_InWeight;
        float m_OutWeight;

        // Create a DOTSKeyframe.
        public DOTSKeyframe(float time, float value)
        {
            m_Time = time;
            m_Value = value;
            m_InTangent = 0;
            m_OutTangent = 0;
            m_WeightedMode = 0;
            m_InWeight = 0f;
            m_OutWeight = 0f;
            m_TangentMode = 0;
        }

        // Create a DOTSKeyframe.
        public DOTSKeyframe(float time, float value, float inTangent, float outTangent)
        {
            m_Time = time;
            m_Value = value;
            m_InTangent = inTangent;
            m_OutTangent = outTangent;
            m_WeightedMode = 0;
            m_InWeight = 0f;
            m_OutWeight = 0f;
            m_TangentMode = 0;
        }

        // Create a DOTSKeyframe.
        public DOTSKeyframe(float time, float value, float inTangent, float outTangent, float inWeight, float outWeight)
        {
            m_Time = time;
            m_Value = value;
            m_InTangent = inTangent;
            m_OutTangent = outTangent;
            m_WeightedMode = (int)WeightedMode.Both;
            m_InWeight = inWeight;
            m_OutWeight = outWeight;
            m_TangentMode = 0;
        }
        public DOTSKeyframe(Keyframe keyframe)
        {
            m_Time = keyframe.time;
            m_Value = keyframe.value;
            m_InTangent = keyframe.inTangent;
            m_OutTangent = keyframe.outTangent;
            m_WeightedMode = (int)keyframe.weightedMode;
            m_InWeight = keyframe.inWeight;
            m_OutWeight = keyframe.outWeight;
#pragma warning disable CS0618 // Type or member is obsolete
            m_TangentMode = keyframe.tangentMode;
#pragma warning restore CS0618 // Type or member is obsolete
        }

        // The time of the DOTSKeyframe.
        public float time { get { return m_Time; } set { m_Time = value; } }

        // The value of the curve at DOTSKeyframe.
        public float value { get { return m_Value; } set { m_Value = value; } }

        // Describes the tangent when approaching this point from the previous point in the curve.
        public float inTangent { get { return m_InTangent; } set { m_InTangent = value; } }

        // Describes the tangent when leaving this point towards the next point in the curve.
        public float outTangent { get { return m_OutTangent; } set { m_OutTangent = value; } }

        // Describes the weight when approaching this point from the previous point in the curve.
        public float inWeight { get { return m_InWeight; } set { m_InWeight = value; } }

        // Describes the weight when leaving this point towards the next point in the curve.
        public float outWeight { get { return m_OutWeight; } set { m_OutWeight = value; } }

        public WeightedMode weightedMode { get { return (WeightedMode)m_WeightedMode; } set { m_WeightedMode = (int)value; } }

        internal int tangentModeInternal
        {
            get
            {
                return m_TangentMode;
            }
            set
            {
                m_TangentMode = value;
            }
        }

        public Keyframe toKeyframe()
        {
            return new Keyframe()
            {
                inTangent = m_InTangent,
                inWeight = m_InWeight,
                outTangent = m_OutTangent,
                outWeight = m_OutWeight,
#pragma warning disable CS0618 // Type or member is obsolete
                tangentMode = m_TangentMode,
#pragma warning restore CS0618 // Type or member is obsolete
                time = m_Time,
                value = m_Value,
                weightedMode = (WeightedMode)m_WeightedMode
            };
        }

    }


}