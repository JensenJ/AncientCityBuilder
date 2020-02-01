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

public class EntityTesting : MonoBehaviour
{
    [SerializeField] private Mesh mesh;
    [SerializeField] private Material material;
    [SerializeField] private bool useJobs;

    private void Start()
    {
        EntityManager entityManager = World.Active.EntityManager;
        EntityArchetype entityArchetype = entityManager.CreateArchetype(
            typeof(Translation),
            typeof(RenderMesh),
            typeof(LocalToWorld)
        );

        NativeArray<Entity> entityArray = new NativeArray<Entity>(1, Allocator.Temp);
        entityManager.CreateEntity(entityArchetype, entityArray);

        for (int i = 0; i < entityArray.Length; i++)
        {
            Entity entity = entityArray[i];

            //Set position
            entityManager.SetComponentData(entity, new Translation { 
                Value = new float3(
                    UnityEngine.Random.Range(10, 20),
                    UnityEngine.Random.Range(10, 20),
                    UnityEngine.Random.Range(10, 20)
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


    //Job system testing
    void Update()
    {
        float startTime = Time.realtimeSinceStartup;
        if (useJobs)
        {
            NativeList<JobHandle> jobHandleList = new NativeList<JobHandle>(Allocator.Temp);
            for (int i = 0; i < 10; i++)
            {
                JobHandle handle = TestTask();
                jobHandleList.Add(handle);
            }
            JobHandle.CompleteAll(jobHandleList);
            jobHandleList.Dispose();
        }
        else
        {
            for (int i = 0; i < 10; i++)
            {
                TestTaskNormal();
            }
        }
        Debug.Log(((Time.realtimeSinceStartup - startTime) * 1000f) + "ms");
    }

    private JobHandle TestTask()
    {
        TestTaskJob job = new TestTaskJob();
        return job.Schedule();
    }

    private void TestTaskNormal()
    {
        float value = 0f;
        for (int i = 0; i < 50000; i++)
        {
            value = math.exp10(math.sqrt(value));
        }
    }
}

//TestTaskJob, represents tough calculation
[BurstCompile]
public struct TestTaskJob : IJob 
{
    public void Execute()
    {
        float value = 0f;
        for (int i = 0; i < 50000; i++)
        {
            value = math.exp10(math.sqrt(value));
        }
    }
}
