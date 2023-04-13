using System;
using System.Collections;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using UnityNexus.EntityRelated;

namespace UnityNexus.Physics
{ 
    // this component will apply some forces when the entity spawns, this is removed
    // later.
    [Serializable]
    public struct EntityPrefabSpawnImpulseRequest : IComponentData, IEnableableComponent
    {
        public bool useRealtiveForce;

        public bool applyImpulse;
        public float3 impulsePoint;
        public float3 impulse;
        public bool applyLinearImpulse;
        public float3 linearImpulse;
        public bool applyAngularImpulse;
        public float3 angularImpulse;
    }
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(PhysicsInitializeGroup))]
    [UpdateBefore(typeof(PhysicsSimulationGroup))]
    public partial struct ApplyImpulseSystem : ISystem
    {
        [BurstCompile]
        public partial struct ApplyEntityPrefabSpawningForceApplication : IJobEntity
        {
            public PhysicsWorld World;
            public EntityCommandBuffer ecb;
            public void Execute(Entity entity,
                in EntityPrefabSpawnImpulseRequest spawnForce,
                in PhysicsVelocity physicsVelocity,
                in PhysicsMass mass,in PhysicsCollider collider,
                in LocalToWorld ltw)
            {
                int rbIndex = World.GetRigidBodyIndex(entity);
                if (spawnForce.applyLinearImpulse)
                {
                //    Debug.Log($"Applying Force to {entity.ToFixedString()},{rbIndex}");
                    World.ApplyLinearImpulse(rbIndex,
                        spawnForce.useRealtiveForce ?
                        math.mul(ltw.Rotation,spawnForce.linearImpulse)
                        : spawnForce.linearImpulse);
                }
                if (spawnForce.applyAngularImpulse)
                    World.ApplyAngularImpulse(rbIndex, spawnForce.angularImpulse);
                if (spawnForce.applyImpulse)
                    World.ApplyImpulse(rbIndex, spawnForce.impulse, spawnForce.impulsePoint);


                //remove the request at the end
                ecb.SetComponentEnabled<EntityPrefabSpawnImpulseRequest>(entity, false);
            //    ecb.RemoveComponent<EntityPrefabSpawnImpulseRequest>(entity);
            }
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {

            PhysicsWorldSingleton worldSingleton = SystemAPI.GetSingletonRW<PhysicsWorldSingleton>().ValueRW;
            var ecb = GetEntityCommandBuffer(ref state);
            state.Dependency = new ApplyEntityPrefabSpawningForceApplication
            {
                World = worldSingleton.PhysicsWorld,
                ecb = ecb
            }.Schedule(state.Dependency);

            // If you want to modify world immediately, complete dependency
          //  state.CompleteDependency();
          //  worldSingleton.PhysicsWorld.ApplyImpulse(3, new float3(1, 0, 0), new float3(1, 1, 1));
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