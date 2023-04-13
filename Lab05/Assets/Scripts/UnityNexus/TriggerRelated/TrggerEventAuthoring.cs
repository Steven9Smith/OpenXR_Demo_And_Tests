using System.Collections;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;
using Unity.Collections;
using UnityNexus.EntityRelated;
using Unity.Transforms;
using Unity.Burst;
using System;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using Unity.Physics.Systems;

namespace UnityNexus.TriggerRelated
{
    [Serializable]
    public struct TriggerEntitySpawnEvent : IComponentData,IEnableableComponent
	{
        public EntityPrefabSpawnRequest spawnRequest;
        public bool spawnAtHit, spawnOnTriggerEntity, spawnOnHitEntity;
    }
    [Serializable]
    public struct TriggerEntityStatusChangeEvent : IComponentData, IEnableableComponent
    {
        public bool enableStatusChangeOnEvent;
        public EntityStatusChange change;
    }
    [Serializable]
    public struct TriggerEventStatusTracker : IComponentData,IEnableableComponent
	{
        public static bool ExecuteNow(ref TriggerEventStatusTracker tracker,byte triggerState)
        {
            switch (triggerState)
            {
                case 0: // StartTriggerJob
                    
                    tracker.startJobTriggered = true;
                    bool s = tracker.startJobTriggered && !tracker.middleJobTriggered && !tracker.endJobTriggered;
                    if (s)
                    {
                 //       Debug.Log("Resetting!");
                        tracker.endJobAlreadyTriggered = false;
                    }
                    return s && tracker.executeOnStart;
                case 1: //MiddleTriggerJob
                    tracker.middleJobTriggered = true;
                    return tracker.executeOnMiddle;
                case 2: //End/Update TriggerStatusJob
                    tracker.endJobTriggered = true;
                    bool e = !tracker.startJobTriggered && !tracker.middleJobTriggered && tracker.endJobTriggered && !tracker.endJobAlreadyTriggered;
                    if (e) tracker.endJobAlreadyTriggered = true;
                    return e && tracker.executeOnEnd;
                default: //clean up
                    //player has left the trigger and the endJobTirggered wasn't set off yet
                    if (!tracker.endJobAlreadyTriggered && !tracker.startJobTriggered && !tracker.middleJobTriggered)
                    {
                        tracker.endJobAlreadyTriggered = true;
                        tracker.endJobTriggered = true;
                        tracker.startJobTriggered = false;
                        tracker.middleJobTriggered = false;
                        return true;
                    }
                    // player has already left the trigger zone but the end trigger effect has already been triggered
                    else if (tracker.endJobAlreadyTriggered && !tracker.middleJobTriggered && !tracker.startJobTriggered)
                    {
                        tracker.endJobTriggered = false;
                    }
                    else if (tracker.startJobTriggered || tracker.middleJobTriggered)
                        tracker.endJobTriggered = true;
                    tracker.startJobTriggered = false;
                    tracker.middleJobTriggered = false;
                    return false;
            }
		}
        public Entity otherEntityThatCausedTrigger;
        [MarshalAs(UnmanagedType.U1)]
        public bool executeOnStart;
        [MarshalAs(UnmanagedType.U1)]
        public bool executeOnMiddle;
        [MarshalAs(UnmanagedType.U1)]
        public bool executeOnEnd;
        // make internal later
        [MarshalAs(UnmanagedType.U1)]
        public bool startJobTriggered;
        [MarshalAs(UnmanagedType.U1)]
        public bool middleJobTriggered;
        [MarshalAs(UnmanagedType.U1)]
        public bool endJobTriggered;
        [MarshalAs(UnmanagedType.U1)]
        public bool endJobAlreadyTriggered;
	}
    [Serializable]
    public struct IsECSTriggerEvent : IComponentData { }

