using Unity.Entities;

[GenerateAuthoringComponent]
public struct PathFollowComponent : IComponentData
{
    public int pathIndex;
}
