using Unity.Entities;
using UnityEngine;

namespace UnityNexus.Weapons
{
    public class GunComponent : MonoBehaviour
    {
        public Vector3 FirePoint;
        [SerializeField] internal GunComponentData data;
        private void OnValidate()
        {
            data.firefireOffsetForward = (FirePoint - transform.position).normalized;
            data.offsetDistance = FirePoint.magnitude;
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
    public class GunComponentBaker : Baker<GunComponent>
    {
        public override void Bake(GunComponent authoring)
        {
            authoring.data.firefireOffsetForward = authoring.FirePoint.normalized;
            authoring.data.offsetDistance = authoring.FirePoint.magnitude;
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), authoring.data);
        }
    }
}