    [UpdateInGroup(typeof(PhysicsSimulationGroup))]
    [UpdateBefore(typeof(ExportPhysicsWorld))]
    [UpdateAfter(typeof(PhysicsSolveAndIntegrateGroup))]
    [BurstCompile]
    public partial struct TriggerEventAuthoringSystem : ISystem
    {
        private static void HandleTriggerStatusCleanup(in byte triggerState,in Entity entity,
            ref ComponentLookup<TriggerEventStatusTracker> TriggerEventStatusTrackerLookup,
            out bool triggerNow, out TriggerEventStatusTracker triggerEventStatusTracker)
        {
            if (!TriggerEventStatusTrackerLookup.HasComponent(entity))
            {
                Debug.LogError("TrggerEventAuthoringSystem: Detected an invalid entity that entered this Job. Are you not using an EntityQuery?");
                triggerNow = false;
                triggerEventStatusTracker = new TriggerEventStatusTracker();
            }
            else
            {
                triggerEventStatusTracker = TriggerEventStatusTrackerLookup[entity];
                triggerNow = TriggerEventStatusTracker.ExecuteNow(ref triggerEventStatusTracker,triggerState);
                TriggerEventStatusTrackerLookup[entity] = triggerEventStatusTracker;
            //    Debug.Log($"New Trigger Status: {triggerState},{triggerEventStatusTracker.startJobTriggered},{triggerEventStatusTracker.middlejobTriggered}," +
            //        $"{triggerEventStatusTracker.endJobTriggered},{triggerEventStatusTracker.endJobAlreadyTriggered}");
            }
            
        }

        private static void HandleTriggerStatus(
            in byte triggerState, in Entity EntityA,in Entity EntityB,
            ref ComponentLookup<TriggerEventStatusTracker> TriggerEventStatusTrackerLookup,
            out bool triggerNow,out Entity other)
		{
            triggerNow = false;
            other = EntityB;
            if (TriggerEventStatusTrackerLookup.HasComponent(EntityA))
            {
                var triggerEventStatus = TriggerEventStatusTrackerLookup[EntityA];
                if (EntityB != Entity.Null)
                {
                    triggerEventStatus.otherEntityThatCausedTrigger = EntityB;
                    //idk if i use this, maybe delete later
                    other = triggerEventStatus.otherEntityThatCausedTrigger;
                }
                else if (EntityB == null && triggerEventStatus.otherEntityThatCausedTrigger != Entity.Null)
                    other = triggerEventStatus.otherEntityThatCausedTrigger;
                triggerNow = TriggerEventStatusTracker.ExecuteNow(ref triggerEventStatus,triggerState);
             //   if(triggerNow) Debug.Log($"Trigger Status Change Detected On {EntityA}!");
                TriggerEventStatusTrackerLookup[EntityA] = triggerEventStatus;
            //    Debug.Log($"New Trigger Status: {triggerEventStatus.startJobTriggered},{triggerEventStatus.middlejobTriggered}," +
            //        $"{triggerEventStatus.endJobTriggered},{triggerEventStatus.endJobAlreadyTriggered}");
            }
        }

