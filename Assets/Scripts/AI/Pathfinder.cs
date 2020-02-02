using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using Unity.Entities;

public class Pathfinder : ComponentSystem
{
    //Const settings
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    private int gridWidth = 40;
    private int gridHeight = 40;

    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, DynamicBuffer<PathPosition> pathPositionBuffer, ref PathfindingComponentData pathfindingComponentData) =>
        {
            Debug.Log("Finding Path");

            FindPathJob findPathJob = new FindPathJob
            {
                startPosition = pathfindingComponentData.startPosition,
                endPosition = pathfindingComponentData.endPosition,
                gridSize = new int2(gridWidth, gridHeight),
                pathPositionBuffer = pathPositionBuffer,
                entity = entity,
                pathFollowComponentDataFromEntity = GetComponentDataFromEntity<PathFollowComponent>()
            };
            findPathJob.Run();
            PostUpdateCommands.RemoveComponent<PathfindingComponentData>(entity);
        });
    }

    //Job to find path
    [BurstCompile]
    private struct FindPathJob : IJob
    {
        public int2 startPosition;
        public int2 endPosition;
        public int2 gridSize;

        public Entity entity;
        public ComponentDataFromEntity<PathFollowComponent> pathFollowComponentDataFromEntity;
        public DynamicBuffer<PathPosition> pathPositionBuffer;

        public void Execute()
        {
            NativeArray<PathNode> pathNodeArray = new NativeArray<PathNode>(gridSize.x * gridSize.y, Allocator.Temp);

            //Grid initialisation
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    PathNode pathNode = new PathNode();
                    pathNode.x = x;
                    pathNode.y = y;
                    pathNode.index = CalculateIndex(x, y, gridSize.x);

                    pathNode.gCost = int.MaxValue;
                    pathNode.hCost = CalculateDistanceCost(new int2(x, y), endPosition);
                    pathNode.CalculateFCost();

                    pathNode.isWalkable = true;
                    pathNode.cameFromNodeIndex = -1;
                    pathNodeArray[pathNode.index] = pathNode;
                }
            }

            //Neighbour positions
            NativeArray<int2> neighbourOffsetArray = new NativeArray<int2>(8, Allocator.Temp);

            neighbourOffsetArray[0] = new int2(-1, 0); //Left
            neighbourOffsetArray[1] = new int2(+1, 0); //Right
            neighbourOffsetArray[2] = new int2(0, +1); //Up
            neighbourOffsetArray[3] = new int2(0, -1); //Down
            neighbourOffsetArray[4] = new int2(-1, -1); //Left Down
            neighbourOffsetArray[5] = new int2(-1, +1); //Left Up
            neighbourOffsetArray[6] = new int2(+1, -1); //Right Down
            neighbourOffsetArray[7] = new int2(+1, +1); //Right up

            int endNodeIndex = CalculateIndex(endPosition.x, endPosition.y, gridSize.x);
            PathNode startNode = pathNodeArray[CalculateIndex(startPosition.x, startPosition.y, gridSize.x)];
            startNode.gCost = 0;
            startNode.CalculateFCost();
            pathNodeArray[startNode.index] = startNode;

            NativeList<int> openList = new NativeList<int>(Allocator.Temp);
            NativeList<int> closedList = new NativeList<int>(Allocator.Temp);

            openList.Add(startNode.index);
            while (openList.Length > 0)
            {
                int currentNodeIndex = GetLowestFCostNodeIndex(openList, pathNodeArray);
                PathNode currentNode = pathNodeArray[currentNodeIndex];

                //Has the destination been reached?
                if (currentNodeIndex == endNodeIndex)
                {
                    break;
                }

                //Remove node from open list
                for (int i = 0; i < openList.Length; i++)
                {
                    if (openList[i] == currentNodeIndex)
                    {
                        openList.RemoveAtSwapBack(i);
                        break;
                    }
                }

                closedList.Add(currentNodeIndex);
                //Cycle through neighbours
                for (int i = 0; i < neighbourOffsetArray.Length; i++)
                {
                    int2 neighbourOffset = neighbourOffsetArray[i];
                    int2 neighbourPos = new int2(currentNode.x + neighbourOffset.x, currentNode.y + neighbourOffset.y);

                    if (!IsPositionInsideGrid(neighbourPos, gridSize))
                    {
                        //Neighbour not in valid pos
                        continue;
                    }

                    int neighbourNodeIndex = CalculateIndex(neighbourPos.x, neighbourPos.y, gridSize.x);

                    if (closedList.Contains(neighbourNodeIndex))
                    {
                        //Already searched this node
                        continue;
                    }

                    PathNode neighbourNode = pathNodeArray[neighbourNodeIndex];
                    if (!neighbourNode.isWalkable)
                    {
                        //Not walkable
                        continue;
                    }

                    int2 currentNodePosition = new int2(currentNode.x, currentNode.y);
                    int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNodePosition, neighbourPos);
                    if (tentativeGCost < neighbourNode.gCost)
                    {
                        neighbourNode.cameFromNodeIndex = currentNodeIndex;
                        neighbourNode.gCost = tentativeGCost;
                        neighbourNode.CalculateFCost();
                        pathNodeArray[neighbourNodeIndex] = neighbourNode;

                        if (!openList.Contains(neighbourNode.index))
                        {
                            openList.Add(neighbourNode.index);
                        }
                    }
                }
            }

            pathPositionBuffer.Clear();
            PathNode endNode = pathNodeArray[endNodeIndex];
            if (endNode.cameFromNodeIndex == -1)
            {
                //Did not find path
                pathFollowComponentDataFromEntity[entity] = new PathFollowComponent { pathIndex = -1 };
            }
            else
            {
                //found path
                CalculatePath(pathNodeArray, endNode, pathPositionBuffer);
                pathFollowComponentDataFromEntity[entity] = new PathFollowComponent { pathIndex = pathPositionBuffer.Length -1 };
            }

            //Array cleanup
            pathNodeArray.Dispose();
            openList.Dispose();
            closedList.Dispose();
            neighbourOffsetArray.Dispose();
        }

        //Calculates the path
        private NativeList<int2> CalculatePath(NativeArray<PathNode> pathNodeArray, PathNode endNode)
        {
            if (endNode.cameFromNodeIndex == -1)
            {
                //Couldn't find path
                return new NativeList<int2>(Allocator.Temp);
            }
            else
            {
                NativeList<int2> path = new NativeList<int2>(Allocator.Temp);
                path.Add(new int2(endNode.x, endNode.y));

                PathNode currentNode = endNode;
                //Trace back path
                while (currentNode.cameFromNodeIndex != -1)
                {
                    PathNode cameFromNode = pathNodeArray[currentNode.cameFromNodeIndex];
                    path.Add(new int2(cameFromNode.x, cameFromNode.y));
                    currentNode = cameFromNode;
                }

                return path;
            }
        }

        private void CalculatePath(NativeArray<PathNode> pathNodeArray, PathNode endNode, DynamicBuffer<PathPosition> pathPositionBuffer)
        {
            if (endNode.cameFromNodeIndex == -1)
            {
                //Couldn't find path
            }
            else
            {
                pathPositionBuffer.Add(new PathPosition { position = new int2(endNode.x, endNode.y) });

                PathNode currentNode = endNode;
                //Trace back path
                while (currentNode.cameFromNodeIndex != -1)
                {
                    PathNode cameFromNode = pathNodeArray[currentNode.cameFromNodeIndex];
                    pathPositionBuffer.Add(new PathPosition { position = new int2(cameFromNode.x, cameFromNode.y) });
                    currentNode = cameFromNode;
                }
            }
        }

        //Is the position inside this grid
        private bool IsPositionInsideGrid(int2 gridPos, int2 gridSize)
        {
            return
                gridPos.x >= 0 &&
                gridPos.y >= 0 &&
                gridPos.x < gridSize.x &&
                gridPos.y < gridSize.y;
        }

        //Function to calculate index given a position
        private int CalculateIndex(int x, int y, int gridWidth)
        {
            return x + y * gridWidth;
        }

        //Calculates distance cost between positions
        private int CalculateDistanceCost(int2 startPosition, int2 endPosition)
        {
            int xDistance = math.abs(startPosition.x - endPosition.x);
            int yDistance = math.abs(startPosition.y - endPosition.y);
            int remaining = math.abs(xDistance - yDistance);
            return MOVE_DIAGONAL_COST * math.min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
        }

        //Finds the lowest FCost
        private int GetLowestFCostNodeIndex(NativeList<int> openList, NativeArray<PathNode> pathNodeArray)
        {
            PathNode lowestCostPathNode = pathNodeArray[openList[0]];
            for (int i = 0; i < openList.Length; i++)
            {
                PathNode testPathNode = pathNodeArray[openList[i]];
                if (testPathNode.fCost < lowestCostPathNode.fCost)
                {
                    lowestCostPathNode = testPathNode;
                }
            }
            return lowestCostPathNode.index;
        }

        //PathNode struct
        private struct PathNode
        {
            public int x;
            public int y;

            public int index;

            public int gCost;
            public int hCost;
            public int fCost;

            public bool isWalkable;

            public int cameFromNodeIndex;

            //Pathnode functions
            public void CalculateFCost()
            {
                fCost = gCost + hCost;
            }

            public void SetIsWalkable(bool value)
            {
                isWalkable = value;
            }
        }
    }
}