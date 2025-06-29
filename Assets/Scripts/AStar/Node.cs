using System;
using UnityEngine;

public class Node : IComparable<Node>
{
    public Vector2Int gridPosition;
    public int gCost = 0; // 距起始点的距离
    public int hCost = 0; // 到目标点的距离
    public bool isObstacle = false;
    public int movementPenalty; // 移动惩罚
    public Node parentNode;


    public Node(Vector2Int gridPosition)
    {
        this.gridPosition = gridPosition;

        parentNode = null;
    }

    public int FCost // FCost表示的是从起点到终点的总代价
    {
        get
        {
            return gCost + hCost;
        }
    }

    public int CompareTo(Node nodeToCompare)
    {
        // 如果该节点的FCost小于要比较的节点的，那么compare将小于0
        // 如果该节点的FCost大于要比较的节点的，那么compare将大于0
        // 如果该节点的FCost等于要比较的节点的，那么compare将等于0
        int compare = FCost.CompareTo(nodeToCompare.FCost);

        // 如果FCost相等，则比较hCost，即到目标点的距离
        // 这样可以确保在相同的FCost情况下，优先选择hCost更小的节点
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return compare;
    }
}