        private static void HandleTriggerSpawnEvent(
            in Entity EntityA,in Entity EntityB, ref EntityCommandBuffer ecb,
            in PhysicsWorld world,
            in ComponentLookup<PhysicsCollider> physicsColliderLookup,
            in ComponentLookup<PhysicsVelocity> physicsVelocityLookup,
            ref ComponentLookup<TriggerEntitySpawnEvent> TriggerSpawnEventLookup,
            in ComponentLookup<LocalToWorld> LocalToWorldLookup,
            in ComponentLookup<LocalTransform> LocalTransformLookup)
		{
            if (TriggerSpawnEventLookup.HasComponent(EntityA))
            {
                var spawnEvent = TriggerSpawnEventLookup[EntityA];
                Entity e = ecb.CreateEntity();
                var ltw = LocalToWorldLookup[spawnEvent.spawnRequest.spawnedBy];
                ecb.AddComponent<EntityPrefabSpawnRequest>(e, spawnEvent.spawnRequest);
                LocalTransform newLT = spawnEvent.spawnRequest.setLocalTransform ? spawnEvent.spawnRequest.localTransform
                      : spawnEvent.spawnRequest.copyLocalTransformFromRequestEntity ? LocalTransformLookup[spawnEvent.spawnRequest.spawnedBy]
                      : spawnEvent.spawnRequest.copyLTWFromRequestEntity ? new LocalTransform { Position = ltw.Position, Rotation = ltw.Rotation, Scale = 1f }
                      : LocalTransform.Identity;
                if (spawnEvent.spawnOnHitEntity)
                {
                    ltw = LocalToWorldLookup[EntityB];
                    newLT = new LocalTransform
                    {
                        Position = ltw.Position,
                        Rotation = ltw.Rotation,
                        Scale = 1f
                    };
                }
                else if (spawnEvent.spawnOnTriggerEntity)
                {
                    ltw = LocalToWorldLookup[EntityA];
                    newLT = new LocalTransform
                    {
                        Position = ltw.Position,
                        Rotation = ltw.Rotation,
                        Scale = 1f
                    };
                }
                else if (spawnEvent.spawnAtHit)
                {
                    var collider = physicsColliderLookup[EntityA];
                    var velocity = physicsVelocityLookup[EntityA];
                    ltw = LocalToWorldLookup[EntityA];
                    newLT = new LocalTransform
                    {
                        Position = ltw.Position,
                        Rotation = ltw.Rotation,
                        Scale = 1f
                    };
                    ColliderCastInput input = new ColliderCastInput(collider.Value,
                        newLT.Position, newLT.Position + velocity.Linear, ltw.Rotation);
                    if (world.CastCollider(input, out ColliderCastHit closestHit))
                    {
                        newLT = new LocalTransform
                        {
                            Position = closestHit.Position,
                            Rotation = quaternion.identity,
                            Scale = 1f
                        };
                   //     Debug.Log($"Got hit at {closestHit.Position},{closestHit.SurfaceNormal},{ltw.Rotation}");
                    }
                   // Debug.LogWarning($"{velocity.Linear},{newLT.Position},{input.Start},{input.End},{input.Orientation}");

                }
           //     Debug.Log($"spawning at {newLT.Position},{spawnEvent.spawnOnHitEntity},{spawnEvent.spawnAtHit},{spawnEvent.spawnOnTriggerEntity}");
                ecb.AddComponent<LocalTransform>(e, newLT);
                ecb.AddComponent<LocalToWorld>(e, new LocalToWorld { Value = new float4x4(newLT.Rotation,newLT.Position)});
            }
        }

        [BurstCompile]
        private static void HandleStatusChangeEvent(
            in Entity EntityA, in Entity EntityB,ref EntityCommandBuffer ecb,
            ref ComponentLookup<TriggerEntityStatusChangeEvent> TriggerEntityStatusChangeEventLookup,
            ref ComponentLookup<EntityStatusChange> EntityStatusChangeLookup)
		{
           // Debug.Log("A");
            if (TriggerEntityStatusChangeEventLookup.HasComponent(EntityA))
            {
             //   Debug.Log("B");
                if (EntityStatusChangeLookup.HasComponent(EntityB))
                {
               //     Debug.Log("C");
                    var changeEvent = TriggerEntityStatusChangeEventLookup[EntityA];
                    var change = EntityStatusChangeLookup.GetRefRW(EntityB, false);
                    change.ValueRW.AddChange(changeEvent.change);
                    if (changeEvent.enableStatusChangeOnEvent)
                    {
                 //       Debug.Log("D");
                        EntityStatusChangeLookup.SetComponentEnabled(EntityB, true);
                        //      ecb.SetComponentEnabled<EntityStatusChange>(EntityB, true);
                    }
                }
            }
        }
        ///////////////
        ///  JOBS  ////
        ///////////////
        #region Burst_CollisionTriggerEvent_Jobs
        [BurstCompile]
        public struct TriggerCollisionEventStartSystem : ITriggerEventsJob
        {
            public EntityCommandBuffer ecb;
            public ComponentLookup<TriggerEventStatusTracker> TriggerEventStatusTrackerLookup;
            public ComponentLookup<LocalToWorld> LocalToWorldLookup;
            public ComponentLookup<LocalTransform> LocalTransformLookup;

            [ReadOnly] public PhysicsWorld m_PhysicsWorld;
            public ComponentLookup<PhysicsCollider> PhysicsColliderLookup;
            public ComponentLookup<PhysicsVelocity> PhysicsVelocityLookup;

            public ComponentLookup<TriggerEntitySpawnEvent> TriggerEntitySpawnEventLookup;

