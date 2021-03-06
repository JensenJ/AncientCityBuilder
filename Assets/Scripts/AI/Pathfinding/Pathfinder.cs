﻿using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Burst;
using Unity.Entities;
using JUCL.Utilities;

public class Pathfinder : ComponentSystem
{
    //Const settings
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    protected override void OnUpdate()
    {
        //Setup grid
        int gridWidth = AIGrid.Instance.pathfindingGrid.GetWidth();
        int gridHeight = AIGrid.Instance.pathfindingGrid.GetHeight();
        List<FindPathJob> findPathJobList = new List<FindPathJob>();
        NativeList<JobHandle> jobHandleList = new NativeList<JobHandle>(Allocator.Temp);
        //For every entity with pathfinding data
        Entities.ForEach((Entity entity, DynamicBuffer<PathPosition> pathPositionBuffer, ref PathfindingComponentData pathfindingComponentData) =>
        {

            //Create pathfinding job
            FindPathJob findPathJob = new FindPathJob
            {
                pathNodeArray = GetPathNodeArray(),
                startPosition = pathfindingComponentData.startPosition,
                endPosition = pathfindingComponentData.endPosition,
                gridSize = new int2(gridWidth, gridHeight),
                //pathPositionBuffer = pathPositionBuffer,
                entity = entity,
                pathFollowComponentDataFromEntity = GetComponentDataFromEntity<PathFollowComponent>()
            };
            findPathJobList.Add(findPathJob);
            jobHandleList.Add(findPathJob.Schedule());
            PostUpdateCommands.RemoveComponent<PathfindingComponentData>(entity);
        });
        //Complete all jobs
        JobHandle.CompleteAll(jobHandleList);

        //Buffer path job for each entity
        foreach (FindPathJob job in findPathJobList)
        {
            new SetBufferPathJob
            {
                entity = job.entity,
                gridSize = job.gridSize,
                pathNodeArray = job.pathNodeArray,
                pathfindingComponentDataFromEntity = GetComponentDataFromEntity<PathfindingComponentData>(),
                pathFollowComponentDataFromEntity = GetComponentDataFromEntity<PathFollowComponent>(),
                pathPositionBufferFromEntity = GetBufferFromEntity<PathPosition>(),
            }.Run();
        }
        //Dispose of array
        jobHandleList.Dispose();
    }

    //Function to return array of pathnode
    private NativeArray<PathNode> GetPathNodeArray()
    {
        JUCLGrid<GridNode> grid = AIGrid.Instance.pathfindingGrid;
        int2 gridSize = new int2(grid.GetWidth(), grid.GetHeight());
        NativeArray<PathNode> pathNodeArray = new NativeArray<PathNode>(gridSize.x * gridSize.y, Allocator.TempJob);

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

                pathNode.isWalkable = grid.GetGridObject(x, y).IsWalkabke();
                pathNode.cameFromNodeIndex = -1;

                pathNodeArray[pathNode.index] = pathNode;
            }
        }
        return pathNodeArray;
    }

    //Burst compiled job for working out path position buffers
    [BurstCompile]
    private struct SetBufferPathJob : IJob
    {
        public int2 gridSize;
        [DeallocateOnJobCompletion]
        public NativeArray<PathNode> pathNodeArray;

        public Entity entity;

        public ComponentDataFromEntity<PathfindingComponentData> pathfindingComponentDataFromEntity;
        public ComponentDataFromEntity<PathFollowComponent> pathFollowComponentDataFromEntity;
        public BufferFromEntity<PathPosition> pathPositionBufferFromEntity;

        //Set path indexes
        public void Execute()
        {
            DynamicBuffer<PathPosition> pathPositionBuffer = pathPositionBufferFromEntity[entity];
            pathPositionBuffer.Clear();

            PathfindingComponentData pathfindingComponentData = pathfindingComponentDataFromEntity[entity];
            int endNodeIndex = CalculateIndex(pathfindingComponentData.endPosition.x, pathfindingComponentData.endPosition.y, gridSize.x);
            PathNode endNode = pathNodeArray[endNodeIndex];
            if(endNode.cameFromNodeIndex == -1)
            {
                //Did not find path
                pathFollowComponentDataFromEntity[entity] = new PathFollowComponent { pathIndex = -1 };
            }
            else
            {
                CalculatePath(pathNodeArray, endNode, pathPositionBuffer);
                pathFollowComponentDataFromEntity[entity] = new PathFollowComponent { pathIndex = pathPositionBuffer.Length -1};
            }
        }
    }

    //Job to find path
    [BurstCompile]
    private struct FindPathJob : IJob
    {
        public int2 startPosition;
        public int2 endPosition;
        public int2 gridSize;
        public NativeArray<PathNode> pathNodeArray;

        public Entity entity;
        [NativeDisableContainerSafetyRestriction]
        public ComponentDataFromEntity<PathFollowComponent> pathFollowComponentDataFromEntity;


        //Execute
        public void Execute()
        {
            for (int i = 0; i < pathNodeArray.Length; i++)
            {
                PathNode pathNode = pathNodeArray[i];
                pathNode.hCost = CalculateDistanceCost(new int2(pathNode.x, pathNode.y), endPosition);
                pathNode.cameFromNodeIndex = -1;
                pathNodeArray[i] = pathNode;
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

            //Array cleanup
            openList.Dispose();
            closedList.Dispose();
            neighbourOffsetArray.Dispose();
        }
    }

    private static void CalculatePath(NativeArray<PathNode> pathNodeArray, PathNode endNode, DynamicBuffer<PathPosition> pathPositionBuffer)
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
    private static bool IsPositionInsideGrid(int2 gridPos, int2 gridSize)
    {
        return
            gridPos.x >= 0 &&
            gridPos.y >= 0 &&
            gridPos.x < gridSize.x &&
            gridPos.y < gridSize.y;
    }

    //Function to calculate index given a position
    private static int CalculateIndex(int x, int y, int gridWidth)
    {
        return x + y * gridWidth;
    }

    //Calculates distance cost between positions
    private static int CalculateDistanceCost(int2 startPosition, int2 endPosition)
    {
        int xDistance = math.abs(startPosition.x - endPosition.x);
        int yDistance = math.abs(startPosition.y - endPosition.y);
        int remaining = math.abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * math.min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    //Finds the lowest FCost
    private static int GetLowestFCostNodeIndex(NativeList<int> openList, NativeArray<PathNode> pathNodeArray)
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