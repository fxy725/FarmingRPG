using UnityEngine;

public class Grid
{
    private int width;
    private int height;

    private Node[,] gridNodes;


    public Grid(int width, int height)
    {
        this.width = width;
        this.height = height;

        gridNodes = new Node[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gridNodes[x, y] = new Node(new Vector2Int(x, y));
            }
        }
    }

    // 根据给定的xPosition和yPosition获取网格节点
    public Node GetGridNode(int xPosition, int yPosition)
    {
        if (xPosition < width && yPosition < height)
        {
            return gridNodes[xPosition, yPosition];
        }
        else
        {
            Debug.Log("被请求的节点超出网格范围");
            return null;
        }
    }
}