            public ComponentLookup<TriggerEntityStatusChangeEvent> TriggerEntityStatusChangeEventLookup;
            public ComponentLookup<EntityStatusChange> EntityStatusChangeLookup;

            private static readonly byte triggerState = 0; // start
            public void Execute(TriggerEvent triggerEvent)
            {
                

                //update trigger status
                HandleTriggerStatus(triggerState, triggerEvent.EntityA, triggerEvent.EntityB, ref TriggerEventStatusTrackerLookup,
                 out bool triggerANow, out Entity otherA);
                HandleTriggerStatus(triggerState, triggerEvent.EntityB, triggerEvent.EntityA, ref TriggerEventStatusTrackerLookup,
                    out bool triggerBNow, out Entity otherB);
                if (triggerANow)
				{
                //    Debug.Log("Trigger Start A!");
                    HandleTriggerSpawnEvent(triggerEvent.EntityA,triggerEvent.EntityB, ref ecb,
                        m_PhysicsWorld,PhysicsColliderLookup,PhysicsVelocityLookup,
                        ref TriggerEntitySpawnEventLookup,LocalToWorldLookup,LocalTransformLookup);
                    HandleStatusChangeEvent(triggerEvent.EntityA,triggerEvent.EntityB,
                        ref ecb, ref TriggerEntityStatusChangeEventLookup,
                        ref EntityStatusChangeLookup);
                }
                if (triggerBNow)
                {
                 //   Debug.Log("Trigger Start B!");
                    HandleTriggerSpawnEvent(triggerEvent.EntityB,triggerEvent.EntityA, ref ecb,
                         m_PhysicsWorld, PhysicsColliderLookup, PhysicsVelocityLookup, 
                         ref TriggerEntitySpawnEventLookup, LocalToWorldLookup, LocalTransformLookup);
                    HandleStatusChangeEvent(triggerEvent.EntityB, triggerEvent.EntityA,
                        ref ecb, ref TriggerEntityStatusChangeEventLookup,
                        ref EntityStatusChangeLookup);
                }
            }
        }

        [BurstCompile]
        public struct TriggerCollisionEventMiddleSystem : ITriggerEventsJob
        {
            public EntityCommandBuffer ecb;
            public ComponentLookup<TriggerEventStatusTracker> TriggerEventStatusTrackerLookup;
            public ComponentLookup<LocalToWorld> LocalToWorldLookup;
            public ComponentLookup<LocalTransform> LocalTransformLookup;


            [ReadOnly] public PhysicsWorld m_PhysicsWorld;
            public ComponentLookup<PhysicsCollider> PhysicsColliderLookup;
            public ComponentLookup<PhysicsVelocity> PhysicsVelocityLookup;
            public ComponentLookup<TriggerEntitySpawnEvent> TriggerEntitySpawnEventLookup;

            public ComponentLookup<TriggerEntityStatusChangeEvent> TriggerEntityStatusChangeEventLookup;
            public ComponentLookup<EntityStatusChange> EntityStatusChangeLookup;

            private static readonly byte triggerState = 1; // middle
            public void Execute(TriggerEvent triggerEvent)
            {

                //update trigger status
                HandleTriggerStatus(triggerState, triggerEvent.EntityA,triggerEvent.EntityB, ref TriggerEventStatusTrackerLookup,
                    out bool triggerANow, out Entity otherA);
                HandleTriggerStatus(triggerState, triggerEvent.EntityB,triggerEvent.EntityA, ref TriggerEventStatusTrackerLookup,
                    out bool triggerBNow, out Entity otherB);
                if (triggerANow)
                {
                    Debug.Log("Trigger Start A!");
                    HandleTriggerSpawnEvent(triggerEvent.EntityA,triggerEvent.EntityB, ref ecb,
                        m_PhysicsWorld, PhysicsColliderLookup, PhysicsVelocityLookup, 
                        ref TriggerEntitySpawnEventLookup, LocalToWorldLookup, LocalTransformLookup);
                    HandleStatusChangeEvent(triggerEvent.EntityA, triggerEvent.EntityB,
                        ref ecb, ref TriggerEntityStatusChangeEventLookup,
                        ref EntityStatusChangeLookup);
                }
                if (triggerBNow)
                {
                    Debug.Log("Trigger Start B!");
                    HandleTriggerSpawnEvent(triggerEvent.EntityB,triggerEvent.EntityA, ref ecb,
                        m_PhysicsWorld, PhysicsColliderLookup, PhysicsVelocityLookup,
                        ref TriggerEntitySpawnEventLookup, LocalToWorldLookup, LocalTransformLookup);
                    HandleStatusChangeEvent(triggerEvent.EntityB, triggerEvent.EntityA,
                        ref ecb, ref TriggerEntityStatusChangeEventLookup,
                        ref EntityStatusChangeLookup);
                }
            }
        }

