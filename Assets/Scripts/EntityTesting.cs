using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Rendering;

public class EntityTesting : MonoBehaviour
{
    [SerializeField] private Mesh mesh;
    [SerializeField] private Material material;

    private void Start()
    {
        EntityManager entityManager = World.Active.EntityManager;
        EntityArchetype entityArchetype = entityManager.CreateArchetype(
            typeof(Translation),
            typeof(RenderMesh),
            typeof(LocalToWorld)
        );

        NativeArray<Entity> entityArray = new NativeArray<Entity>(100000, Allocator.Temp);
        entityManager.CreateEntity(entityArchetype, entityArray);

        for (int i = 0; i < entityArray.Length; i++)
        {
            Entity entity = entityArray[i];

            //Set position
            entityManager.SetComponentData(entity, new Translation { 
                Value = new float3(
                    UnityEngine.Random.Range(10, 200),
                    UnityEngine.Random.Range(10, 200),
                    UnityEngine.Random.Range(10, 200)
                ) 
            });

            entityManager.SetSharedComponentData(entity, new RenderMesh
            {
                mesh = mesh,
                material = material,
                castShadows = UnityEngine.Rendering.ShadowCastingMode.On
            });
        }

        entityArray.Dispose();
    }
}
