using System.Collections;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace UnityNexus.EntityRelated
{
    public class EntityPrefabComponent : MonoBehaviour
    {
        public bool canNewEntityKeepEntityPrefabData;
        public int id;
        public int spawnAmount;
        public Transform entityToUseAsPrefab;
        public float delay;
        public bool useSpawnPosition;
        public float3 spawnPosition;
        public Quaternion spawnRotation;
        [Header("Entity Lifetime Settings")]
        public EntityDestructionRequest lifeTime;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
  
    public class EntityPrefabComponentBaker : Baker<EntityPrefabComponent>
    {
        public override void Bake(EntityPrefabComponent authoring)
        {
            Entity e = GetEntity(TransformUsageFlags.None);
            AddComponent(e, new EntityPrefabDataReference
            {
                id = authoring.id,
                Prefab = GetEntity(authoring.entityToUseAsPrefab,TransformUsageFlags.Dynamic),
                canNewEntityKeepEntityPrefabData = authoring.canNewEntityKeepEntityPrefabData,
                localTransform = new LocalTransform
                {
                    Position = authoring.spawnPosition,
                    Rotation = authoring.spawnRotation,
                    Scale = 1
                },
                setLocalTransform = authoring.useSpawnPosition,
                
            });
            AddComponent(e,new EntityDestructionRequest {
                destroyAfterXSeconds = authoring.lifeTime.destroyAfterXSeconds,
                changeEntityToNewlySpawnedEntity = authoring.lifeTime.changeEntityToNewlySpawnedEntity,
                entity = authoring.lifeTime.changeEntityToNewlySpawnedEntity ? Entity.Null : e
            });
            SetComponentEnabled<EntityDestructionRequest>(e, false);
        }
    }
} 