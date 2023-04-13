using UnitNexus.Animation;
using Unity.AI.Navigation;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.AI;
using UnityNexus.AI_Related.NavMesh;
using UnityNexus.Physics;

namespace UnityNexus.EntityRelated
{
    public class PrefabInstantiationComponent : MonoBehaviour
    {
        public GameObject Prefab;
    }
    public class PrefabInstantiationComponentBaker : Baker<PrefabInstantiationComponent>
    {
        public override void Bake(PrefabInstantiationComponent authoring)
        {
            Debug.Log($"Instantiating {authoring.name}...");
            Entity e = GetEntity(TransformUsageFlags.Dynamic);
            AddComponentObject(e, new PrefabGO { Prefab = authoring.Prefab });
            AddComponent(e, new PrefabGOTag { });
            AddComponentObject(e, new TransformGO { transform = authoring.Prefab.transform });
            AddComponent(e, new TransformGOTag { });
            AddComponent(e, new PrefabInitializeRequest { });
        }
    }
    // this tag allows you to test if PrefabGO exists in burst systems
    public struct PrefabGOTag : IComponentData { }
    public class PrefabGO : IComponentData { public GameObject Prefab; }

    // this tag allows you to test if TransformGO exists in burst systems
    public struct TransformGOTag : IComponentData { }
    public class TransformGO : IComponentData { public Transform transform; }
    // add this to initialize an entity with a PrefabGO component
    public struct PrefabInitializeRequest : IComponentData { };
    public struct TransformGOFollowEntity : IComponentData { public bool useLocalTransform; };
    public struct EntityFollowTransformGO : IComponentData {  };
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct PrefabInstantiationSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecbb = GetBeginSimulationEntityCommandBuffer(ref state);
            var ecbe = GetEndSimulationEntityCommandBuffer(ref state);

