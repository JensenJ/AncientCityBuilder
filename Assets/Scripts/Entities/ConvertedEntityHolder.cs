using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

//Reference for converted entities from gameobjects
public class ConvertedEntityHolder : MonoBehaviour, IConvertGameObjectToEntity
{
    private Entity entity;
    private EntityManager entityManager;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        this.entity = entity;
        this.entityManager = dstManager;
    }

    //Getters
    public Entity GetEntity()
    {
        return entity;
    }

    public EntityManager GetEntityManager()
    {
        return entityManager;
    }
}
