using System;
using System.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using static Unity.Burst.Intrinsics.X86.Avx;

namespace UnityNexus.Input
{
    public class InputActionAssetToSharedComponentData : MonoBehaviour
    {
        [SerializeField] internal InputActionAsset m_InputActionAsset;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
    public class InputActionAssetToSharedComponentDataBaker : Baker<InputActionAssetToSharedComponentData>
    {
        public override void Bake(InputActionAssetToSharedComponentData authoring)
        {
            if (authoring.m_InputActionAsset == null) Debug.LogError("The Given InputActionAsset is null!");
            var tmp = new InputActionAssetSharedComponentData { Value = authoring.m_InputActionAsset };
            
            //This must be initialized in the input system
            //tmp.Initialize();
            
            //this is single player for now so i'm using a singleton
            AddSharedComponentManaged(GetEntity(TransformUsageFlags.Dynamic),tmp);
        }
    }
    [Serializable]
    // Adding shared component of InputActionAsset for easier accessibility
    public struct InputActionAssetSharedComponentData : ISharedComponentData, IEquatable<InputActionAssetSharedComponentData>,ICloneable
    {
        public InputActionAsset Value;
        [SerializeField]InputAction JumpAction;
        [SerializeField] InputAction CrouchAction;
        [SerializeField] InputAction RunAction;
        [SerializeField] InputAction InteractAction;
        [SerializeField] InputAction ReloadAction;
        [SerializeField] InputAction LeathalAction;
        [SerializeField] InputAction TacticalAction;
        [SerializeField] InputAction MoveAction;
        [SerializeField] InputAction LookAction;
        [SerializeField] InputAction FireAction;
        [SerializeField] InputAction AimAction;
        [SerializeField] InputAction ScrollAction;

        public readonly bool IsJump()
        {
            if (JumpAction == null)
            {
                Debug.LogError("JumpAction is Null!");
                return false;
            }
            return JumpAction.ReadValue<float>() != 0;
        }
        public readonly bool IsRun()
        {
            if (RunAction == null)
            {
                Debug.LogError("RunAction is Null!");
                return false;
            }
            return RunAction.ReadValue<float>() != 0;
        }
        public readonly bool IsInteract()
        {
            if (InteractAction == null)
            {
                Debug.LogError("InteractAction is Null!");
                return false;
            }
            return InteractAction.ReadValue<float>() != 0;
        }
        public readonly bool IsCrouch()
        {
            if (CrouchAction == null)
            {
                Debug.LogError("CrouchAction is Null!");
                return false;
            }
            return CrouchAction.ReadValue<float>() != 0;
        }
        public readonly bool IsLeathal()
        {
            if (LeathalAction == null)
            {
                Debug.LogError("LeathalAction is Null!");
                return false;
            }
            return LeathalAction.ReadValue<float>() != 0;
        }
        public readonly bool IsTactical()
        {
            if (TacticalAction == null)
            {
                Debug.LogError("TacticalAction is Null!");
                return false;
            }
            return TacticalAction.ReadValue<float>() != 0;
        }
        public readonly bool IsFire()
        {
            if (FireAction == null)
            {
                Debug.LogError("FireAction is Null!");
                return false;
            }
            return FireAction.ReadValue<float>() != 0;
        }
        public readonly bool IsAim()
        {
            if (AimAction == null)
            {
                Debug.LogError("AimAction is Null!");
                return false;
            }
            return AimAction.ReadValue<float>() != 0;
        }
        public readonly bool IsReload()
        {
            if (ReloadAction == null)
            {
                Debug.LogError("ReloadAction is Null!");
                return false;
            }
            return ReloadAction.ReadValue<float>() != 0;
        }
        public readonly float2 GetMove()
        {
            if (MoveAction == null)
            {
                Debug.LogError("MoveAction is Null!");
                return float2.zero;
            }
            return MoveAction.ReadValue<Vector2>().normalized;
        }
        public readonly float2 GetLook()
        {
            if (LookAction == null)
            {
            }
            return LookAction.ReadValue<Vector2>().normalized;
        }
        public readonly float GetScroll()
        {
            if(ScrollAction == null)
            {

                Debug.LogError("LookAction is Null!");
                return 0;
            }
            return ScrollAction.ReadValue<float>();
        }

