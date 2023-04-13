#pragma warning disable CS0649
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using UnityEngine;

public class DOTSTerrainColliderComponent : MonoBehaviour
{
    [SerializeField] public PhysicsCategoryTags belongsTo;
    [SerializeField] public PhysicsCategoryTags collidesWith;


    [SerializeField] public int groupIndex;

 
}
public class DOTSTerrainColliderComponentBaker : Baker<DOTSTerrainColliderComponent>
{
    public override void Bake(DOTSTerrainColliderComponent authoring)
    {
        var terrain = authoring.GetComponent<Terrain>();

        if (terrain == null)
        {
            Debug.LogError("No terrain found!");
            return;
        }

        CollisionFilter collisionFilter = new CollisionFilter
        {
            BelongsTo = authoring.belongsTo.Value,
            CollidesWith = authoring.collidesWith.Value,
            GroupIndex = authoring.groupIndex
        };

        AddComponent(GetEntity(TransformUsageFlags.Dynamic),
            TerrainMapHelper.CreateTerrainCollider(terrain.terrainData, collisionFilter));
    }
}

// Note: Resolution was originally split into Width and Height
public static class TerrainMapHelper
{

    public static PhysicsCollider CreateTerrainCollider(TerrainData terrainData, CollisionFilter filter)
    {
        var physicsCollider = new PhysicsCollider();
        var size = new int2(terrainData.heightmapResolution, terrainData.heightmapResolution);
        var scale = terrainData.heightmapScale;

        var colliderHeights = new NativeArray<float>(terrainData.heightmapResolution * terrainData.heightmapResolution,
            Allocator.TempJob);

        var terrainHeights = terrainData.GetHeights(0, 0, terrainData.heightmapResolution,
            terrainData.heightmapResolution);

        #region SameHeight fall through bug work around
        // NOTE: Solves an issue with perfectly flat terrain failing to collide with objects.
        var heightmapScale = terrainData.size.z;
        var smallestOffset = 0.01f; // 1 cm offset, works with 2048 resolution terrain
        var heightmapValuePerMeterInWorldSpace = 0.5f / heightmapScale;
        var inHeightMapUnits = smallestOffset * heightmapValuePerMeterInWorldSpace;

        for (int j = 0; j < size.y; j++)
            for (int i = 0; i < size.x; i++)
            {
                var h = terrainHeights[i, j];

                var checkerboard = (i + j) % 2;

                colliderHeights[j + i * size.x] = h + inHeightMapUnits * checkerboard; // Note : assumes terrain neighboars are never 1 cm difference from eachother

            }

        // Note: Heightmap is between 0 and 0.5f (https://forum.unity.com/threads/terraindata-heightmaptexture-float-value-range.672421/)

        #endregion

        physicsCollider.Value = Unity.Physics.TerrainCollider.Create(colliderHeights, size, scale,
            Unity.Physics.TerrainCollider.CollisionMethod.Triangles, filter);




        colliderHeights.Dispose();

        return physicsCollider;
    }
}
