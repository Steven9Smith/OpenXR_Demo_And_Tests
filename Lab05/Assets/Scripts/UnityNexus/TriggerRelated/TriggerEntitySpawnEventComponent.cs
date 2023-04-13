using System.Collections;
using Unity.Entities;
using UnityEngine;
using UnityNexus.EntityRelated;
using UnityNexus.TriggerRelated;

namespace UnityNexus.TriggerRelated
{
    public class TriggerEntitySpawnEventComponent : MonoBehaviour
    {

        public bool spawnAtHit;
        public bool spawnOnTriggerEntity;
        public bool spawnOnHitEntity;
        public Transform EntityToSpawn;
        public TriggerEventStatusTracker triggerEventTracker;
        public EntityPrefabSpawnRequestDataForComponent spawnData;
    }
    public class TriggerEntitySpawnEventComponentBaker : Baker<TriggerEntitySpawnEventComponent>
    {
        public override void Bake(TriggerEntitySpawnEventComponent authoring)
        {
            // this is to prevent the end trigger from fireing when the game starts
            if(authoring.triggerEventTracker.executeOnEnd)
                authoring.triggerEventTracker.endJobAlreadyTriggered = true;


            Entity e = GetEntity(TransformUsageFlags.Dynamic);
            Entity ee = GetEntity(authoring.EntityToSpawn, TransformUsageFlags.Dynamic);
            AddComponent(e, new TriggerEntitySpawnEvent{ 
                spawnRequest = authoring.spawnData.ToEntityPrefabSpawnRequest(e,ee),
                spawnAtHit = authoring.spawnAtHit,
                spawnOnHitEntity = authoring.spawnOnHitEntity,
                spawnOnTriggerEntity = authoring.spawnOnTriggerEntity
            });
            AddComponent(e, authoring.triggerEventTracker);
            AddComponent(e, new IsECSTriggerEvent { });
            
        }
    }
}