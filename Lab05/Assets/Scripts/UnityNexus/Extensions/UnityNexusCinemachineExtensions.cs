using System;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;
using Cinemachine;
using Cinemachine.Utility;

namespace UnityNexus.Extensions.UN_Cinemachine
{
     /// <summary>
    /// Axis state for defining how to react to player input.
    /// The settings here control the responsiveness of the axis to player input.
    /// </summary>
    [Serializable]
    public struct DOTSAxisState : IComponentData
    {
        /// <summary>The current value of the axis</summary>
        [NoSaveDuringPlay]
        [Tooltip("The current value of the axis.")]
        public float Value;

        /// <summary>How to interpret the Max Speed setting.</summary>
        [Tooltip("How to interpret the Max Speed setting: in units/second, or as a "
            + "direct input value multiplier")]
        public Cinemachine.AxisState.SpeedMode m_SpeedMode;

        /// <summary>How fast the axis value can travel.  Increasing this number
        /// makes the behaviour more responsive to joystick input</summary>
        [Tooltip("The maximum speed of this axis in units/second, or the input value "
            + "multiplier, depending on the Speed Mode")]
        public float m_MaxSpeed;

        /// <summary>The amount of time in seconds it takes to accelerate to
        /// MaxSpeed with the supplied Axis at its maximum value</summary>
        [Tooltip("The amount of time in seconds it takes to accelerate to MaxSpeed "
            + "with the supplied Axis at its maximum value")]
        public float m_AccelTime;

        /// <summary>The amount of time in seconds it takes to decelerate
        /// the axis to zero if the supplied axis is in a neutral position</summary>
        [Tooltip("The amount of time in seconds it takes to decelerate the axis to "
            + "zero if the supplied axis is in a neutral position")]
        public float m_DecelTime;

        /// <summary>The name of this axis as specified in Unity Input manager.
        /// Setting to an empty string will disable the automatic updating of this axis</summary>
        [FormerlySerializedAs("m_AxisName")]
        [Tooltip("The name of this axis as specified in Unity Input manager. "
            + "Setting to an empty string will disable the automatic updating of this axis")]
        public FixedString32Bytes m_InputAxisName;

        /// <summary>The value of the input axis.  A value of 0 means no input
        /// You can drive this directly from a
        /// custom input system, or you can set the Axis Name and have the value
        /// driven by the internal Input Manager</summary>
        [NoSaveDuringPlay]
        [Tooltip("The value of the input axis.  A value of 0 means no input.  "
            + "You can drive this directly from a custom input system, or you can set "
            + "the Axis Name and have the value driven by the internal Input Manager")]
        public float m_InputAxisValue;

        /// <summary>If checked, then the raw value of the input axis will be inverted
        /// before it is used.</summary>
        [FormerlySerializedAs("m_InvertAxis")]
        [Tooltip("If checked, then the raw value of the input axis will be inverted "
            + "before it is used")]
        public bool m_InvertInput;

        /// <summary>The minimum value for the axis</summary>
        [Tooltip("The minimum value for the axis")]
        public float m_MinValue;

        /// <summary>The maximum value for the axis</summary>
        [Tooltip("The maximum value for the axis")]
        public float m_MaxValue;

        /// <summary>If checked, then the axis will wrap around at the 
        /// min/max values, forming a loop</summary>
        [Tooltip("If checked, then the axis will wrap around at the min/max values, "
            + "forming a loop")]
        public bool m_Wrap;

        private float m_CurrentSpeed;
      
        public bool ValueRangeLocked;

