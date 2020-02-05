using Unity.Entities;
using Unity.Mathematics;
public struct PathfindingComponentData : IComponentData
{
    public int2 startPosition;
    public int2 endPosition;
}
