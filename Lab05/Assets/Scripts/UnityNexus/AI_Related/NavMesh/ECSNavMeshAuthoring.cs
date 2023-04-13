using System;
using System.Collections;
using UnitNexus.Animation;
using Unity.AI.Navigation;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.AI;
using UnityNexus.EntityRelated;

namespace UnityNexus.AI_Related.NavMesh
{
    [Serializable]
    public struct ECSNavMeshAgentPrefabSetup : IComponentData{
    }
    [Serializable]
    public struct ECSNavMeshSurfacePrefabSetup : IComponentData {
    }
    [Serializable]
    public struct ECSNavMeshObstaclePrefabSetup : IComponentData {
    };
    [Serializable]
    public class ECSNavOffMeshLinkPrefabSetup : IComponentData {
        //     public OffMeshLink meshLink;
        public Entity ANavMeshSurfaceEntity;
        public Entity start;
        public bool startHasPrefabInstantiateComponent;
        public Entity end;
        public bool endHasPrefabInstantiateComponent;
    }
    public class ECSNavMeshSurface : IComponentData
    {
        public NavMeshSurface navMeshSurface;
    }
    // add this to an entity to force the nav mesh surface to rebuild
    // during runtime
    public struct ECSNavMeshSurfaceRebuild : IComponentData { }
    public class ECSNavMeshAgent : IComponentData
    {
        public NavMeshAgent navMeshAgent;
    }
    public class ECSNavMeshObstacle : IComponentData
    {
        public NavMeshObstacle navMeshObstacle;
    }
    public class ECSNavOffMeshLink : IComponentData
    {
        public OffMeshLink navOffMeshLink;
    }
    public struct ECSNavMeshAgentMoveTo : IComponentData
    {
        // the agent will move to the entity if it is not null
        // otherwise it will move to the position
        public Entity entity;
        public float3 position;
    }
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct ECSNavMeshSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = GetEntityCommandBuffer(ref state);

            // this sets the destination of the NavMeshAgent to either an entity position of float3
            foreach (var (agent, follow) in SystemAPI.Query<ECSNavMeshAgent, RefRO<ECSNavMeshAgentMoveTo>>())
            {
                if (follow.ValueRO.entity != Entity.Null)
                {
                    var ltw = SystemAPI.GetComponent<LocalToWorld>(follow.ValueRO.entity);
                    agent.navMeshAgent.destination = ltw.Position;
                }
                else
                {
                    agent.navMeshAgent.destination = follow.ValueRO.position;
                }
            }
            //moves the NavMeshAgent's entity with the Agent
            foreach (var (goTrasform, agent, transform, status)
               in SystemAPI.Query<TransformGO, ECSNavMeshAgent, RefRW<LocalTransform>, RefRW<EntityStatus>>())
            {
                // move entity
                //    transform.ValueRW.Position = goTrasform.transform.position;
                //    transform.ValueRW.Rotation = goTrasform.transform.rotation;
                // updat speed of agent
                if (status.ValueRO.currentHP > 0)
                {
                    agent.navMeshAgent.speed = status.ValueRO.normalSpeed;
                    status.ValueRW.currentSpeed = status.ValueRO.normalSpeed;
                }
                else agent.navMeshAgent.isStopped = true;
            }

            foreach (var (surface, rebuildRequest, entity) in SystemAPI.Query<ECSNavMeshSurface, RefRW<ECSNavMeshSurfaceRebuild>>().WithEntityAccess())
            {
                surface.navMeshSurface.BuildNavMesh();
                ecb.RemoveComponent<ECSNavMeshSurfaceRebuild>(entity);
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