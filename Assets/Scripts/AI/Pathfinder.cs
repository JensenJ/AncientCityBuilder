using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;

public class Pathfinder : MonoBehaviour
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    //Function to find path, given two positions
    private void FindPath(int2 startPos, int2 endPos)
    {
        int2 gridSize = new int2(4, 4);
        NativeArray<PathNode> pathNodeArray = new NativeArray<PathNode>(gridSize.x * gridSize.y, Allocator.Temp);

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                PathNode pathNode = new PathNode();
                pathNode.x = x;
                pathNode.y = y;
                pathNode.index = CalculateIndex(x, y, gridSize.x);

                pathNode.gCost = int.MaxValue;
                pathNode.hCost = CalculateDistanceCost(new int2(x, y), endPos);
                pathNode.CalculateFCost();

                pathNode.isWalkable = true;
                pathNode.cameFromNodeIndex = -1;
                pathNodeArray[pathNode.index] = pathNode;
            }
        }

        //Neighbour positions
        NativeArray<int2> neighbourOffsetArray = new NativeArray<int2>(new int2[]{
            new int2(-1, 0), //Left
            new int2(+1, 0), //Right
            new int2(0, +1), //Up
            new int2(0, -1), //Down
            new int2(-1, -1), //Left Down
            new int2(-1, +1), //Left Up
            new int2(+1, -1), //Right Down
            new int2(+1, +1) //Right up
        }, Allocator.Temp); 

        int endNodeIndex = CalculateIndex(endPos.x, endPos.y, gridSize.x);
        PathNode startNode = pathNodeArray[CalculateIndex(startPos.x, startPos.y, gridSize.x)];
        startNode.gCost = 0;
        startNode.CalculateFCost();
        pathNodeArray[startNode.index] = startNode;

        NativeList<int> openList = new NativeList<int>(Allocator.Temp);
        NativeList<int> closedList = new NativeList<int>(Allocator.Temp);

        openList.Add(startNode.index);
        while(openList.Length > 0)
        {
            int currentNodeIndex = GetLowestFCostNodeIndex(openList, pathNodeArray);
            PathNode currentNode = pathNodeArray[currentNodeIndex];

            //Has the destination been reached?
            if(currentNodeIndex == endNodeIndex)
            {
                break;
            }

            //Remove node from open list
            for (int i = 0; i < openList.Length; i++)
            {
                if(openList[i] == currentNodeIndex)
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
                if(tentativeGCost < neighbourNode.gCost)
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

        PathNode endNode = pathNodeArray[endNodeIndex];
        if(endNode.cameFromNodeIndex == -1)
        {
            //Did not find path
        }
        else
        {
            //found path
            NativeList<int2> path = CalculatePath(pathNodeArray, endNode);
            path.Dispose();
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
        int yDistance = math.abs(startPosition.y - startPosition.y);
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
            if(testPathNode.fCost < lowestCostPathNode.fCost)
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