            //handles prefab setup step A:
            //Instantaes the prefab and add a TransformGO IComponentData to the entity
            foreach (var (prefabGO, transformGO,entityLTW,tag, entity) 
                in SystemAPI.Query<PrefabGO, TransformGO,RefRW<LocalToWorld>,RefRW<PrefabInitializeRequest>>().WithEntityAccess())
            {
                GameObject go = null;
                if (prefabGO.Prefab != null)
                {
                    go = GameObject.Instantiate(prefabGO.Prefab,
                    entityLTW.ValueRW.Position, entityLTW.ValueRW.Rotation);
                }
                else
                {
                    Debug.LogWarning("Detected Null Prefab during Instantiation...creating new one...");
                    go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    //Remove default components so only the transfor is left
                    go.RemoveComponent<MeshFilter>();
                    go.RemoveComponent<MeshRenderer>();
                    go.RemoveComponent<BoxCollider>();
                    go.name = state.EntityManager.GetName(entity);
                }
                transformGO.transform = go.transform;
            }
            // Performs Prefab Instantiation Setup
            foreach (var (prefabGO, transformGO,tag, entity) in SystemAPI.Query<PrefabGO, TransformGO, RefRW<PrefabInitializeRequest>>().WithEntityAccess())
            {
                if (transformGO.transform == null)
                {
                    Debug.LogError($"Failed to setup prefab instantiation correctly for {prefabGO.Prefab.name}, and {state.EntityManager.GetName(entity)}. Deleting Entity as it will cause problems...");
                }
               
                GameObject go = transformGO.transform.gameObject;
                // Handle Animator Setup
                if (SystemAPI.ManagedAPI.HasComponent<ECSAnimatorPrefabSetup>(entity))
                {
                    int i = 0;
                    var animator = go.GetComponent<Animator>();
                    if (animator == null) Debug.LogError("Failed to find animator in prefab!");
                    else
                    {
                        var animatorRef = SystemAPI.ManagedAPI.GetComponent<ECSAnimatorPrefabSetup>(entity);
                        ecbb.AddComponent(entity, new AnimatorGO { animator = animator });

                        if (animatorRef.animatorBoolParameters.Length > 0)
                        {
                            var buffer = ecbb.AddBuffer<AnimatorBoolParameter>(entity);
                            for (i = 0; i < animatorRef.animatorBoolParameters.Length; i++)
                                buffer.Add(new AnimatorBoolParameter
                                {
                                    NameId = Animator.StringToHash(animatorRef.animatorBoolParameters[i].Name),
                                    Value = animatorRef.animatorBoolParameters[i].Value
                                });
                        }
                        if (animatorRef.animatorTriggerParameters.Length > 0)
                        {
                            var buffer = ecbb.AddBuffer<AnimatorTriggerParameter>(entity);
                            for (i = 0; i < animatorRef.animatorTriggerParameters.Length; i++)
                                buffer.Add(new AnimatorTriggerParameter
                                {
                                    NameId = Animator.StringToHash(animatorRef.animatorTriggerParameters[i].Name),
                                    Value = animatorRef.animatorTriggerParameters[i].Value
                                });
                        }
                        if (animatorRef.animatorFloatParameters.Length > 0)
                        {
                            var buffer = ecbb.AddBuffer<AnimatorFloatParameter>(entity);
                            for (i = 0; i < animatorRef.animatorFloatParameters.Length; i++)
                                buffer.Add(new AnimatorFloatParameter
                                {
                                    NameId = Animator.StringToHash(animatorRef.animatorFloatParameters[i].Name),
                                    Value = animatorRef.animatorFloatParameters[i].Value
                                });
                        }
                        if (animatorRef.animatorIntParameters.Length > 0)
                        {
                            var buffer = ecbb.AddBuffer<AnimatorIntParameter>(entity);
                            for (i = 0; i < animatorRef.animatorIntParameters.Length; i++)
                                buffer.Add(new AnimatorIntParameter
                                {
                                    NameId = Animator.StringToHash(animatorRef.animatorIntParameters[i].Name),
                                    Value = animatorRef.animatorIntParameters[i].Value
                                });
                        }
                    }
                    ecbb.RemoveComponent<ECSAnimatorPrefabSetup>(entity);
                }

                //Handle ECSNavMeshSurface
                if (SystemAPI.HasComponent<ECSNavMeshSurfacePrefabSetup>(entity))
                {
                    bool hasComponent = go.TryGetComponent<NavMeshSurface>(out var nms);
                    //use the prefab NavMeshSurface
                    if (nms == null) Debug.LogError("NavMeshSurface is missing from the prefab!");
                    else ecbb.AddComponent(entity, new ECSNavMeshSurface { navMeshSurface = nms });
                    ecbb.RemoveComponent<ECSNavMeshSurfacePrefabSetup>(entity);
                }

                //handle ECSNavMeshAgent
                if (SystemAPI.HasComponent<ECSNavMeshAgentPrefabSetup>(entity))
                {
                    bool hasComponent = go.TryGetComponent<NavMeshAgent>(out var nms);

                    //use the prefab NavMeshAgent
                    if (nms == null) Debug.LogError("NavMeshAgent is missing from the prefab!");
                    else ecbb.AddComponent(entity, new ECSNavMeshAgent { navMeshAgent = nms });
                    ecbb.RemoveComponent<ECSNavMeshAgentPrefabSetup>(entity);



                    /*      var nma = go.GetComponent<NavMeshAgent>();
                            if (nma == null) Debug.LogError("NavMeshAgent is missing from the prefab!");
                            else ecb.AddComponent(entity, new ECSNavMeshAgent { navMeshAgent = nma });
                            ecb.RemoveComponent<ECSNavMeshAgentPrefabSetup>(entity);*/

                }
                //handle ECSNavMeshObstacle
                if (SystemAPI.HasComponent<ECSNavMeshObstaclePrefabSetup>(entity))
                {
                    bool hasComponent = go.TryGetComponent<NavMeshObstacle>(out var nms);
                    //use the prefab NavMeshObstacle
                    if (!hasComponent) Debug.LogError("NavMeshObstacle is missing from the prefab!");
                    else ecbb.AddComponent(entity, new ECSNavMeshObstacle { navMeshObstacle = nms });
                    ecbb.RemoveComponent<ECSNavMeshObstaclePrefabSetup>(entity);

                    /*   var nma = go.GetComponent<NavMeshObstacle>();
                        if (nma == null) Debug.LogError("NavMeshObstacle is missing from the prefab!");
                        else ecb.AddComponent(entity, new ECSNavMeshObstacle { navMeshObstacle = nma });
                        ecb.RemoveComponent<ECSNavMeshObstaclePrefabSetup>(entity);*/

                }
                //handle ECSNavOffMeshLink
                if (SystemAPI.ManagedAPI.HasComponent<ECSNavOffMeshLinkPrefabSetup>(entity))
                {
                    var setup = SystemAPI.ManagedAPI.GetComponent<ECSNavOffMeshLinkPrefabSetup>(entity);
                    var link = go.GetComponent<OffMeshLink>();
                    if (setup.start != Entity.Null)
                    {
                        if (setup.startHasPrefabInstantiateComponent)
                            link.startTransform = SystemAPI.ManagedAPI.GetComponent<TransformGO>(setup.start).transform;
                        else //TODO: should i create a new transform? i don't know where to put it?
                            Debug.LogWarning("This feature for OfflinkMesh isn't setup yet");
                    }
                    else Debug.LogError("Start Entity doesn't exist so setup annt continue");
                    if (setup.end != Entity.Null)
                    {
                        if (setup.endHasPrefabInstantiateComponent)
                            link.endTransform = SystemAPI.ManagedAPI.GetComponent<TransformGO>(setup.end).transform;
                        else //TODO: should i create a new transform? i don't know where to put it?
                            Debug.LogWarning("This feature for OfflinkMesh isn't setup yet");
                    }
                    else Debug.LogError("end Entity doesn't exist so setup annt continue");

                    ecbb.AddComponent(entity, new ECSNavOffMeshLink { navOffMeshLink = link });

                    if (setup.ANavMeshSurfaceEntity == Entity.Null)
                        Debug.LogWarning("The NavMeshSurfave entity is null. This is needed to update the nav mesh properly");
                    else
                        ecbb.AddComponent<ECSNavMeshSurfaceRebuild>(setup.ANavMeshSurfaceEntity);

                    ecbb.RemoveComponent<ECSNavOffMeshLinkPrefabSetup>(entity);
                }
                // enable impulse component
                if (SystemAPI.HasComponent<EntityPrefabSpawnImpulseRequest>(entity))
                    SystemAPI.SetComponentEnabled<EntityPrefabSpawnImpulseRequest>(entity, true);
                



                ecbb.RemoveComponent<PrefabInitializeRequest>(entity);
                
            }

