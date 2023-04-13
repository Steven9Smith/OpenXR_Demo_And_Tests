//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.5.1
//     from Assets/Settings/Input/PlayerControlActions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace UnityNexus.Input
{
    public partial class @PlayerControlActions: IInputActionCollection2, IDisposable
    {
        public InputActionAsset asset { get; }
        public @PlayerControlActions()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControlActions"",
    ""maps"": [
        {
            ""name"": ""GameControl"",
            ""id"": ""eb0f4159-4b4d-4406-ba10-43daa8416d5e"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""b3dc37cb-5085-4246-8016-7020b76435b0"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Look"",
                    ""type"": ""PassThrough"",
                    ""id"": ""dace5997-93d5-4205-8a19-653587fc5216"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Value"",
                    ""id"": ""16dc011c-dfe7-4b17-8549-86b4b008bb43"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Crouch"",
                    ""type"": ""Value"",
                    ""id"": ""57b075f6-814a-48dc-a36f-2d0827d90372"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Run"",
                    ""type"": ""Value"",
                    ""id"": ""3c674be6-431a-47d4-9f32-232c15cf6ce3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Value"",
                    ""id"": ""ec4c3495-24ad-42db-a1d2-59b9794c02c5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Reload"",
                    ""type"": ""Value"",
                    ""id"": ""9b502035-6b83-42a7-b504-fff00877a734"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Leathal"",
                    ""type"": ""Value"",
                    ""id"": ""1c9e100e-6960-4661-91a9-a10fde22edaf"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Tactical"",
                    ""type"": ""Value"",
                    ""id"": ""39abb1f5-7e46-4a55-8e58-bf36d793f743"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Fire"",
                    ""type"": ""Value"",
                    ""id"": ""7303fdb1-bd28-4cad-87cc-8335dfb99125"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Aim"",
                    ""type"": ""Value"",
                    ""id"": ""5c0e96e5-234a-4744-976e-bc6513e92d04"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Scroll"",
                    ""type"": ""Value"",
                    ""id"": ""30339764-90c7-43eb-9b6f-77d5db6476d4"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""7a9e2baa-c804-43ee-8cf2-f91e08787e02"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""24098658-2e50-4796-9d49-81f23e75c321"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""80df8623-3cd8-4b90-be90-8087b85fe3d0"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""0513a5e4-8a9a-4940-a422-0c6470754f6a"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""7945fc2a-e012-4509-ad9c-90a65e0fd70b"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""d9f6d0ee-255c-4ad5-a10e-763e9bca38e3"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""51a1e68c-354d-4d1d-932e-503e2d1ab6db"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b8c6ace0-bb67-487f-9511-434c12a1e091"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f6a3f175-b072-4c99-813c-d75fcb1056cb"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""205608dd-4c27-4394-8251-f8974797562d"",
                    ""path"": ""<Keyboard>/c"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Crouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e6fdc1a7-0a3f-4348-b37d-63dd2e743c9c"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Run"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bb8131ea-8700-4721-bfd7-d83c37b8abf5"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""80fe6966-6ee1-42d0-8a09-d10e7fc27b4e"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Reload"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""863f9de8-3fe6-4fc0-bda6-4137f53e7cc8"",
                    ""path"": ""<Keyboard>/g"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Leathal"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ba1ef6c8-c4f8-4f16-967a-2db0e1d2affc"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Tactical"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""90bccb88-065e-4ea0-9330-8904fe30ed24"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Fire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""85ab00cb-b86f-4293-8593-9410538451bb"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Scroll"",
                    ""id"": ""2065a1ea-6190-4efd-bd3b-42bbc1ebd972"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Scroll"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Negative"",
                    ""id"": ""549c4e05-1b27-4a28-b205-4b577a94570d"",
                    ""path"": ""<Mouse>/scroll/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Scroll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Positive"",
                    ""id"": ""2b7fafad-4d3b-43fd-bb9c-c3669a23b883"",
                    ""path"": ""<Mouse>/scroll/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Scroll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // GameControl
            m_GameControl = asset.FindActionMap("GameControl", throwIfNotFound: true);
            m_GameControl_Move = m_GameControl.FindAction("Move", throwIfNotFound: true);
            m_GameControl_Look = m_GameControl.FindAction("Look", throwIfNotFound: true);
            m_GameControl_Jump = m_GameControl.FindAction("Jump", throwIfNotFound: true);
            m_GameControl_Crouch = m_GameControl.FindAction("Crouch", throwIfNotFound: true);
            m_GameControl_Run = m_GameControl.FindAction("Run", throwIfNotFound: true);
            m_GameControl_Interact = m_GameControl.FindAction("Interact", throwIfNotFound: true);
            m_GameControl_Reload = m_GameControl.FindAction("Reload", throwIfNotFound: true);
            m_GameControl_Leathal = m_GameControl.FindAction("Leathal", throwIfNotFound: true);
            m_GameControl_Tactical = m_GameControl.FindAction("Tactical", throwIfNotFound: true);
            m_GameControl_Fire = m_GameControl.FindAction("Fire", throwIfNotFound: true);
            m_GameControl_Aim = m_GameControl.FindAction("Aim", throwIfNotFound: true);
            m_GameControl_Scroll = m_GameControl.FindAction("Scroll", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        public IEnumerable<InputBinding> bindings => asset.bindings;

        public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
        {
            return asset.FindAction(actionNameOrId, throwIfNotFound);
        }

        public int FindBinding(InputBinding bindingMask, out InputAction action)
        {
            return asset.FindBinding(bindingMask, out action);
        }

        // GameControl
        private readonly InputActionMap m_GameControl;
        private List<IGameControlActions> m_GameControlActionsCallbackInterfaces = new List<IGameControlActions>();
        private readonly InputAction m_GameControl_Move;
        private readonly InputAction m_GameControl_Look;
        private readonly InputAction m_GameControl_Jump;
        private readonly InputAction m_GameControl_Crouch;
        private readonly InputAction m_GameControl_Run;
        private readonly InputAction m_GameControl_Interact;
        private readonly InputAction m_GameControl_Reload;
        private readonly InputAction m_GameControl_Leathal;
        private readonly InputAction m_GameControl_Tactical;
        private readonly InputAction m_GameControl_Fire;
        private readonly InputAction m_GameControl_Aim;
        private readonly InputAction m_GameControl_Scroll;
        public struct GameControlActions
        {
            private @PlayerControlActions m_Wrapper;
            public GameControlActions(@PlayerControlActions wrapper) { m_Wrapper = wrapper; }
            public InputAction @Move => m_Wrapper.m_GameControl_Move;
            public InputAction @Look => m_Wrapper.m_GameControl_Look;
            public InputAction @Jump => m_Wrapper.m_GameControl_Jump;
            public InputAction @Crouch => m_Wrapper.m_GameControl_Crouch;
            public InputAction @Run => m_Wrapper.m_GameControl_Run;
            public InputAction @Interact => m_Wrapper.m_GameControl_Interact;
            public InputAction @Reload => m_Wrapper.m_GameControl_Reload;
            public InputAction @Leathal => m_Wrapper.m_GameControl_Leathal;
            public InputAction @Tactical => m_Wrapper.m_GameControl_Tactical;
            public InputAction @Fire => m_Wrapper.m_GameControl_Fire;
            public InputAction @Aim => m_Wrapper.m_GameControl_Aim;
            public InputAction @Scroll => m_Wrapper.m_GameControl_Scroll;
            public InputActionMap Get() { return m_Wrapper.m_GameControl; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(GameControlActions set) { return set.Get(); }
            public void AddCallbacks(IGameControlActions instance)
            {
                if (instance == null || m_Wrapper.m_GameControlActionsCallbackInterfaces.Contains(instance)) return;
                m_Wrapper.m_GameControlActionsCallbackInterfaces.Add(instance);
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Look.started += instance.OnLook;
                @Look.performed += instance.OnLook;
                @Look.canceled += instance.OnLook;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Crouch.started += instance.OnCrouch;
                @Crouch.performed += instance.OnCrouch;
                @Crouch.canceled += instance.OnCrouch;
                @Run.started += instance.OnRun;
                @Run.performed += instance.OnRun;
                @Run.canceled += instance.OnRun;
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
                @Reload.started += instance.OnReload;
                @Reload.performed += instance.OnReload;
                @Reload.canceled += instance.OnReload;
                @Leathal.started += instance.OnLeathal;
                @Leathal.performed += instance.OnLeathal;
                @Leathal.canceled += instance.OnLeathal;
                @Tactical.started += instance.OnTactical;
                @Tactical.performed += instance.OnTactical;
                @Tactical.canceled += instance.OnTactical;
                @Fire.started += instance.OnFire;
                @Fire.performed += instance.OnFire;
                @Fire.canceled += instance.OnFire;
                @Aim.started += instance.OnAim;
                @Aim.performed += instance.OnAim;
                @Aim.canceled += instance.OnAim;
                @Scroll.started += instance.OnScroll;
                @Scroll.performed += instance.OnScroll;
                @Scroll.canceled += instance.OnScroll;
            }

            private void UnregisterCallbacks(IGameControlActions instance)
            {
                @Move.started -= instance.OnMove;
                @Move.performed -= instance.OnMove;
                @Move.canceled -= instance.OnMove;
                @Look.started -= instance.OnLook;
                @Look.performed -= instance.OnLook;
                @Look.canceled -= instance.OnLook;
                @Jump.started -= instance.OnJump;
                @Jump.performed -= instance.OnJump;
                @Jump.canceled -= instance.OnJump;
                @Crouch.started -= instance.OnCrouch;
                @Crouch.performed -= instance.OnCrouch;
                @Crouch.canceled -= instance.OnCrouch;
                @Run.started -= instance.OnRun;
                @Run.performed -= instance.OnRun;
                @Run.canceled -= instance.OnRun;
                @Interact.started -= instance.OnInteract;
                @Interact.performed -= instance.OnInteract;
                @Interact.canceled -= instance.OnInteract;
                @Reload.started -= instance.OnReload;
                @Reload.performed -= instance.OnReload;
                @Reload.canceled -= instance.OnReload;
                @Leathal.started -= instance.OnLeathal;
                @Leathal.performed -= instance.OnLeathal;
                @Leathal.canceled -= instance.OnLeathal;
                @Tactical.started -= instance.OnTactical;
                @Tactical.performed -= instance.OnTactical;
                @Tactical.canceled -= instance.OnTactical;
                @Fire.started -= instance.OnFire;
                @Fire.performed -= instance.OnFire;
                @Fire.canceled -= instance.OnFire;
                @Aim.started -= instance.OnAim;
                @Aim.performed -= instance.OnAim;
                @Aim.canceled -= instance.OnAim;
                @Scroll.started -= instance.OnScroll;
                @Scroll.performed -= instance.OnScroll;
                @Scroll.canceled -= instance.OnScroll;
            }

            public void RemoveCallbacks(IGameControlActions instance)
            {
                if (m_Wrapper.m_GameControlActionsCallbackInterfaces.Remove(instance))
                    UnregisterCallbacks(instance);
            }

            public void SetCallbacks(IGameControlActions instance)
            {
                foreach (var item in m_Wrapper.m_GameControlActionsCallbackInterfaces)
                    UnregisterCallbacks(item);
                m_Wrapper.m_GameControlActionsCallbackInterfaces.Clear();
                AddCallbacks(instance);
            }
        }
        public GameControlActions @GameControl => new GameControlActions(this);
        public interface IGameControlActions
        {
            void OnMove(InputAction.CallbackContext context);
            void OnLook(InputAction.CallbackContext context);
            void OnJump(InputAction.CallbackContext context);
            void OnCrouch(InputAction.CallbackContext context);
            void OnRun(InputAction.CallbackContext context);
            void OnInteract(InputAction.CallbackContext context);
            void OnReload(InputAction.CallbackContext context);
            void OnLeathal(InputAction.CallbackContext context);
            void OnTactical(InputAction.CallbackContext context);
            void OnFire(InputAction.CallbackContext context);
            void OnAim(InputAction.CallbackContext context);
            void OnScroll(InputAction.CallbackContext context);
        }
    }
}