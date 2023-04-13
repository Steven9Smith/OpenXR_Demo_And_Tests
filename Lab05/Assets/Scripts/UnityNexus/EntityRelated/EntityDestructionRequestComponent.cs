using System;
using System.Collections;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace UnityNexus.EntityRelated
{
    public class EntityDestructionRequestComponent : MonoBehaviour
    {
        public bool isPrefab;
        public bool changeEntityToNewlySpawnedEntity;
        public Transform entity;
        public float DestroyAfterXSeconds;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
    public class EntityDestructionRequestComponentBaker : Baker<EntityDestructionRequestComponent>
    {
        public override void Bake(EntityDestructionRequestComponent authoring)
        {
            Entity e = GetEntity(authoring.entity,TransformUsageFlags.Dynamic);
            AddComponent(e,new EntityDestructionRequest { 
                entity = e,
                destroyAfterXSeconds = authoring.DestroyAfterXSeconds,
                changeEntityToNewlySpawnedEntity = authoring.changeEntityToNewlySpawnedEntity
            });
            if (authoring.isPrefab)
                SetComponentEnabled<EntityDestructionRequest>(e, false);
        }
    }
    [Serializable]
    public struct EntityDestructionRequest : IComponentData,IEnableableComponent
    {
        public bool changeEntityToNewlySpawnedEntity;
        public Entity entity;
        public float destroyAfterXSeconds;
    }
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct EntityDestructionSystem : ISystem
    {
        /* public void OnCreate(ref SystemState state)
         {

         }
         public void OnDestroy(ref SystemState state)
         {

         }*/
        public void OnUpdate(ref SystemState state)
        {
            var ecb = GetEntityCommandBuffer(ref state);
            foreach (RefRW<EntityDestructionRequest> request
                in SystemAPI.Query<RefRW<EntityDestructionRequest>>())
            {
                if (request.ValueRO.destroyAfterXSeconds <= 0)
                {
                    if (SystemAPI.HasComponent<TransformGOTag>(request.ValueRO.entity))
                    {
                        // this is managed...gotta firgure out how to make this...nicer
                        var go = SystemAPI.ManagedAPI.GetComponent<TransformGO>(request.ValueRO.entity).transform.gameObject;
                        GameObject.Destroy(go);
                    }
                    ecb.DestroyEntity(request.ValueRW.entity);

                }
                else
                    request.ValueRW.destroyAfterXSeconds -= Time.deltaTime;
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