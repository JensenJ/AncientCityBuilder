using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Jobs;
using Unity.Burst;

public class EntitySpawnManager : MonoBehaviour
{
    private EntityManager entityManager;
    [SerializeField] private EntityData[] entityTypes;

    public enum EntityArchetypes {
        TestAI
    }

    // Start is called before the first frame update
    void Start()
    {
        entityManager = World.Active.EntityManager;
        GenerateEntityArchetypes();

        SpawnEntity(entityTypes[0]);
    }

    //Function to generate entity archetypes from enum
    private void GenerateEntityArchetypes()
    {
        for (int i = 0; i < entityTypes.Length; i++)
        {
            //Get current data
            EntityData data = entityTypes[i];
            //If testAI archetype
            if(data.entityArchetypeEnum == EntityArchetypes.TestAI)
            {
                //Create archtype with following components
                data.entityArchetype = entityManager.CreateArchetype(
                    typeof(Translation),
                    typeof(RenderMesh),
                    typeof(LocalToWorld)
                );
            }
            //Set data back to main array
            entityTypes[i] = data;
        }
    }

    public void SpawnEntity(EntityData entityData)
    {
        SpawnEntity(entityData, float3.zero);
    }

    public void SpawnEntity(EntityData entityData, float3 spawnPosition)
    {
        //Create entity
        Entity entity = entityManager.CreateEntity(entityData.entityArchetype) ;

        //Set entity data
        entityManager.SetComponentData(entity, new Translation { Value = spawnPosition });
        entityManager.SetSharedComponentData(entity, new RenderMesh
        {
            mesh = entityData.entityMesh,
            material = entityData.entityMaterial,
            castShadows = entityData.entityShadowCastingMode
        });
    }
}

[System.Serializable]
public struct EntityData
{
    public string entityName;
    public EntitySpawnManager.EntityArchetypes entityArchetypeEnum;
    public Mesh entityMesh;
    public Material entityMaterial;
    public UnityEngine.Rendering.ShadowCastingMode entityShadowCastingMode;
    public EntityArchetype entityArchetype;
}