using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct FirstPersonPlayer : IComponentData
{
    public Entity ControlledCharacter;
    public float MouseSensitivity;
}

[Serializable]
public struct FirstPersonPlayerInputs : IComponentData
{
    public float2 MoveInput;
    public float2 LookInput;
    // use for things that execute on each fixed update 
    public bool Jump;
    public FixedInputEvent JumpPressed;
    // use for things that execute on each fixed update 
    public bool Crouch;
    public FixedInputEvent CrouchPressed;
    // use for things that execute on each fixed update 
    public bool Run;
    public FixedInputEvent RunPressed;
    // use for things that execute on each fixed update 
    public bool Interact;
    public FixedInputEvent InteractPressed;
    // use for things that execute on each fixed update 
    public bool Reload;
    public FixedInputEvent ReloadPressed;
    // use for things that execute on each fixed update 
    public bool Leathal;
    public FixedInputEvent LeathalPressed;
    // use for things that execute on each fixed update 
    public bool Tactical;
    public FixedInputEvent TacticalPressed;
    // use for things that execute on each fixed update 
    public bool Fire;
    public FixedInputEvent FirePressed;
    // use for things that execute on each fixed update 
    public bool Aim;
    public FixedInputEvent AimPressed;

    public float Scroll;
}