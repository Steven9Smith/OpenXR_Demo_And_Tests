
using System;
using System.Collections;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;
using UnityEngine;

namespace UnityNexus.EntityRelated
{
    [Serializable]
    public struct EntityStatus : IComponentData
    {
        public int currentHP,maxHP,defense,
            attack,spDefense,spAttack;
        public float normalSpeed,maxSpeed,currentSpeed;

    }
    [Serializable]
    public struct EntityStatusChange : IEnableableComponent,IComponentData {
        public float lastForXSeconds;
        public int hpChange,defenseChange,attackChange
            ,spDefenseChange,spAttackChange;
        public float normalSpeedChange, maxSpeedChange;

        public void Reset()
        {
            hpChange = 0;
            defenseChange = 0;
            attackChange = 0;
            spDefenseChange = 0;
            spAttackChange = 0;
            normalSpeedChange = 0;
            maxSpeedChange = 0;
            lastForXSeconds = 0;
        }
        public void AddChange(EntityStatusChange change)
        {
            lastForXSeconds += change.lastForXSeconds;
            hpChange += change.hpChange;
            defenseChange += change.defenseChange;
            attackChange += change.spDefenseChange;
            spDefenseChange += change.spAttackChange;
            spAttackChange += change.spAttackChange;
            normalSpeedChange += change.normalSpeedChange;
                maxSpeedChange += change.maxSpeedChange;

        }
    }

    /// <summary>
    /// This struct is only used to show some options in the Component version of this
    /// </summary>
    [Serializable]
    public struct EntityPrefabSpawnRequestDataForComponent
    {
        [Header("Entity Spawining Specific Options")]
        public int id;
        public int spawnAmount;
        public bool canNewEntityKeepEntityPrefabData;
        [Header("Set Transform on Spawn Options")]
        public bool setLocalTransform;
        public bool copyLocalTransformFromRequestEntity;
        public bool copyLTWFromRequestEntity;
        public Vector3 position;
        public Quaternion rotation;

        public EntityPrefabSpawnRequest ToEntityPrefabSpawnRequest(Entity spawner,Entity spawnee)
        {
            return new EntityPrefabSpawnRequest
            {
                canNewEntityKeepEntityPrefabData = canNewEntityKeepEntityPrefabData,
                entity = spawnee,
                spawnedBy = spawner,
                localTransform = new LocalTransform
                {
                    Position = position,
                    Rotation = rotation,
                    Scale = 1
                },
                setLocalTransform = setLocalTransform,
                spawnAmount = spawnAmount,
                copyLocalTransformFromRequestEntity = copyLocalTransformFromRequestEntity,
                copyLTWFromRequestEntity = copyLTWFromRequestEntity
            };
        }
    }
    
    [Serializable]
    public struct EntityPrefabDataReference : IComponentData
    {
        public int id;
        public Entity Prefab;
        public bool canNewEntityKeepEntityPrefabData;

        public bool setLocalTransform;
        public bool copyLocalTransformFromRequestEntity;
        public bool copyLTWFromRequestEntity;
        public LocalTransform localTransform;
    }
    [Serializable]
    public struct EntityPrefabSpawnRequest : IComponentData
    {
        public Entity entity,spawnedBy;
        public int spawnAmount;
        public bool canNewEntityKeepEntityPrefabData;