        [BurstCompile]
        public partial struct TriggerCollisionEventCleanupSystem : IJobEntity
        {

            public EntityCommandBuffer ecb;
            public ComponentLookup<TriggerEventStatusTracker> TriggerEventStatusTrackerLookup;
            public ComponentLookup<LocalToWorld> LocalToWorldLookup;
            public ComponentLookup<LocalTransform> LocalTransformLookup;

            [ReadOnly] public PhysicsWorld m_PhysicsWorld;
            public ComponentLookup<PhysicsCollider> PhysicsColliderLookup;
            public ComponentLookup<PhysicsVelocity> PhysicsVelocityLookup;
            public ComponentLookup<TriggerEntitySpawnEvent> TriggerEntitySpawnEventLookup;

            public ComponentLookup<TriggerEntityStatusChangeEvent> TriggerEntityStatusChangeEventLookup;
            public ComponentLookup<EntityStatusChange> EntityStatusChangeLookup;

            private static readonly byte triggerState = 3; // end
            public void Execute(Entity entity)
            {

                //update trigger status
                HandleTriggerStatusCleanup(triggerState,entity, ref TriggerEventStatusTrackerLookup,
                    out bool triggerNow,out var tracker);
                //update trigger status
                if (triggerNow)
                {
                    Debug.Log("Trigger Start A!");
                    HandleTriggerSpawnEvent(entity,tracker.otherEntityThatCausedTrigger, ref ecb,
                        m_PhysicsWorld, PhysicsColliderLookup, PhysicsVelocityLookup,
                        ref TriggerEntitySpawnEventLookup, LocalToWorldLookup, LocalTransformLookup);
                    HandleStatusChangeEvent(entity, tracker.otherEntityThatCausedTrigger,
                        ref ecb, ref TriggerEntityStatusChangeEventLookup,
                        ref EntityStatusChangeLookup);
                }


            }
        }
        #endregion

