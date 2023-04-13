using Assets.Scripts.UnityNexus.VFX;
using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Scenes;
using Unity.Transforms;
using UnityEngine;
using UnityNexus.Common;
using UnityNexus.EntityRelated;
using UnityNexus.Input;
using UnityNexus.TriggerRelated;
using UnityNexus.Weapons;

namespace UnityNexus.Weapons
{
   
    [Serializable]
    public struct GunComponentData : IComponentData
    {
        public int damage;
        public float spread, range;

        public float timeBetweenShooting;
        [SerializeField] internal float timeLeftBetweenShooting;
        public float timeBetweenShots;
        [SerializeField] internal float timeLeftBetweenShots;
        public float reloadTime;
        [SerializeField] internal float reloadTimeLeft;


        public int magizineSize, bulletsPerTap;
        public bool allowButtonHold;
        [SerializeField] internal int bulletsLeft, bulletsShot;
        [SerializeField] internal bool shooting, readyToShoot, reloading;
        [SerializeField] internal float3 firefireOffsetForward;
        [SerializeField] internal float offsetDistance;
    }


    [Serializable]
    public struct WeaponData : IComponentData
    {
        public WeaponSystem.WeaponType weaponType;
    }

    [Serializable]
    // this also requires the TrigerEntitySpawnEvent struct to be added to the entity
    public struct WeaponDataShootTrigger : IComponentData, IEnableableComponent
    {
        public bool triggerEntitySpawnEvent;
        public EntityPrefabSpawnRequest spawnEvent;
        public EntityPrefabSpawnRequest spawnEvent2;
        public bool triggerEntityStatusChangeEvent;
        public TriggerEntityStatusChangeEvent statusChangeEvent;
        public bool triggerCameraEvent;
    }
    [Serializable]
    public struct WeaponDataAimTrigger : IComponentData, IEnableableComponent
    {
        public bool triggerEntitySpawnEvent;
        public TriggerEntitySpawnEvent spawnEvent;
        public bool triggerEntityStatusChangeEvent;
        public TriggerEntityStatusChangeEvent statusChangeEvent;
        public bool triggerCameraEvent;
    }
  /*  public struct WeaponDataScrollTrigger : IComponentData, IEnableableComponent
    {
        public bool triggerEntitySpawnEvent;
        public bool triggerEntityStatusChangeEvent;
        public bool triggerCameraEvent;
    }*/


    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(TriggerEventAuthoringSystem))]
    [BurstCompile]
    public partial struct WeaponSystem : ISystem//SystemBase
    {
        public enum WeaponType
        {
            None = 0,
            Gun = 1,
            Melee = 2,
            Magic = 3,
            Special = 4
        }
        public static readonly int BulletPrefabID = 5;

        EntityPrefabDataReference BulletPrefab;
        public bool runOnce;

        Unity.Physics.Authoring.PhysicsCategoryTags BulletBelongsTo;
        Unity.Physics.Authoring.PhysicsCategoryTags BulletCollidesWith;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            BulletBelongsTo.Category01 = true;
            BulletCollidesWith.Category02 = true;
            BulletCollidesWith.Category05 = true;
        }

        [BurstCompile]
        public void OnStartRunning(ref SystemState state)
        {

        }

        public void OnUpdate(ref SystemState state)
        {
            if (!runOnce)
            {
                foreach (var (gun, gunTransform, eqipedSlot) in SystemAPI.Query<RefRW<GunComponentData>, RefRW<LocalTransform>, RefRO<EquipedInSlot>>())
                {
                    gun.ValueRW.bulletsLeft = gun.ValueRW.magizineSize;
                    gun.ValueRW.readyToShoot = true;
                }
                foreach (RefRW<EntityPrefabDataReference> prefab in SystemAPI.Query<RefRW<EntityPrefabDataReference>>())
                {
                    if (prefab.ValueRO.id == BulletPrefabID)
                    {
                        Debug.Log("Entity is Set!");
                        BulletPrefab = prefab.ValueRO;
                    }
                }
                if (BulletPrefab.Prefab == Entity.Null)
                    Debug.LogError("Failed to find bullet prefab!");
                runOnce = true;
            }
            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
            var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (gun, transform, gunLTW, bulletSpawnPoint, equipedSlot,shootTrigger) in
                SystemAPI.Query<RefRW<GunComponentData>, RefRW<LocalTransform>,
                    RefRO<LocalToWorld>, RefRO<BulletSpawnPointData>, RefRO<EquipedInSlot>,
                    RefRO<WeaponDataShootTrigger>>())
            {
                //get slot info
                var entitySlot = SystemAPI.GetComponent<EntitySlot>(equipedSlot.ValueRO.slotEquipedIn);
                var playerInput = SystemAPI.GetComponent<FirstPersonPlayerInputs>(entitySlot.playerEntity);

                // Update 
                bool a = gun.ValueRW.readyToShoot;
                if (gun.ValueRW.allowButtonHold) gun.ValueRW.shooting = playerInput.Fire;
                else gun.ValueRW.shooting = playerInput.Fire;
                //  Debug.Log($"{gun.ValueRW.shooting} or {m_InputActionAsset.IsFire()}");
                if (playerInput.Reload && gun.ValueRW.bulletsLeft < gun.ValueRW.magizineSize && !gun.ValueRW.reloading)
                    StartReload(gun);
                if (gun.ValueRO.shooting && gun.ValueRO.readyToShoot && gun.ValueRO.bulletsLeft > 0)
                    Shoot(ref state, gunLTW, SystemAPI.GetComponent<LocalTransform>(bulletSpawnPoint.ValueRO.Value),
                        SystemAPI.GetComponent<LocalToWorld>(bulletSpawnPoint.ValueRO.Value), physicsWorld, gun, transform,
                        shootTrigger.ValueRO, playerInput.Aim ? 0 : 1,ecb);
                Update(SystemAPI.Time.fixedDeltaTime, gun);
            };
        }
        void Update(float fixedDeltaTime, RefRW<GunComponentData> gun)
        {
            // decrement shoot time
            if (!gun.ValueRO.readyToShoot)
            {
                gun.ValueRW.timeLeftBetweenShooting -= fixedDeltaTime;
                if (gun.ValueRO.timeLeftBetweenShooting <= 0) gun.ValueRW.readyToShoot = true;
            }
            // decrement reload time
            if (gun.ValueRO.reloading)
            {
                gun.ValueRW.reloadTimeLeft -= fixedDeltaTime;
                FinishReload(gun);
            }
            // decrement time between shots
            if (gun.ValueRO.timeLeftBetweenShots > 0) gun.ValueRW.timeLeftBetweenShots -= fixedDeltaTime;

        }
        void Shoot(ref SystemState state, RefRO<LocalToWorld> ltw,
            LocalTransform bulletSpawnPointLocalTransform, LocalToWorld bulletSpawnPointLTW, PhysicsWorld physicsWorld,
            RefRW<GunComponentData> gun, RefRW<LocalTransform> transform,
            WeaponDataShootTrigger shootTrigger, int spell,EntityCommandBuffer ecb)
        {
            if (gun.ValueRO.readyToShoot)
            {
                gun.ValueRW.readyToShoot = false;
                Entity spellEntity = ecb.CreateEntity();
                ecb.AddComponent(spellEntity,spell == 0 ? 
                    shootTrigger.spawnEvent :
                    shootTrigger.spawnEvent2);
             //   Debug.Log($"Spawning at {bulletSpawnPointLTW.Position},{bulletSpawnPointLocalTransform.Position}");
                ecb.AddComponent<LocalToWorld>(spellEntity, bulletSpawnPointLTW);
                ecb.AddComponent<LocalTransform>(spellEntity, new LocalTransform { Position = bulletSpawnPointLTW.Position,
                Rotation = bulletSpawnPointLTW.Rotation,Scale = 1f});
                
                // temp code for lab 05


                /*Unity.Physics.RaycastInput input = new Unity.Physics.RaycastInput
                {
                    Start = bulletSpawnPointLTW.Position,
                    End = bulletSpawnPointLTW.Forward * gun.ValueRO.range + bulletSpawnPointLTW.Position,
                    Filter = new Unity.Physics.CollisionFilter
                    {
                        BelongsTo = BulletBelongsTo.Value,
                        CollidesWith = BulletCollidesWith.Value,
                        GroupIndex = 0,
                    }
                };

                //    Debug.Log($"{input.Start},{input.End},{ltw.ValueRO.Forward},{transform.ValueRO.Forward()},{bulletSpawnPointLocalTransform.Forward()}");
                //    Debug.Log($"{bulletSpawnPointLTW.Forward}");
                Unity.Physics.RaycastHit hit = new Unity.Physics.RaycastHit();
                if (physicsWorld.CollisionWorld.CastRay(input, out hit))
                {
                    Debug.Log($"hit {hit.Entity}");
                    //damage entity here
                    if (SystemAPI.HasComponent<EntityStatus>(hit.Entity))
                    {
                        //Damage the entity
                        var status = SystemAPI.GetComponent<EntityStatus>(hit.Entity);
                        if (status.currentHP > 0)
                        {
                            status.currentHP = math.clamp(status.currentHP - gun.ValueRO.damage, 0, status.maxHP);
                            SystemAPI.SetComponent(hit.Entity, status);
                        }
                    }
                }
                else
                {
                    Entity e = state.EntityManager.CreateEntity(new ComponentType[] {
                        typeof(EntityPrefabSpawnRequest),
                        typeof(TriggerECSVFXEffect),
                        typeof(EntityDestructionRequest)
                    });
                    var a = new EntityPrefabSpawnRequest
                    {
                        entity = BulletPrefab.Prefab,
                        spawnAmount = 1,
                        setLocalTransform = true,
                        localTransform = new LocalTransform
                        {
                            Position = input.Start,
                            Rotation = BulletPrefab.useLocalTransform ? math.mul(bulletSpawnPointLTW.Rotation, BulletPrefab.localTransform.Rotation)
                            : math.mul(bulletSpawnPointLTW.Rotation, BulletPrefab.localTransform.Rotation),
                            Scale = transform.ValueRO.Scale
                        }
                    };
                    state.EntityManager.SetComponentData(e, a);
                    state.EntityManager.SetComponentData(e, new TriggerECSVFXEffect
                    {
                        AudioSourceId = 0,
                        ParticleSystemId = 0,
                        position = a.localTransform.Position,
                        rotation = a.localTransform.Rotation
                    });
                    state.EntityManager.SetComponentData(e, new EntityDestructionRequest
                    {
                        entity = e,
                        destroyAfterXSeconds = 1
                    });
                    *//* e = state.EntityManager.CreateEntity(new ComponentType[] { typeof(EntityPrefabSpawnData) });
                     state.EntityManager.SetComponentData(e, new EntityPrefabSpawnData
                     {
                         entity = BulletPrefab.Prefab,
                         spawnAmount = 1,
                         setLocalTransform = true,
                         localTransform = new LocalTransform
                         {
                             Position = input.End,
                             Rotation = BulletPrefab.useSpawnPosition ? math.mul(bulletSpawnPointLTW.Rotation, BulletPrefab.spawnRotation)
                             : math.mul(bulletSpawnPointLTW.Rotation, BulletPrefab.spawnRotation),
                             Scale = transform.ValueRO.Scale
                         }
                     });
     *//*
                }*/
                gun.ValueRW.bulletsLeft--;
                gun.ValueRW.timeLeftBetweenShooting = gun.ValueRO.timeBetweenShooting;
            }
            else
            {
                if (gun.ValueRO.timeLeftBetweenShooting <= 0)
                    gun.ValueRW.readyToShoot = true;

            }
        }

        void StartReload(RefRW<GunComponentData> gun)
        {
            gun.ValueRW.reloading = true;
            gun.ValueRW.reloadTimeLeft = gun.ValueRW.reloadTime;
        }
        void FinishReload(RefRW<GunComponentData> gun)
        {
            if (gun.ValueRW.reloading && gun.ValueRO.reloadTimeLeft <= 0)
            {
                gun.ValueRW.reloading = false;
                gun.ValueRW.bulletsLeft = gun.ValueRO.magizineSize;
            }
        }
    }
}