            //TODO: make the TransformmGO entity create an array of LocalTransform/LocalToWorld
            // that keeps track of all the children and make it accessable. This should increase
            // performance and make this system more burstable
            foreach (var (prefabChildEntityLink,entity) in SystemAPI.Query<RefRW<PrefabChildEntityLink>>().WithEntityAccess())
            {
                var transform = SystemAPI.ManagedAPI.GetComponent<TransformGO>(prefabChildEntityLink.ValueRO.EntityThatHasPrefab).transform;
                if (transform == null) Debug.LogError("Failed to get transfrom prefab from entity!");
                else
                {
                    var child = transform.SearchForChild(prefabChildEntityLink.ValueRO.PrefabChilName.ToString());
                    if (child != null)
                    {
                        ecbe.AddComponent(entity, new TransformGO { transform = child });
                        ecbe.AddComponent(entity, new EntityFollowTransformGO { });
                    }
                    else Debug.LogError($"Failed to find child {prefabChildEntityLink.ValueRO.PrefabChilName.ToString()} in transform {transform.name}");

                    ecbe.RemoveComponent<PrefabChildEntityLink>(entity);
                }
            }

            //handle GameObject transforms following entities
            foreach (var ( transformGO, ltw,localTransform,tag) in SystemAPI.Query<TransformGO,
                RefRO<LocalToWorld>,RefRO<LocalTransform>,RefRO<TransformGOFollowEntity>>()
                .WithNone<EntityFollowTransformGO>())
            {
                if (!tag.ValueRO.useLocalTransform)
                {
                    if (float.IsNaN(localTransform.ValueRO.Position.x))
                        Debug.LogError("DETECTED NULL LOCAL TRANSFORM");
                    else
                    {
                        transformGO.transform.position = localTransform.ValueRO.Position;
                        transformGO.transform.rotation = localTransform.ValueRO.Rotation;
                    }
                }
                else
                {
                    transformGO.transform.position = ltw.ValueRO.Position;
                    transformGO.transform.rotation = ltw.ValueRO.Rotation;
                }
            }
           
            // have a entity follow a GamObject
            foreach (var (transformGO, ltw, localTransform, tag) in SystemAPI.Query<TransformGO,
                RefRW<LocalToWorld>, RefRW<LocalTransform>, RefRO<EntityFollowTransformGO>>()
                .WithNone<TransformGOFollowEntity>())
            {
                localTransform.ValueRW.Position = transformGO.transform.position;
                localTransform.ValueRW.Rotation = transformGO.transform.rotation;
            }
       
        
        }
        [BurstCompile]
        private EntityCommandBuffer GetBeginSimulationEntityCommandBuffer(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            return ecb;
        }
        [BurstCompile]
        private EntityCommandBuffer GetEndSimulationEntityCommandBuffer(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            return ecb;
        }
    }
}