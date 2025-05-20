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

// EventHandler静态类包含了静态事件与事件处理方法的调用。
public static class EventHandler
{
    // Drop selected item event
    public static event Action DropSelectedItemEvent;

    public static void CallDropSelectedItemEvent()
    {
        DropSelectedItemEvent?.Invoke();
    }

    // Remove selected item from inventory
    public static event Action RemoveSelectedItemFromInventoryEvent;

    public static void CallRemoveSelectedItemFromInventoryEvent()
    {
        RemoveSelectedItemFromInventoryEvent?.Invoke();
    }



    // Harvest Action Effect Event
    public static event Action<Vector3, HarvestActionEffect> HarvestActionEffectEvent;

    public static void CallHarvestActionEffectEvent(Vector3 effectPosition, HarvestActionEffect harvestActionEffect)
    {
        HarvestActionEffectEvent?.Invoke(effectPosition, harvestActionEffect);
    }



    // Inventory Updated Event
    public static event Action<InventoryLocation, List<InventoryItem>> InventoryUpdatedEvent;

    public static void CallInventoryUpdatedEvent(InventoryLocation inventoryLocation, List<InventoryItem> inventoryList)
    {
        InventoryUpdatedEvent?.Invoke(inventoryLocation, inventoryList);
    }

    // Instantiate crop prefabs
    public static event Action InstantiateCropPrefabsEvent;

    public static void CallInstantiateCropPrefabsEvent()
    {
        InstantiateCropPrefabsEvent?.Invoke();
    }


    // Movement Event
    public static event MovementDelegate MovementEvent;

    // Movement Event Call For Publishers
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


    // AdvanceGameMinuteEvent静态事件的定义
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameMinuteEvent;
    //调用AdvanceGameMinuteEvent事件
    public static void CallAdvanceGameMinuteEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        //如果AdvanceGameMinuteEvent事件不为空，则调用该事件
        AdvanceGameMinuteEvent?.Invoke(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
    }

    // Advance game hour
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameHourEvent;

    public static void CallAdvanceGameHourEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        AdvanceGameHourEvent?.Invoke(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
    }

    // Advance game day
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameDayEvent;

    public static void CallAdvanceGameDayEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        AdvanceGameDayEvent?.Invoke(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
    }

    // Advance game season
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameSeasonEvent;

    public static void CallAdvanceGameSeasonEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        AdvanceGameSeasonEvent?.Invoke(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
    }

    // Advance game year
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameYearEvent;

    public static void CallAdvanceGameYearEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        AdvanceGameYearEvent?.Invoke(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
    }

    // Scene Load Events - in the order they happen

    // Before Scene Unload Fade Out Event
    public static event Action BeforeSceneUnloadFadeOutEvent;

    public static void CallBeforeSceneUnloadFadeOutEvent()
    {
        BeforeSceneUnloadFadeOutEvent?.Invoke();
    }

    // Before Scene Unload Event
    public static event Action BeforeSceneUnloadEvent;

    public static void CallBeforeSceneUnloadEvent()
    {
        BeforeSceneUnloadEvent?.Invoke();
    }

    // After Scene Loaded Event
    public static event Action AfterSceneLoadEvent;

    public static void CallAfterSceneLoadEvent()
    {
        AfterSceneLoadEvent?.Invoke();
    }

    // After Scene Load Fade In Event
    public static event Action AfterSceneLoadFadeInEvent;

    public static void CallAfterSceneLoadFadeInEvent()
    {
        AfterSceneLoadFadeInEvent?.Invoke();
    }
}