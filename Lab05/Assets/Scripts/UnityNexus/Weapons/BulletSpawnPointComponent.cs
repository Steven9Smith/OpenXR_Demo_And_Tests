using System.Collections;
using Unity.Entities;
using UnityEngine;

namespace UnityNexus.Weapons
{
    public class BulletSpawnPointComponent : MonoBehaviour
    {
        public Transform spawnPoint;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
    public struct BulletSpawnPointData : IComponentData
    {
        public Entity Value;
    }
    public class BulletSpawnPointComponentnBaker : Baker<BulletSpawnPointComponent>
    {
        public override void Bake(BulletSpawnPointComponent authoring)
        {
            AddComponent(GetEntity(TransformUsageFlags.None),new BulletSpawnPointData{ Value = GetEntity(authoring.spawnPoint, TransformUsageFlags.Dynamic)});
        }
    }
}