        /// <summary>Constructor with specific values</summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <param name="wrap"></param>
        /// <param name="rangeLocked"></param>
        /// <param name="maxSpeed"></param>
        /// <param name="accelTime"></param>
        /// <param name="decelTime"></param>
        /// <param name="name"></param>
        /// <param name="invert"></param>
        public DOTSAxisState(
            float minValue, float maxValue, bool wrap, bool rangeLocked,
            float maxSpeed, float accelTime, float decelTime,
            string name, bool invert)
        {
            m_MinValue = minValue;
            m_MaxValue = maxValue;
            m_Wrap = wrap;
            ValueRangeLocked = rangeLocked;

            m_SpeedMode = AxisState.SpeedMode.MaxSpeed;
            m_MaxSpeed = maxSpeed;
            m_AccelTime = accelTime;
            m_DecelTime = decelTime;
            Value = (minValue + maxValue) / 2;
            m_InputAxisName = name;
            m_InputAxisValue = 0;
            m_InvertInput = invert;

            m_CurrentSpeed = 0f;
        }
        public DOTSAxisState(Cinemachine.AxisState a)
        {

            m_MinValue = a.m_MinValue;
            m_MaxValue = a.m_MaxValue;
            m_Wrap = a.m_Wrap;
            ValueRangeLocked = a.ValueRangeLocked;
            m_SpeedMode = a.m_SpeedMode;
            m_MaxSpeed = a.m_MaxSpeed;
            m_AccelTime = a.m_AccelTime;
            m_DecelTime = a.m_DecelTime;
            Value = (m_MinValue + m_MaxValue) / 2;
            m_InputAxisName = new FixedString32Bytes(a.m_InputAxisName);
            m_InputAxisValue = 0;
            m_InvertInput = a.m_InvertInput;
            m_CurrentSpeed = 0f;
        }


        /// <summary>Call from OnValidate: Make sure the fields are sensible</summary>
        public void Validate()
        {
            if (m_SpeedMode ==  AxisState.SpeedMode.MaxSpeed)
                m_MaxSpeed = math.max(0, m_MaxSpeed);
            m_AccelTime = math.max(0, m_AccelTime);
            m_DecelTime = math.max(0, m_DecelTime);
            m_MaxValue = math.clamp(m_MaxValue, m_MinValue, m_MaxValue);
        }

        const float Epsilon = UnityVectorExtensions.Epsilon;

        /// <summary>
        /// Cancel current input state and reset input to 0
        /// </summary>
        public void Reset()
        {
            m_InputAxisValue = 0;
            m_CurrentSpeed = 0;
        }

        /// <summary>
        /// This is an interface to override default querying of Unity's legacy Input system.
        /// If a befaviour implementing this interface is attached to a Cinemachine virtual camera that 
        /// requires input, that interface will be polled for input instead of the standard Input system.
        /// </summary>
     
        public Cinemachine.AxisState ToCinemachineAxisState()
        {
            return new Cinemachine.AxisState(m_MinValue,m_MaxValue,m_Wrap,ValueRangeLocked,m_MaxSpeed,m_AccelTime,m_DecelTime,m_InputAxisName.ToString(),m_InvertInput);
        }
        public float ClampValue(float v)
        {
            float r = m_MaxValue - m_MinValue;
            if (m_Wrap && r > Epsilon)
            {
                v = (v - m_MinValue) % r;
                v += m_MinValue + ((v < 0) ? r : 0);
            }
            return Mathf.Clamp(v, m_MinValue, m_MaxValue);
        }
        /// <summary>Helper for automatic axis recentering</summary>
        [DocumentationSorting(DocumentationSortingAttribute.Level.UserRef)]
        [Serializable]
        public struct DOTSRecentering
        {
            /// <summary>If checked, will enable automatic recentering of the
            /// axis. If FALSE, recenting is disabled.</summary>
            [Tooltip("If checked, will enable automatic recentering of the axis. If unchecked, recenting is disabled.")]
            public bool m_enabled;

            /// <summary>If no input has been detected, the camera will wait
            /// this long in seconds before moving its heading to the default heading.</summary>
            [Tooltip("If no user input has been detected on the axis, the axis will wait this long in seconds before recentering.")]
            public float m_WaitTime;

            /// <summary>How long it takes to reach destination once recentering has started</summary>
            [Tooltip("How long it takes to reach destination once recentering has started.")]
            public float m_RecenteringTime;