        public bool setLocalTransform;
        public bool copyLocalTransformFromRequestEntity;
        public bool copyLTWFromRequestEntity;
        public LocalTransform localTransform;
    }
   


    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(TriggerRelated.TriggerEventAuthoringSystem))]
    [BurstCompile]
    public partial struct EntityAuthoringSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (status, statusChange,entity) in
                SystemAPI.Query<RefRW<EntityStatus>, RefRW<EntityStatusChange>>().WithEntityAccess())
            {
                if (statusChange.ValueRO.lastForXSeconds > 0)
                {
                    status.ValueRW.currentHP += statusChange.ValueRO.hpChange;
                    statusChange.ValueRW.lastForXSeconds -= Time.deltaTime;
                }
                else
                {
                    statusChange.ValueRW.Reset();
                    SystemAPI.SetComponentEnabled<EntityStatusChange>(entity, false);
                }
            }
        }
    }



    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [BurstCompile]
    public partial struct EntitySpawnerSystem : ISystem
    {

        public void OnUpdate(ref SystemState state)
        {
            var ecb = GetEntityCommandBuffer(ref state);
       
            foreach (var (spawnRequest,ltw,localTransform, entity) in SystemAPI.Query<RefRW<EntityPrefabSpawnRequest>,RefRO<LocalToWorld>,RefRO<LocalTransform>>().WithEntityAccess())
            {
                // to track this error set "Pause On Error" to true and find the entity
                // that has a EntityPrefabSpawnRequest
                if (spawnRequest.ValueRO.entity == Entity.Null)
                    Debug.LogError($" Got a entity request with a null Entity! spawned by {spawnRequest.ValueRO.spawnedBy.ToString()}");
                else
                {
                    Entity newEntity = ecb.Instantiate(spawnRequest.ValueRO.entity);

                    LocalTransform newLT = spawnRequest.ValueRO.setLocalTransform ? spawnRequest.ValueRO.localTransform
                        : spawnRequest.ValueRO.copyLocalTransformFromRequestEntity ? localTransform.ValueRO
                        : spawnRequest.ValueRO.copyLTWFromRequestEntity ? new LocalTransform { Position = ltw.ValueRO.Position, Rotation = ltw.ValueRO.Rotation,Scale = 1f}
                        : LocalTransform.Identity;
                    //TODO: clean this function up
                    ecb.SetComponent<LocalTransform>(newEntity,newLT);

                    //    Debug.Log($"spawning at {newLT.Position},{ltw.ValueRO.Position},{localTransform.ValueRO.Position} {spawnRequest.ValueRO.setLocalTransform},{spawnRequest.ValueRO.copyLocalTransformFromRequestEntity},{spawnRequest.ValueRO.copyLTWFromRequestEntity}");
                    if (SystemAPI.HasComponent<EntityDestructionRequest>(spawnRequest.ValueRO.entity))
                    {
                        var destruct = SystemAPI.GetComponent<EntityDestructionRequest>(spawnRequest.ValueRO.entity);
                        if (destruct.changeEntityToNewlySpawnedEntity)
                        {
                            destruct.entity = newEntity;
                            // enable the request
                            if (destruct.destroyAfterXSeconds >= 0)
                                ecb.SetComponentEnabled<EntityDestructionRequest>(newEntity, true);
                            ecb.SetComponent(newEntity, destruct);
                        }
                    }
                 //   else Debug.Log($"entity {spawnRequest.ValueRO.entity.ToString()} does have sestruct");
                    if (!spawnRequest.ValueRO.canNewEntityKeepEntityPrefabData)
                        ecb.RemoveComponent<EntityPrefabDataReference>(newEntity);
                    if (SystemAPI.HasComponent<PrefabGOTag>(spawnRequest.ValueRO.entity))
                        //have to initialize a new prefab
                        ecb.AddComponent<PrefabInitializeRequest>(newEntity);

                    ecb.DestroyEntity(entity);
                }
            }
        }



        [BurstCompile]

        private EntityCommandBuffer GetEntityCommandBuffer(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            return ecb;
        }

     /*   [BurstCompile]
        public partial struct ProcessSpawnerJob : IJobEntity
        {
            public EntityCommandBuffer Ecb;
            public double ElapsedTime;

            // IJobEntity generates a component data query based on the parameters of its `Execute` method.
            // This example queries for all Spawner components and uses `ref` to specify that the operation
            // requires read and write access. Unity processes `Execute` for each entity that matches the
            // component data query.
            private void Execute(*//*[ChunkIndexInQuery] int chunkIndex,*//*
                Entity e, ref LocalTransform localTransform, ref EntityPrefabSpawnData spawner)
            {
                // If the next spawn time has passed.
                *//* if (spawner.NextSpawnTime < ElapsedTime)
                 {
                     // Spawns a new entity and positions it at the spawner.
                     Entity newEntity = Ecb.Instantiate(chunkIndex, spawner.Prefab);
                     Ecb.SetComponent(chunkIndex, newEntity, LocalTransform.FromPosition(spawner.SpawnPosition));

                     // Resets the next spawn time.
                     spawner.NextSpawnTime = (float)ElapsedTime + spawner.SpawnRate;
                 }*//*

                // Ecb.DestroyEntity(chunkIndex, e);
            }
        }*/
    }
}