        #region Burst_TriggerEvent_Jobs
/*
        [BurstCompile]
        public partial struct TriggerEventStartSystem : IJobEntity
        {
            public EntityCommandBuffer ecb;
            public ComponentLookup<TriggerEventStatusTracker> TriggerEventStatusTrackerLookup;

            public ComponentLookup<LocalToWorld> LocalToWorldLookup;
            public ComponentLookup<LocalTransform> LocalTransformLookup;
            public ComponentLookup<TriggerEntitySpawnEvent> TriggerEntitySpawnEventLookup;

            public ComponentLookup<TriggerEntityStatusChangeEvent> TriggerEntityStatusChangeEventLookup;
            public ComponentLookup<EntityStatusChange> EntityStatusChangeLookup;

            private static readonly byte triggerState = 0; // start
            public void Execute(Entity entity)
            {
                //update trigger status
                HandleTriggerStatus(triggerState, entity, Entity.Null, ref TriggerEventStatusTrackerLookup,
                 out bool triggerANow,out Entity other);
                if (triggerANow)
                {
                    Debug.Log("Trigger Start A!");
                    HandleTriggerSpawnEvent(entity, ref ecb, ref TriggerEntitySpawnEventLookup, LocalToWorldLookup, LocalTransformLookup);
                    HandleStatusChangeEvent(entity, Entity.Null,
                        ref ecb, ref TriggerEntityStatusChangeEventLookup,
                        ref EntityStatusChangeLookup);
                }
            }
        }

        [BurstCompile]
        public partial struct TriggerEventMiddleSystem : IJobEntity
        {
            public EntityCommandBuffer ecb;
            public ComponentLookup<TriggerEventStatusTracker> TriggerEventStatusTrackerLookup;

            public ComponentLookup<LocalToWorld> LocalToWorldLookup;
            public ComponentLookup<LocalTransform> LocalTransformLookup;
            public ComponentLookup<TriggerEntitySpawnEvent> TriggerEntitySpawnEventLookup;

            public ComponentLookup<TriggerEntityStatusChangeEvent> TriggerEntityStatusChangeEventLookup;
            public ComponentLookup<EntityStatusChange> EntityStatusChangeLookup;

            private static readonly byte triggerState = 1; // middle
            public void Execute(Entity entity)
            {

                //update trigger status
                HandleTriggerStatus(triggerState, entity, Entity.Null, ref TriggerEventStatusTrackerLookup,
                    out bool triggerANow, out Entity other);
                if (triggerANow)
                {
                    Debug.Log("Trigger Start A!");
                    HandleTriggerSpawnEvent(entity, ref ecb, ref TriggerEntitySpawnEventLookup, LocalToWorldLookup, LocalTransformLookup);
                    HandleStatusChangeEvent(entity, Entity.Null,
                        ref ecb, ref TriggerEntityStatusChangeEventLookup,
                        ref EntityStatusChangeLookup);
                }
            }
        }

        [BurstCompile]
        public partial struct TriggerEventCleanupSystem : IJobEntity
        {

            public EntityCommandBuffer ecb;
            public ComponentLookup<TriggerEventStatusTracker> TriggerEventStatusTrackerLookup;

            public ComponentLookup<LocalToWorld> LocalToWorldLookup;
            public ComponentLookup<LocalTransform> LocalTransformLookup;
            public ComponentLookup<TriggerEntitySpawnEvent> TriggerEntitySpawnEventLookup;

            public ComponentLookup<TriggerEntityStatusChangeEvent> TriggerEntityStatusChangeEventLookup;
            public ComponentLookup<EntityStatusChange> EntityStatusChangeLookup;

            private static readonly byte triggerState = 3; // end
            public void Execute(Entity entity)
            {

                //update trigger status
                HandleTriggerStatusCleanup(triggerState, entity, ref TriggerEventStatusTrackerLookup,
                    out bool triggerNow, out var tracker);
                //update trigger status
                if (triggerNow)
                {
                    Debug.Log("Trigger Start A!");
                    HandleTriggerSpawnEvent(entity, ref ecb, ref TriggerEntitySpawnEventLookup, LocalToWorldLookup, LocalTransformLookup);
                    HandleStatusChangeEvent(entity, tracker.otherEntityThatCausedTrigger,
                        ref ecb, ref TriggerEntityStatusChangeEventLookup,
                        ref EntityStatusChangeLookup);
                }


            }
        }
       */

        #endregion

        public ComponentLookup<TriggerEventStatusTracker> TriggerEventStatusTrackerLookup;
        public ComponentLookup<LocalToWorld> LocalToWorldLookup;
        public ComponentLookup<LocalTransform> LocalTransformLookup;
        public ComponentLookup<PhysicsCollider> PhysicsColliderLookup;
        public ComponentLookup<PhysicsVelocity> PhysicsVelocityLookup;

        public ComponentLookup<TriggerEntitySpawnEvent> TriggerEntitySpawnEventLookup;

        public ComponentLookup<TriggerEntityStatusChangeEvent> TriggerEntityStatusChangeEventLookup;
        public ComponentLookup<EntityStatusChange> EntityStatusChangeLookup;

        EntityQuery TriggerCollisionEventStatusTrackerQuery;
        EntityQuery TriggerEventStatusTrackerQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            EntityStatusChangeLookup = SystemAPI.GetComponentLookup<EntityStatusChange>(false);
       
            TriggerEntityStatusChangeEventLookup = SystemAPI.GetComponentLookup<TriggerEntityStatusChangeEvent>(false);
            TriggerEntitySpawnEventLookup = SystemAPI.GetComponentLookup<TriggerEntitySpawnEvent>(false);
            TriggerEventStatusTrackerLookup = SystemAPI.GetComponentLookup<TriggerEventStatusTracker>(false);
            LocalToWorldLookup = SystemAPI.GetComponentLookup<LocalToWorld>();
            LocalTransformLookup = SystemAPI.GetComponentLookup<LocalTransform>();
            PhysicsVelocityLookup = SystemAPI.GetComponentLookup<PhysicsVelocity>();
            PhysicsColliderLookup = SystemAPI.GetComponentLookup<PhysicsCollider>();