            /// <summary>Constructor with specific field values</summary>
            /// <param name="enabled"></param>
            /// <param name="waitTime"></param>
            /// <param name="recenteringTime"></param>
            public DOTSRecentering(bool enabled, float waitTime,  float recenteringTime)
            {
                m_enabled = enabled;
                m_WaitTime = waitTime;
                m_RecenteringTime = recenteringTime;
                mLastAxisInputTime = 0;
                mRecenteringVelocity = 0;
                m_LegacyHeadingDefinition = m_LegacyVelocityFilterStrength = -1;
            }
            public DOTSRecentering(Cinemachine.AxisState.Recentering r)
            {
                this.m_enabled = r.m_enabled;
                this.m_WaitTime = r.m_WaitTime;
                this.m_RecenteringTime = r.m_RecenteringTime;
                mLastAxisInputTime = 0;
                mRecenteringVelocity = 0;
                m_LegacyHeadingDefinition = m_LegacyVelocityFilterStrength = -1;
            }


            /// <summary>Call this from OnValidate()</summary>
            public void Validate()
            {
                m_WaitTime = Mathf.Max(0, m_WaitTime);
                m_RecenteringTime = Mathf.Max(0, m_RecenteringTime);
            }

            // Internal state
            float mLastAxisInputTime;
            float mRecenteringVelocity;

            /// <summary>
            /// Copy Recentering state from another Recentering component.
            /// </summary>
            /// <param name="other"></param>
            public void CopyStateFrom(ref DOTSRecentering other)
            {
                if (mLastAxisInputTime != other.mLastAxisInputTime)
                    other.mRecenteringVelocity = 0;
                mLastAxisInputTime = other.mLastAxisInputTime;
            }

            /// <summary>Cancel any recenetering in progress.</summary>
            public void CancelRecentering()
            {
                mLastAxisInputTime = CinemachineCore.CurrentTime;
                mRecenteringVelocity = 0;
            }

            /// <summary>Skip the wait time and start recentering now (only if enabled).</summary>
            public void RecenterNow()
            {
                mLastAxisInputTime = 0;
            }
           
            /// <summary>Bring the axis back to the centered state (only if enabled).</summary>
            /// <param name="axis">The axis to recenter</param>
            /// <param name="deltaTime">Current effective deltaTime</param>
            /// <param name="recenterTarget">The value that is considered to be centered</param>
            public void DoRecentering(ref DOTSAxisState axis, float deltaTime, float recenterTarget)
            {
                if (!m_enabled && deltaTime >= 0)
                    return;

                recenterTarget = axis.ClampValue(recenterTarget);
                if (deltaTime < 0)
                {
                    CancelRecentering();
                    if (m_enabled)
                        axis.Value = recenterTarget;
                    return;
                }

                float v = axis.ClampValue(axis.Value);
                float delta = recenterTarget - v;
                if (delta == 0)
                    return;

                if (CinemachineCore.CurrentTime < (mLastAxisInputTime + m_WaitTime))
                    return;

                // Determine the direction
                float r = axis.m_MaxValue - axis.m_MinValue;
                if (axis.m_Wrap && Mathf.Abs(delta) > r * 0.5f)
                    v += Mathf.Sign(recenterTarget - v) * r;

                // Damp our way there
                if (m_RecenteringTime < 0.001f)
                    v = recenterTarget;
                else
                    v = Mathf.SmoothDamp(
                        v, recenterTarget, ref mRecenteringVelocity,
                        m_RecenteringTime, 9999, deltaTime);
                axis.Value = axis.ClampValue(v);
            }

            // Legacy support
            [SerializeField] [HideInInspector] [FormerlySerializedAs("m_HeadingDefinition")] private int m_LegacyHeadingDefinition;
            [SerializeField] [HideInInspector] [FormerlySerializedAs("m_VelocityFilterStrength")] private int m_LegacyVelocityFilterStrength;
            internal bool LegacyUpgrade(ref int heading, ref int velocityFilter)
            {
                if (m_LegacyHeadingDefinition != -1 && m_LegacyVelocityFilterStrength != -1)
                {
                    heading = m_LegacyHeadingDefinition;
                    velocityFilter = m_LegacyVelocityFilterStrength;
                    m_LegacyHeadingDefinition = m_LegacyVelocityFilterStrength = -1;
                    return true;
                }
                return false;
            }

            public Cinemachine.AxisState.Recentering ToRecentering()
            {
                return new AxisState.Recentering(this.m_enabled,this.m_WaitTime,this.m_RecenteringTime);
            }
        }
    }

}
