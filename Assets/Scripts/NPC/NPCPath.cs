using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPCMovement))]
public class NPCPath : MonoBehaviour
{
    public Stack<NPCMovementStep> npcMovementStepStack;

    private NPCMovement npcMovement;

    private void Awake()
    {
        npcMovement = GetComponent<NPCMovement>();
        npcMovementStepStack = new Stack<NPCMovementStep>();
    }

    public void ClearPath()
    {
        npcMovementStepStack.Clear();
    }

    public void BuildPath(NPCScheduleEvent npcScheduleEvent)
    {
        ClearPath();

        // 如果计划事件是与当前NPC场景相同的场景
        if (npcScheduleEvent.toSceneName == npcMovement.npcCurrentScene)
        {
            Vector2Int npcCurrentGridPosition = (Vector2Int)npcMovement.npcCurrentGridPosition;

            Vector2Int npcTargetGridPosition = (Vector2Int)npcScheduleEvent.toGridCoordinate;

            // 构建路径并添加移动步骤到移动步骤堆栈
            NPCManager.Instance.BuildPath(npcScheduleEvent.toSceneName, npcCurrentGridPosition, npcTargetGridPosition, npcMovementStepStack);


        }
        // 如果计划事件是另一个场景中的位置
        else if (npcScheduleEvent.toSceneName != npcMovement.npcCurrentScene)
        {
            SceneRoute sceneRoute;

            // 获取匹配计划的场景路由
            sceneRoute = NPCManager.Instance.GetSceneRoute(npcMovement.npcCurrentScene.ToString(), npcScheduleEvent.toSceneName.ToString());

            // 是否找到有效的场景路由？
            if (sceneRoute != null)
            {
                // 反向遍历场景路径

                for (int i = sceneRoute.scenePathList.Count - 1; i >= 0; i--)
                {
                    int toGridX, toGridY, fromGridX, fromGridY;

                    ScenePath scenePath = sceneRoute.scenePathList[i];

                    // 检查这是否是最终目的地
                    if (scenePath.toGridCell.x >= Settings.maxGridWidth || scenePath.toGridCell.y >= Settings.maxGridHeight)
                    {
                        // 如果是，则使用最终目的地网格单元
                        toGridX = npcScheduleEvent.toGridCoordinate.x;
                        toGridY = npcScheduleEvent.toGridCoordinate.y;
                    }
                    else
                    {
                        // 否则使用场景路径到位置
                        toGridX = scenePath.toGridCell.x;
                        toGridY = scenePath.toGridCell.y;
                    }

                    // 检查这是否是起始位置
                    if (scenePath.fromGridCell.x >= Settings.maxGridWidth || scenePath.fromGridCell.y >= Settings.maxGridHeight)
                    {
                        // 如果是，则使用NPC位置
                        fromGridX = npcMovement.npcCurrentGridPosition.x;
                        fromGridY = npcMovement.npcCurrentGridPosition.y;
                    }
                    else
                    {
                        // 否则使用场景路径从位置
                        fromGridX = scenePath.fromGridCell.x;
                        fromGridY = scenePath.fromGridCell.y;
                    }

                    Vector2Int fromGridPosition = new Vector2Int(fromGridX, fromGridY);

                    Vector2Int toGridPosition = new Vector2Int(toGridX, toGridY);

                    // 构建路径并添加移动步骤到移动步骤堆栈
                    NPCManager.Instance.BuildPath(scenePath.sceneName, fromGridPosition, toGridPosition, npcMovementStepStack);
                }
            }
        }


        // 如果堆栈计数大于1，则更新时间并弹出第一个项目，即起始位置
        if (npcMovementStepStack.Count > 1)
        {
            UpdateTimesOnPath();
            npcMovementStepStack.Pop(); // 丢弃起始步骤

            // 设置计划事件详情在NPC移动
            npcMovement.SetScheduleEventDetails(npcScheduleEvent);
        }

    }

    /// <summary>
    /// 更新路径移动步骤与预期游戏时间
    /// </summary>
    public void UpdateTimesOnPath()
    {
        // 获取当前游戏时间
        TimeSpan currentGameTime = TimeManager.Instance.GetGameTime();

        NPCMovementStep previousNPCMovementStep = null;

        foreach (NPCMovementStep npcMovementStep in npcMovementStepStack)
        {
            if (previousNPCMovementStep == null)
                previousNPCMovementStep = npcMovementStep;

            npcMovementStep.hour = currentGameTime.Hours;
            npcMovementStep.minute = currentGameTime.Minutes;
            npcMovementStep.second = currentGameTime.Seconds;

            TimeSpan movementTimeStep;

            // 如果对角线
            if (MovementIsDiagonal(npcMovementStep, previousNPCMovementStep))
            {
                movementTimeStep = new TimeSpan(0, 0, (int)(Settings.gridCellDiagonalSize / Settings.secondsPerGameSecond / npcMovement.npcNormalSpeed));
            }
            else
            {
                movementTimeStep = new TimeSpan(0, 0, (int)(Settings.gridCellSize / Settings.secondsPerGameSecond / npcMovement.npcNormalSpeed));
            }

            currentGameTime = currentGameTime.Add(movementTimeStep);

            previousNPCMovementStep = npcMovementStep;
        }

    }

    /// <summary>
    /// 返回true如果前一个移动步骤是对角线移动步骤，否则返回false
    /// </summary>
    private bool MovementIsDiagonal(NPCMovementStep npcMovementStep, NPCMovementStep previousNPCMovementStep)
    {
        if ((npcMovementStep.gridCoordinate.x != previousNPCMovementStep.gridCoordinate.x) && (npcMovementStep.gridCoordinate.y != previousNPCMovementStep.gridCoordinate.y))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}