using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    [Header("图块与图块地图参考")] [Header("选项")] [SerializeField]
    private bool observeMovementPenalties = true; // 是否观察移动惩罚

    [Range(0, 20)] [SerializeField] private int pathMovementPenalty = 0;
    [Range(0, 20)] [SerializeField] private int defaultMovementPenalty = 0;

    private Grid grid;
    private Node startNode;
    private Node targetNode;
    private int gridWidth;
    private int gridHeight;
    private int originX;
    private int originY;

    private List<Node> openNodeList; // 开放列表，存储待评估的节点
    private HashSet<Node> closedNodeList; // 关闭列表，存储已评估的节点

    private bool pathFound;


    // 根据给定的场景名称为其从startGridPosition到endGridPosition构建一条路径，并将移动步骤添加到传入的npcMovementStepStack中。如果找到路径则返回true，否则返回false。
    public bool BuildPath(SceneName sceneName, Vector2Int startGridPosition, Vector2Int endGridPosition,
        Stack<NPCMovementStep> npcMovementStepStack)
    {
        pathFound = false;

        if (PopulateGridNodesFromGridPropertiesDictionary(sceneName, startGridPosition, endGridPosition))
        {
            if (FindShortestPath())
            {
                UpdatePathOnNPCMovementStepStack(sceneName, npcMovementStepStack);

                return true;
            }
        }

        return false;
    }

    //
    private void UpdatePathOnNPCMovementStepStack(SceneName sceneName, Stack<NPCMovementStep> npcMovementStepStack)
    {
        Node nextNode = targetNode;

        while (nextNode != null)
        {
            NPCMovementStep npcMovementStep = new NPCMovementStep();

            npcMovementStep.sceneName = sceneName;
            npcMovementStep.gridCoordinate =
                new Vector2Int(nextNode.gridPosition.x + originX, nextNode.gridPosition.y + originY);

            npcMovementStepStack.Push(npcMovementStep);

            nextNode = nextNode.parentNode;
        }
    }

    /// <summary>
    ///  Returns true if a path has been found
    /// </summary>
    private bool FindShortestPath()
    {
        // Add start node to open list
        openNodeList.Add(startNode);

        // Loop through open node list until empty
        while (openNodeList.Count > 0)
        {
            // Sort List
            openNodeList.Sort();

            //  current node = the node in the open list with the lowest fCost
            Node currentNode = openNodeList[0];
            openNodeList.RemoveAt(0);

            // add current node to the closed list
            closedNodeList.Add(currentNode);

            // if the current node = target node
            //      then finish

            if (currentNode == targetNode)
            {
                pathFound = true;
                break;
            }

            // evaluate fcost for each neighbour of the current node
            EvaluateCurrentNodeNeighbours(currentNode);
        }

        if (pathFound)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void EvaluateCurrentNodeNeighbours(Node currentNode)
    {
        Vector2Int currentNodeGridPosition = currentNode.gridPosition;

        Node validNeighbourNode;

        // Loop through all directions
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;

                validNeighbourNode =
                    GetValidNodeNeighbour(currentNodeGridPosition.x + i, currentNodeGridPosition.y + j);

                if (validNeighbourNode != null)
                {
                    // Calculate new gcost for neighbour
                    int newCostToNeighbour;

                    if (observeMovementPenalties)
                    {
                        newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, validNeighbourNode) +
                                             validNeighbourNode.movementPenalty;
                    }
                    else
                    {
                        newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, validNeighbourNode);
                    }

                    bool isValidNeighbourNodeInOpenList = openNodeList.Contains(validNeighbourNode);

                    if (newCostToNeighbour < validNeighbourNode.gCost || !isValidNeighbourNodeInOpenList)
                    {
                        validNeighbourNode.gCost = newCostToNeighbour;
                        validNeighbourNode.hCost = GetDistance(validNeighbourNode, targetNode);

                        validNeighbourNode.parentNode = currentNode;

                        if (!isValidNeighbourNodeInOpenList)
                        {
                            openNodeList.Add(validNeighbourNode);
                        }
                    }
                }
            }
        }
    }

    private int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
        int dstY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    private Node GetValidNodeNeighbour(int neighboutNodeXPosition, int neighbourNodeYPosition)
    {
        // If neighbour node position is beyond grid then return null
        if (neighboutNodeXPosition >= gridWidth || neighboutNodeXPosition < 0 || neighbourNodeYPosition >= gridHeight ||
            neighbourNodeYPosition < 0)
        {
            return null;
        }

        // if neighbour is an obstacle or neighbour is in the closed list then skip
        Node neighbourNode = grid.GetGridNode(neighboutNodeXPosition, neighbourNodeYPosition);

        if (neighbourNode.isObstacle || closedNodeList.Contains(neighbourNode))
        {
            return null;
        }
        else
        {
            return neighbourNode;
        }
    }

    private bool PopulateGridNodesFromGridPropertiesDictionary(SceneName sceneName, Vector2Int startGridPosition,
        Vector2Int endGridPosition)
    {
        // Get grid properties dictionary for the scene
        SceneSave sceneSave;

        if (GridPropertiesManager.Instance.GameObjectSave.sceneData.TryGetValue(sceneName.ToString(), out sceneSave))
        {
            // Get Dict grid property details
            if (sceneSave.gridPropertyDetailsDictionary != null)
            {
                // Get grid height and width
                if (GridPropertiesManager.Instance.GetGridDimensions(sceneName, out Vector2Int gridDimensions,
                        out Vector2Int gridOrigin))
                {
                    // Create nodes grid based on grid properties dictionary
                    grid = new Grid(gridDimensions.x, gridDimensions.y);
                    gridWidth = gridDimensions.x;
                    gridHeight = gridDimensions.y;
                    originX = gridOrigin.x;
                    originY = gridOrigin.y;

                    // Create openNodeList
                    openNodeList = new List<Node>();

                    // Create closed Node List
                    closedNodeList = new HashSet<Node>();
                }
                else
                {
                    return false;
                }

                // Populate start node
                startNode = grid.GetGridNode(startGridPosition.x - gridOrigin.x, startGridPosition.y - gridOrigin.y);

                // Populate target node
                targetNode = grid.GetGridNode(endGridPosition.x - gridOrigin.x, endGridPosition.y - gridOrigin.y);

                // populate obstacle and path info for grid
                for (int x = 0; x < gridDimensions.x; x++)
                {
                    for (int y = 0; y < gridDimensions.y; y++)
                    {
                        GridPropertyDetails gridPropertyDetails =
                            GridPropertiesManager.Instance.GetGridPropertyDetails(x + gridOrigin.x, y + gridOrigin.y,
                                sceneSave.gridPropertyDetailsDictionary);

                        if (gridPropertyDetails != null)
                        {
                            // If NPC obstacle
                            if (gridPropertyDetails.isNPCObstacle == true)
                            {
                                Node node = grid.GetGridNode(x, y);
                                node.isObstacle = true;
                            }
                            else if (gridPropertyDetails.isPath == true)
                            {
                                Node node = grid.GetGridNode(x, y);
                                node.movementPenalty = pathMovementPenalty;
                            }
                            else
                            {
                                Node node = grid.GetGridNode(x, y);
                                node.movementPenalty = defaultMovementPenalty;
                            }
                        }
                    }
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }

        return true;
    }
}