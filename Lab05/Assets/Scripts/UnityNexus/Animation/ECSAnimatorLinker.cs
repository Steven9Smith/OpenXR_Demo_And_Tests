using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Authoring;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
using UnityNexus.AI_Related.NavMesh;
using UnityNexus.EntityRelated;

namespace UnitNexus.Animation
{
    [RequireComponent(typeof(PrefabInstantiationComponent))]
    //This adds and animator reference to an entity
    public class ECSAnimatorLinker : MonoBehaviour
    {
        [Header("Animator Related")]
        [Tooltip("Add the animator's bool parameters here")]
        public ManagedAnimatorBoolParameter[] animatorBoolParameters;
        [Tooltip("Add the animator's trigger parameters here")]
        public ManagedAnimatorTriggerParameter[] animatorTriggerParameters;
        [Tooltip("Add the animator's float parameters here")]
        public ManagedAnimatorFloatParameter[] animatorFloatParameters;
        [Tooltip("Add the animator's int parameters here")]
        public ManagedAnimatorIntParameter[] animatorIntParameters;


        [Header("Bone Entity Linker Related")]
        [Tooltip("Set this to true to update the bone list in the editor")]
        public bool reset;
        [Tooltip("This is used to create the boneLinkerData list")]
        [SerializeField] private PrefabInstantiationComponent prefab;
        [Tooltip("This displays all available game objects that can be linked at runtime. set what entities you want to be linked to the given name")]
        public List<EntityBoneLinkerData> boneLinkerData;

