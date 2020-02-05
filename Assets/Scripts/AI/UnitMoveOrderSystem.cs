using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class UnitMoveOrderSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    Entities.ForEach((Entity entity, ref Translation translation) =>
        //    {
        //        EntityManager.AddComponentData(entity, new PathfindingComponentData
        //        {
        //            startPosition = new Unity.Mathematics.int2(0, 0),
        //            endPosition = new Unity.Mathematics.int2(4, 0)
        //        }); 
        //    });
        //}
    }
}