        public void Initialize()
        {
            if (Value != null)
            {
                Value.Enable();
                //Setup Actions
                JumpAction = Value.FindActionMap("GameControl").FindAction("Jump");
                if (JumpAction == null) Debug.LogError("Failed to initialize JumpAction");
                CrouchAction = Value.FindActionMap("GameControl").FindAction("Crouch");
                RunAction = Value.FindActionMap("GameControl").FindAction("Run");
                InteractAction = Value.FindActionMap("GameControl").FindAction("Interact");
                ReloadAction = Value.FindActionMap("GameControl").FindAction("Reload");
                LeathalAction = Value.FindActionMap("GameControl").FindAction("Leathal");
                TacticalAction = Value.FindActionMap("GameControl").FindAction("Tactical");
                MoveAction = Value.FindActionMap("GameControl").FindAction("Move");
                LookAction = Value.FindActionMap("GameControl").FindAction("Look");
                FireAction = Value.FindActionMap("GameControl").FindAction("Fire");
                AimAction = Value.FindActionMap("GameControl").FindAction("Aim");
                ScrollAction = Value.FindActionMap("GameControl").FindAction("Scroll");
                Debug.Log("Sucessfully Initialized New Input System Controls");
            }
            else Debug.LogError("Value is null please set the value before initializing!");
        }

        public void UpdateFirstPersonPlayerInputs(ref FirstPersonPlayerInputs playerInputs, uint fixedTick)
        {
            playerInputs.MoveInput = new float2();
            playerInputs.Crouch = IsCrouch();
            playerInputs.Interact = IsInteract();
            playerInputs.Run = IsRun();
            playerInputs.Leathal = IsLeathal();
            playerInputs.Tactical = IsTactical();
            playerInputs.Reload = IsReload();
            playerInputs.Jump = IsJump(); ;
            playerInputs.Fire = IsFire();
            playerInputs.Aim = IsAim();
            if (playerInputs.Crouch) playerInputs.CrouchPressed.Set(fixedTick);
            if (playerInputs.Run) playerInputs.RunPressed.Set(fixedTick);
            if (playerInputs.Jump) playerInputs.JumpPressed.Set(fixedTick);
            if (playerInputs.Reload) playerInputs.ReloadPressed.Set(fixedTick);
            if (playerInputs.Interact) playerInputs.InteractPressed.Set(fixedTick);
            if (playerInputs.Leathal) playerInputs.LeathalPressed.Set(fixedTick);
            if (playerInputs.Tactical) playerInputs.TacticalPressed.Set(fixedTick);
            if (playerInputs.Fire) playerInputs.FirePressed.Set(fixedTick);
            if (playerInputs.Aim) playerInputs.AimPressed.Set(fixedTick);
            playerInputs.LookInput = GetLook();
            playerInputs.MoveInput = GetMove();
            playerInputs.Scroll = GetScroll();
        }
        public FirstPersonPlayerInputs GenerateFirstPersonPlayerPrefs(uint fixedTick)
        {
            var playerInputs = new FirstPersonPlayerInputs();
            playerInputs.MoveInput = new float2();
            playerInputs.Crouch = IsCrouch();
            playerInputs.Interact = IsInteract();
            playerInputs.Run = IsRun();
            playerInputs.Leathal = IsLeathal();
            playerInputs.Tactical = IsTactical();
            playerInputs.Reload = IsReload();
            playerInputs.Jump = IsJump();
            playerInputs.Fire =IsFire();
            playerInputs.Aim = IsAim();
            if (playerInputs.Crouch) playerInputs.CrouchPressed.Set(fixedTick);
            if (playerInputs.Run) playerInputs.RunPressed.Set(fixedTick);
            if (playerInputs.Jump) playerInputs.JumpPressed.Set(fixedTick);
            if (playerInputs.Reload) playerInputs.ReloadPressed.Set(fixedTick);
            if (playerInputs.Interact) playerInputs.InteractPressed.Set(fixedTick);
            if (playerInputs.Leathal) playerInputs.LeathalPressed.Set(fixedTick);
            if (playerInputs.Tactical) playerInputs.TacticalPressed.Set(fixedTick);
            if (playerInputs.Fire) playerInputs.FirePressed.Set(fixedTick);
            if (playerInputs.Aim) playerInputs.AimPressed.Set(fixedTick);
            playerInputs.LookInput =GetLook();
            playerInputs.MoveInput = GetMove();
            playerInputs.Scroll = GetScroll();
            return playerInputs;
        }

        public object Clone()
        {
            return new InputActionAssetSharedComponentData {
                Value = this.Value,

            };
        }

        public bool Equals(InputActionAssetSharedComponentData other)
        {
            if (Value == null) return other.Value == null;
            return Value.Equals(other.Value);
        }
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
    

}