        [Header("ECS Clone Related")]
        public bool GenerateECSClone;
        public bool LinkCloneToBoneData;
        public Transform Parent;
        public string name_prefix,name_suffix;
        public bool AddPhysicsShapeComponent;
        public bool AddPhysicsBodyComponent;

        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                if (GenerateECSClone)
                {
                    GenerateECSClone = false;
                    GenerateClone();
                }
                if (reset)
                    UpdateBoneLinkerData();
                UpdateBoneData();
                
            }
        }
        private void UpdateBoneLinkerData()
        {
            foreach (var a in boneLinkerData)
                if(a.Entity != null) 
                    a.Entity.gameObject.RemoveComponent<PrefabChildEntityLinkComponent>();

            prefab = GetComponent<PrefabInstantiationComponent>();
            if (prefab != null)
            {
                boneLinkerData = new List<EntityBoneLinkerData>();
                List<Transform> children = prefab.Prefab.transform.GetAllChildren();
                for (int i = 0; i < children.Count; i++)
                    boneLinkerData.Add(new EntityBoneLinkerData {
                        Bone = children[i].name,
                        Entity = null
                    });
                reset = false;
            }
            else Debug.LogError("This component requires a PrefabInstantiationComponent...how did you remove it?");

        }
        private void UpdateBoneData()
        {
            for(int i = 0;i < boneLinkerData.Count;i++) {
                var a = boneLinkerData[i];
                if(a.Entity != a.LastEntity)
                {
                    //change detected

                    if(a.LastEntity != null)a.LastEntity.gameObject.RemoveComponent<PrefabChildEntityLinkComponent>();
                    if (a.Entity != null)
                    {
                        var b = a.Entity.gameObject.GetComponent<PrefabChildEntityLinkComponent>();
                        b = b == null ? a.Entity.gameObject.AddComponent<PrefabChildEntityLinkComponent>() : b;
                        b.EntityThatHasPrefab = this.transform;
                        b.PrefabChilName = a.Bone;
                    }
                }
                a.LastEntity = a.Entity;
            }
        }
        private void GenerateClone()
        {
            prefab = GetComponent<PrefabInstantiationComponent>();
            if (prefab != null)
            {
                if (Parent != null)
                {
                    GameObject go = GameObject.Instantiate(prefab.Prefab);
                    go.transform.parent = go.transform;
                    var children = go.transform.GetAllChildren();
                    go.transform.parent = Parent;
                    for(int i = 0; i < children.Count; i++)
                    {
                        var child = children[i];
                        child.name = name_prefix + child.name + name_suffix;
                        if (AddPhysicsBodyComponent) child.AddComponent<PhysicsBodyAuthoring>();
                        if (AddPhysicsShapeComponent) child.AddComponent<PhysicsShapeAuthoring>();
                    }
                    if (LinkCloneToBoneData)
                    {
                        for (int i = 0; i < children.Count; i++)
                        {
                            for(int j = 0; j < boneLinkerData.Count; j++)
                            {
                                var tmp_name = children[i].name;
                                tmp_name = name_prefix == "" ? tmp_name : tmp_name.Replace(name_prefix,""); 
                                tmp_name = name_suffix == "" ? tmp_name : tmp_name.Replace(name_suffix,""); 
                                if (boneLinkerData[j].Bone == tmp_name)
                                    boneLinkerData[j].Entity = children[i];
                            }
                        }
                    }
                }
                else Debug.LogError("Parent is null!");
            }
            else Debug.LogError("This component requires a PrefabInstantiationComponent...how did you remove it?");

        }
    }
        [Serializable]
    public class EntityBoneLinkerData 
    {
        public string Bone;
        public Transform Entity;
        internal Transform LastEntity;
    }
    public class ECSAnimatorLinkerBaker : Baker<ECSAnimatorLinker>
    {
        public override void Bake(ECSAnimatorLinker authoring)
        {
            var e = GetEntity(TransformUsageFlags.Dynamic);
            AddComponentObject(e, new ECSAnimatorPrefabSetup
            {
              //  Prefab = authoring.PresentationPrefab,
                animatorBoolParameters = authoring.animatorBoolParameters,
                animatorFloatParameters = authoring.animatorFloatParameters,
                animatorIntParameters = authoring.animatorIntParameters,
                animatorTriggerParameters = authoring.animatorTriggerParameters
            });
            AddComponent(e, new AnimatorState { });

         
        }
    }

    public struct AnimatorState : IComponentData
    {
        public int stateId;
    }

  //  public struct AnimationTriggerer : IEnableableComponent,

    #region ManagedAnimatorParameters
    [Serializable]
    public struct ManagedAnimatorFloatParameter 
    {
        public string Name;
        public float Value;

    }
    [Serializable]
    public struct ManagedAnimatorBoolParameter 
    {
        public string Name;
        public bool Value;
    }
    [Serializable]
    public struct ManagedAnimatorIntParameter
    {
        public string Name;
        public int Value;
    }
    [Serializable]
    public struct ManagedAnimatorTriggerParameter
    {
        public string Name;
        public bool Value;
    }
    #endregion
    //////////////////////////////////////////////////////////////////
    // these are some values that the animation controller will use //
    //////////////////////////////////////////////////////////////////
    #region AnimatorStructParameters
    public struct AnimatorFloatParameter : IBufferElementData
    {
        public float Value;
        public int NameId;
    }
    public struct AnimatorBoolParameter : IBufferElementData
    {
        public bool Value;
        public int NameId;
    }
    public struct AnimatorIntParameter : IBufferElementData
    {
        public int Value;
        public int NameId;
    }
    public struct AnimatorTriggerParameter : IBufferElementData
    {
        public bool Value;
        public int NameId;
    }
    #endregion
    
    // Creating a managed components
    public class ECSAnimatorPrefabSetup : IComponentData
    {
    //    public GameObject Prefab;
        public ManagedAnimatorBoolParameter[] animatorBoolParameters;
        public ManagedAnimatorTriggerParameter[] animatorTriggerParameters;
        public ManagedAnimatorFloatParameter[] animatorFloatParameters;
        public ManagedAnimatorIntParameter[] animatorIntParameters;
    }
    public class AnimatorGO : IComponentData  { public Animator animator;  }

    public partial struct ECSAnimatorSystem : ISystem
    {
        public static int deathId,speedId;
        public static int fullPathAnimationDeathStateId,fullPathAnimationSpeedStateId;

        public void OnCreate(ref SystemState state)
        {
            //zombie specific ids
            deathId = Animator.StringToHash("death");
            fullPathAnimationDeathStateId = Animator.StringToHash("Base Layer.10-death_fall_backward");
            speedId = Animator.StringToHash("speed");
            fullPathAnimationSpeedStateId = Animator.StringToHash("Base Layer.1-walk_chase");
        }
        public void OnUpdate(ref SystemState state)
        {

            var ecb = GetEntityCommandBuffer(ref state);
            /// This system will create an new instance of the prefab and link the 
            /// Animator and Gameobject to the entity
            /*foreach (var (animatorRef,prefabGO, entity) in SystemAPI.Query<ECSAnimatorPrefabSetup,PrefabGO>().WithEntityAccess())
            {
                int i = 0;
                GameObject go = GameObject.Instantiate(prefabGO.Prefab);
              
            }*/
           
            //Handle entities dying!
            foreach (var ( status,_buffer) in SystemAPI.Query<RefRW<EntityStatus>,DynamicBuffer<AnimatorTriggerParameter>>())
            {
                var buffer = _buffer;
                if(status.ValueRO.currentHP == 0)
                {
                    for (int i = 0; i < buffer.Length; i++)
                        if (buffer[i].NameId == deathId)
                            buffer[i] = new AnimatorTriggerParameter {NameId = buffer[i].NameId, Value = true };
                    status.ValueRW.currentHP--;
                }
            }

            #region AnimatorParameterHandling
       
            foreach (var (animatorGO, _buffer) in SystemAPI.Query<AnimatorGO, DynamicBuffer<AnimatorTriggerParameter>>())
            {
                // this is needed to fix a compiler error as stated in this post
                //https://forum.unity.com/threads/dynamicbuffer-in-foreach-loop.1408333/
                var buffer = _buffer;
                for (int i = 0; i < buffer.Length; i++)
                {
                    if (buffer[i].Value)
                    {
                        animatorGO.animator.SetTrigger(buffer[i].NameId);
                        buffer[i] = new AnimatorTriggerParameter { NameId = buffer[i].NameId, Value = false };
                    }
                }
            }
            foreach (var (animatorGO, buffer) in SystemAPI.Query<AnimatorGO, DynamicBuffer<AnimatorBoolParameter>>())
            {
                for (int i = 0; i < buffer.Length; i++)
                    animatorGO.animator.SetBool(buffer[i].NameId, buffer[i].Value);
            }
            foreach (var (animatorGO, buffer) in SystemAPI.Query<AnimatorGO, DynamicBuffer<AnimatorIntParameter>>())
            {
                for (int i = 0; i < buffer.Length; i++)
                    animatorGO.animator.SetInteger(buffer[i].NameId, buffer[i].Value);
            }
            foreach (var (animatorGO, buffer) in SystemAPI.Query<AnimatorGO, DynamicBuffer<AnimatorFloatParameter>>())
            {
                for (int i = 0; i < buffer.Length; i++)
                    animatorGO.animator.SetFloat(buffer[i].NameId, buffer[i].Value);
            }
            #endregion
            foreach(var (animatorGO,animatorState) in SystemAPI.Query<AnimatorGO, RefRW<AnimatorState>>())
            {
                animatorState.ValueRW.stateId = animatorGO.animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
           //     var a = animatorGO.animator.GetCurrentAnimatorStateInfo(0);
           //     Debug.Log($"hashes = {a.shortNameHash},{a.fullPathHash},{a.nameHash},{a.tagHash},{Animator.StringToHash("Base Layer.0-idle_agressive")}");
            }
            /// This will be responseable for setting the transform to the entity transform
            /// NOTE: nav mesh does it's own thing so we won't mess with that
            foreach (var (goTrasform, goAnimator, animatorState, transform) 
                in SystemAPI.Query<TransformGO, AnimatorGO, RefRO<AnimatorState>, RefRW<LocalTransform>>()
                .WithNone<ECSNavMeshAgent>()) 
            {
               /* if (animatorState.ValueRO.stateId != fullPathAnimationDeathStateId)
                {
                    goTrasform.transform.position = transform.ValueRO.Position;
                    goTrasform.transform.rotation = transform.ValueRO.Rotation;
                }*/
            }
           
            foreach (var (status,_buffer) in SystemAPI.Query<EntityStatus,DynamicBuffer<AnimatorFloatParameter>>())
            {
                var buffer = _buffer;
                for(int i = 0; i < buffer.Length; i++)
                    if (buffer[i].NameId == speedId && status.currentSpeed != buffer[i].Value)
                        buffer[i] = new AnimatorFloatParameter { NameId = buffer[i].NameId,Value = status.currentSpeed };
            }
            
            
            /// this needs to be tested!
            /// this class is responsible for cleaning up some stuff
            foreach (var (goTransform, entity) in SystemAPI.Query<TransformGO>().WithNone<LocalToWorld>().WithEntityAccess())
            {
                if (goTransform.transform != null)
                {
                    GameObject.Destroy(goTransform.transform.gameObject);
                }
                ecb.RemoveComponent<TransformGO>(entity);
            }

        }
        [BurstCompile]
        private EntityCommandBuffer GetEntityCommandBuffer(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            return ecb;
        }
    }
}