        TriggerCollisionEventStatusTrackerQuery = new EntityQueryBuilder(Allocator.Persistent)
                .WithAllRW<TriggerEventStatusTracker>()
               
                .WithAll<IsECSTriggerEvent>()
                .Build(ref state);
            TriggerEventStatusTrackerQuery = new EntityQueryBuilder(Allocator.Persistent)
                .WithAllRW<TriggerEventStatusTracker>()
                .WithNone<IsECSTriggerEvent>()
                .Build(ref state);
        }
        [BurstCompile]
        public void OnDesroy(ref SystemState state)
        {
            TriggerCollisionEventStatusTrackerQuery.Dispose();
            TriggerEventStatusTrackerQuery.Dispose();
        }
        public void OnUpdate(ref SystemState state)
        {
            SimulationSingleton simulation = SystemAPI.GetSingleton <SimulationSingleton>();
            EntityCommandBuffer ecb = GetEntityCommandBuffer(ref state);
            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;

            TriggerEventStatusTrackerLookup.Update(ref state);
            EntityStatusChangeLookup.Update(ref state);
            TriggerEntitySpawnEventLookup.Update(ref state);
            TriggerEntityStatusChangeEventLookup.Update(ref state);
            LocalToWorldLookup.Update(ref state);
            LocalTransformLookup.Update(ref state);
            PhysicsColliderLookup.Update(ref state);
            PhysicsVelocityLookup.Update(ref state);


            //Start
            state.Dependency = new TriggerCollisionEventStartSystem
            {
                ecb = ecb,
                EntityStatusChangeLookup = EntityStatusChangeLookup,
                TriggerEntityStatusChangeEventLookup = TriggerEntityStatusChangeEventLookup,
                TriggerEntitySpawnEventLookup = TriggerEntitySpawnEventLookup,
                TriggerEventStatusTrackerLookup = TriggerEventStatusTrackerLookup,
                LocalToWorldLookup = LocalToWorldLookup,
                LocalTransformLookup = LocalTransformLookup,
                m_PhysicsWorld = physicsWorld,
                PhysicsColliderLookup = PhysicsColliderLookup,
                 PhysicsVelocityLookup = PhysicsVelocityLookup,
            }.Schedule(simulation, state.Dependency); 
            
            //Middle
            state.Dependency = new TriggerCollisionEventMiddleSystem
            {
                ecb = ecb,
                EntityStatusChangeLookup = EntityStatusChangeLookup,
                TriggerEntityStatusChangeEventLookup = TriggerEntityStatusChangeEventLookup,
                TriggerEntitySpawnEventLookup = TriggerEntitySpawnEventLookup,
                TriggerEventStatusTrackerLookup = TriggerEventStatusTrackerLookup,
                LocalToWorldLookup = LocalToWorldLookup,
                LocalTransformLookup = LocalTransformLookup,
                m_PhysicsWorld = physicsWorld,
                PhysicsColliderLookup = PhysicsColliderLookup,
                PhysicsVelocityLookup = PhysicsVelocityLookup,
            }.Schedule(simulation, state.Dependency);
           
            // End/Cleanup
            state.Dependency = new TriggerCollisionEventCleanupSystem
            {
                ecb = ecb,
                EntityStatusChangeLookup = EntityStatusChangeLookup,
                TriggerEntityStatusChangeEventLookup = TriggerEntityStatusChangeEventLookup,
                TriggerEntitySpawnEventLookup = TriggerEntitySpawnEventLookup,
                TriggerEventStatusTrackerLookup = TriggerEventStatusTrackerLookup,
                LocalToWorldLookup = LocalToWorldLookup,
                LocalTransformLookup = LocalTransformLookup,
                m_PhysicsWorld = physicsWorld,
                PhysicsColliderLookup = PhysicsColliderLookup,
                PhysicsVelocityLookup = PhysicsVelocityLookup,

            }.Schedule(TriggerCollisionEventStatusTrackerQuery, state.Dependency);



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