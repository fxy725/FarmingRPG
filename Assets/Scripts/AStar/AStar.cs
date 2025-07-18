using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    [Header("图块与图块地图参考")]
    [Header("选项")]
    [SerializeField]
    private bool observeMovementPenalties = true; // 是否观察移动惩罚

    [Range(0, 20)][SerializeField] private int pathMovementPenalty = 0;
    [Range(0, 20)][SerializeField] private int defaultMovementPenalty = 0;

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
    ///  如果找到路径则返回true
    /// </summary>
    private bool FindShortestPath()
    {
        // 将起始节点添加到开放列表
        openNodeList.Add(startNode);

        // 遍历开放列表直到空
        while (openNodeList.Count > 0)
        {
            // 排序列表
            openNodeList.Sort();

            //  当前节点 = 开放列表中fCost最低的节点
            Node currentNode = openNodeList[0];
            openNodeList.RemoveAt(0);

            // 将当前节点添加到关闭列表
            closedNodeList.Add(currentNode);

            // 如果当前节点 = 目标节点
            //      则完成

            if (currentNode == targetNode)
            {
                pathFound = true;
                break;
            }

            // 评估当前节点的每个邻居的fCost
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

        // 遍历所有方向
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
                    // 计算邻居的新gCost
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
        // 如果邻居节点位置超出网格则返回null
        if (neighboutNodeXPosition >= gridWidth || neighboutNodeXPosition < 0 || neighbourNodeYPosition >= gridHeight ||
            neighbourNodeYPosition < 0)
        {
            return null;
        }

        // 如果邻居是障碍物或邻居在关闭列表中则跳过
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
        // 获取场景的网格属性字典
        SceneSave sceneSave;

        if (GridPropertiesManager.Instance.GameObjectSave.sceneData.TryGetValue(sceneName.ToString(), out sceneSave))
        {
            // 获取网格属性细节字典
            if (sceneSave.gridPropertyDetailsDictionary != null)
            {
                // 获取网格高度和宽度
                if (GridPropertiesManager.Instance.GetGridDimensions(sceneName, out Vector2Int gridDimensions,
                        out Vector2Int gridOrigin))
                {
                    // 根据网格属性字典创建节点网格
                    grid = new Grid(gridDimensions.x, gridDimensions.y);
                    gridWidth = gridDimensions.x;
                    gridHeight = gridDimensions.y;
                    originX = gridOrigin.x;
                    originY = gridOrigin.y;

                    // 创建开放节点列表
                    openNodeList = new List<Node>();

                    // 创建关闭节点列表
                    closedNodeList = new HashSet<Node>();
                }
                else
                {
                    return false;
                }

                // 填充起始节点
                startNode = grid.GetGridNode(startGridPosition.x - gridOrigin.x, startGridPosition.y - gridOrigin.y);

                // 填充目标节点
                targetNode = grid.GetGridNode(endGridPosition.x - gridOrigin.x, endGridPosition.y - gridOrigin.y);

                // 填充网格的障碍物和路径信息
                for (int x = 0; x < gridDimensions.x; x++)
                {
                    for (int y = 0; y < gridDimensions.y; y++)
                    {
                        GridPropertyDetails gridPropertyDetails =
                            GridPropertiesManager.Instance.GetGridPropertyDetails(x + gridOrigin.x, y + gridOrigin.y,
                                sceneSave.gridPropertyDetailsDictionary);

                        if (gridPropertyDetails != null)
                        {
                            // 如果NPC障碍物
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