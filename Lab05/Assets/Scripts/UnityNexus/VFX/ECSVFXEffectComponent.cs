using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityNexus.Input;
using static Unity.Burst.Intrinsics.X86.Avx;

namespace Assets.Scripts.UnityNexus.VFX
{
    public class ECSVFXEffectComponent : MonoBehaviour
    {
        public int id;
        public ParticleSystem m_particleSystem;
        public AudioSource m_audioSource;
        private void OnValidate()
        {
            if(m_particleSystem == null)
                m_particleSystem = GetComponent<ParticleSystem>();
            if(m_audioSource == null)
                m_audioSource = GetComponent<AudioSource>();    
        }
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
    public class ECSVFXEffectComponentBaker : Baker<ECSVFXEffectComponent>
    {
        public override void Bake(ECSVFXEffectComponent authoring)
        {
            AddSharedComponentManaged(GetEntity(TransformUsageFlags.Dynamic),
                new ECSVFXEffect { 
                    audioSource = authoring.m_audioSource,
                    particleSystem = authoring.m_particleSystem,
                    id = authoring.id
                });
        }
    }

    public struct ECSVFXEffect : ISharedComponentData, IEquatable<ECSVFXEffect>,ICloneable
    {
        public bool createNewEntity;
        public int id;
        public ParticleSystem particleSystem;
        public AudioSource audioSource;
        public object Clone()
        {
            return new ECSVFXEffect
            {
                id = id,
                audioSource = audioSource,
                particleSystem = particleSystem
            };
        }

        public bool Equals(ECSVFXEffect other)
        {
            return particleSystem == other.particleSystem && audioSource == other.audioSource &&
                id == other.id;
        }
        public override int GetHashCode()
        {
            return id + particleSystem.GetHashCode() + audioSource.GetHashCode();
        }
    }
    public struct TriggerECSVFXEffect : IComponentData {
        public bool createNewEntity;
        public int ParticleSystemId;
        public int AudioSourceId;
        public float3 position;
        public quaternion rotation;
    }
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(TransformSystemGroup))]
    public partial class ECSVFXEffectSystem : SystemBase
    {
        public bool ranOnce;
        List<ECSVFXEffect> effects;

        EntityArchetype ParticleSystemArchetype;
        EntityArchetype AudioSourceArchetype;
        protected override void OnCreate()
        {
            effects = new List<ECSVFXEffect>();
            ParticleSystemArchetype = EntityManager.CreateArchetype(typeof(ParticleSystem));
            AudioSourceArchetype = EntityManager.CreateArchetype(typeof(AudioSource));
        }
        protected override void OnUpdate()
        {
            if (!ranOnce)
            {
                Entities
                    .WithoutBurst()
                .ForEach((in ECSVFXEffect data) =>
                {
                    if(data.id >= effects.Count) effects.Add(data);
                }).Run();
                ranOnce = true;
            }

            Entities
                .WithName("TriggerECSVFXEffectSystem")
                .WithoutBurst()
                .ForEach((Entity entity,ref TriggerECSVFXEffect trigger) => {
                    if (trigger.createNewEntity)
                    {

                    }
                    else
                    {
                        if (trigger.ParticleSystemId < effects.Count)
                        {
                            effects[trigger.ParticleSystemId].particleSystem.Play();
                        }
                        if (trigger.AudioSourceId < effects.Count)
                        {
                            effects[trigger.AudioSourceId].audioSource.Play();
                        }
                    }
                }).Run();

        }
    }
}