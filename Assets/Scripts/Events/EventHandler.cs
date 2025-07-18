using System;
using System.Collections.Generic;
using UnityEngine;

// MovementDelegate是一个委托类型，接受多个与玩家移动与工具使用相关的参数
// 这些参数包括输入坐标、是否行走、奔跑、静止、携带物品、工具效果等
public delegate void MovementDelegate(float inputX, float inputY, bool isWalking, bool isRunning, bool isIdle, bool isCarrying, ToolEffect toolEffect,
    bool isUsingToolRight, bool isUsingToolLeft, bool isUsingToolUp, bool isUsingToolDown,
    bool isLiftingToolRight, bool isLiftingToolLeft, bool isLiftingToolUp, bool isLiftingToolDown,
    bool isPickingRight, bool isPickingLeft, bool isPickingUp, bool isPickingDown,
    bool isSwingingToolRight, bool isSwingingToolLeft, bool isSwingingToolUp, bool isSwingingToolDown,
    bool idleUp, bool idleDown, bool idleLeft, bool idleRight);


// EventHandler静态类包含了静态事件与与调用事件的静态方法
public static class EventHandler
{
    //库存事件
    // DropSelectedItemEvent 丢弃选定项事件
    public static event Action DropSelectedItemEvent;
    public static void CallDropSelectedItemEvent()
    {
        DropSelectedItemEvent?.Invoke();
    }

    // RemoveSelectedItemFromInventoryEvent 从库存中移除选定项事件
    public static event Action RemoveSelectedItemFromInventoryEvent;
    public static void CallRemoveSelectedItemFromInventoryEvent()
    {
        RemoveSelectedItemFromInventoryEvent?.Invoke();
    }

    // InventoryUpdatedEvent 库存更新事件
    public static event Action<InventoryLocation, List<InventoryItem>> InventoryUpdatedEvent;
    public static void CallInventoryUpdatedEvent(InventoryLocation inventoryLocation, List<InventoryItem> inventoryList)
    {
        InventoryUpdatedEvent?.Invoke(inventoryLocation, inventoryList);
    }


    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    // HarvestActionEffectEvent 收割动作效果事件
    public static event Action<Vector3, HarvestActionEffect> HarvestActionEffectEvent;
    public static void CallHarvestActionEffectEvent(Vector3 effectPosition, HarvestActionEffect harvestActionEffect)
    {
        HarvestActionEffectEvent?.Invoke(effectPosition, harvestActionEffect);
    }

    // InstantiateCropPrefabsEvent 实例化作物预制体事件
    public static event Action InstantiateCropPrefabsEvent;
    public static void CallInstantiateCropPrefabsEvent()
    {
        InstantiateCropPrefabsEvent?.Invoke();
    }

    // 移动事件 MovementEvent
    public static event MovementDelegate MovementEvent;
    public static void CallMovementEvent(float inputX, float inputY, bool isWalking, bool isRunning, bool isIdle, bool isCarrying, ToolEffect toolEffect,
    bool isUsingToolRight, bool isUsingToolLeft, bool isUsingToolUp, bool isUsingToolDown,
    bool isLiftingToolRight, bool isLiftingToolLeft, bool isLiftingToolUp, bool isLiftingToolDown,
    bool isPickingRight, bool isPickingLeft, bool isPickingUp, bool isPickingDown,
    bool isSwingingToolRight, bool isSwingingToolLeft, bool isSwingingToolUp, bool isSwingingToolDown,
    bool idleUp, bool idleDown, bool idleLeft, bool idleRight)
    {
        MovementEvent?.Invoke(inputX, inputY,
        isWalking, isRunning, isIdle, isCarrying,
        toolEffect,
        isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown,
        isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown,
        isPickingRight, isPickingLeft, isPickingUp, isPickingDown,
        isSwingingToolRight, isSwingingToolLeft, isSwingingToolUp, isSwingingToolDown,
        idleUp, idleDown, idleLeft, idleRight);
    }


    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    // 游戏时间推进事件
    // AdvanceGameMinuteEvent 游戏分钟时间推进事件
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameMinuteEvent;
    public static void CallAdvanceGameMinuteEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        //如果AdvanceGameMinuteEvent事件不为空，则调用该事件
        AdvanceGameMinuteEvent?.Invoke(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
    }

    // AdvanceGameHourEvent 游戏小时时间推进事件
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameHourEvent;
    public static void CallAdvanceGameHourEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        AdvanceGameHourEvent?.Invoke(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
    }

    // AdvanceGameDayEvent 游戏天数时间推进事件
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameDayEvent;
    public static void CallAdvanceGameDayEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        AdvanceGameDayEvent?.Invoke(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
    }

    // AdvanceGameSeasonEvent 游戏季节时间推进事件
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameSeasonEvent;
    public static void CallAdvanceGameSeasonEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        AdvanceGameSeasonEvent?.Invoke(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
    }

    // AdvanceGameYearEvent 游戏年份时间推进事件
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameYearEvent;
    public static void CallAdvanceGameYearEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        AdvanceGameYearEvent?.Invoke(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
    }


    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    // SceneLoadEvents 场景加载事件 它们按顺序发生

    // AfterSceneLoadEvent 场景加载后事件
    public static event Action AfterSceneLoadEvent;
    public static void CallAfterSceneLoadEvent()
    {
        AfterSceneLoadEvent?.Invoke();
    }

    // AfterSceneLoadFadeInEvent 场景加载后淡入事件
    public static event Action AfterSceneLoadFadeInEvent;
    public static void CallAfterSceneLoadFadeInEvent()
    {
        AfterSceneLoadFadeInEvent?.Invoke();
    }

    // BeforeSceneUnloadFadeOutEvent 场景卸载前淡出事件
    public static event Action BeforeSceneUnloadFadeOutEvent;
    public static void CallBeforeSceneUnloadFadeOutEvent()
    {
        BeforeSceneUnloadFadeOutEvent?.Invoke();
    }

    // BeforeSceneUnloadEvent 场景卸载前事件
    public static event Action BeforeSceneUnloadEvent;
    public static void CallBeforeSceneUnloadEvent()
    {
        BeforeSceneUnloadEvent?.Invoke();
    }
}