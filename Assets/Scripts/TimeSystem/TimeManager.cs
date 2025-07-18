using System;
using System.Collections.Generic; //包含定义泛型集合的类与接口的命名空间
using UnityEngine;

public class TimeManager : SingletonMonobehaviour<TimeManager>, ISaveable
{
    // 游戏初始时间为第1年春季的第1天6点30分0秒，星期一
    private int gameYear = 1;
    private Season gameSeason = Season.春天;
    private int gameDay = 1;
    private int gameHour = 6;
    private int gameMinute = 30;
    private int gameSecond = 0;
    private string gameDayOfWeek = "Mon";

    private bool gameClockPaused = false; // 游戏时钟是否暂停

    private float gameTick = 0f;


    // 实现ISaveable接口的ISaveableUniqueID属性与GameObjectSave属性
    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }

    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }


    protected override void Awake()
    {
        base.Awake();

        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    private void OnEnable()
    {
        SaveableRegister();

        EventHandler.BeforeSceneUnloadEvent += BeforeSceneUnloadFadeOut;
        EventHandler.AfterSceneLoadEvent += AfterSceneLoadFadeIn;
    }

    private void OnDisable()
    {
        SaveableDeregister();

        EventHandler.BeforeSceneUnloadEvent -= BeforeSceneUnloadFadeOut;
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoadFadeIn;
    }

    private void BeforeSceneUnloadFadeOut()
    {
        gameClockPaused = true;
    }

    private void AfterSceneLoadFadeIn()
    {
        gameClockPaused = false;
    }


    private void Start()
    {
        EventHandler.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
    }

    private void Update()
    {
        if (!gameClockPaused) // 如果游戏时钟没有暂停
        {
            GameTick();
        }
    }

    private void GameTick()
    {
        gameTick += Time.deltaTime;

        // 当实际时间等于或超过游戏时间的最小刻度（即secondsPerGameSecond秒）时，重置gameTick，重新累加，并调用UpdateGameSecond方法
        if (gameTick >= Settings.secondsPerGameSecond)
        {
            gameTick -= Settings.secondsPerGameSecond;

            UpdateGameSecond();
        }
    }

    private void UpdateGameSecond()
    {
        gameSecond++; // 增加游戏秒数

        // 如果游戏秒数超过59秒，则将游戏秒数重置为0，并增加游戏分钟数
        if (gameSecond > 59)
        {
            gameSecond = 0;
            gameMinute++;

            // 如果游戏分钟数超过59分钟，则将游戏分钟数重置为0，并增加游戏小时数
            if (gameMinute > 59)
            {
                gameMinute = 0;
                gameHour++;

                // 如果游戏小时数超过23小时，则将游戏小时数重置为0，并增加游戏天数
                if (gameHour > 23)
                {
                    gameHour = 0;
                    gameDay++;

                    // 如果游戏天数超过30天，则将游戏天数重置为1，并到达下一个游戏季节
                    if (gameDay > 30)
                    {
                        gameDay = 1;

                        int gs = (int)gameSeason;
                        gs++;

                        gameSeason = (Season)gs;

                        // 如果游戏季节超过3个季节，则将游戏季节重置为0，并增加游戏年份
                        if (gs > 3)
                        {
                            gs = 0;
                            gameSeason = (Season)gs;

                            gameYear++;

                            // 如果游戏年份超过9999年，则将游戏年份重置为1
                            if (gameYear > 9999)
                                gameYear = 1;


                            EventHandler.CallAdvanceGameYearEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
                        }

                        EventHandler.CallAdvanceGameSeasonEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
                    }

                    gameDayOfWeek = GetDayOfWeek();
                    EventHandler.CallAdvanceGameDayEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
                }

                EventHandler.CallAdvanceGameHourEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
            }

            EventHandler.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);

        }

        // 如果需要，这里可以调用更新游戏秒事件
    }

    private string GetDayOfWeek()
    {
        int totalDays = (((int)gameSeason) * 30) + gameDay;
        int dayOfWeek = totalDays % 7;

        switch (dayOfWeek)
        {
            case 1:
                return "Mon";

            case 2:
                return "Tue";

            case 3:
                return "Wed";

            case 4:
                return "Thu";

            case 5:
                return "Fri";

            case 6:
                return "Sat";

            case 0:
                return "Sun";

            default:
                return "";
        }
    }

    public TimeSpan GetGameTime()
    {
        TimeSpan gameTime = new TimeSpan(gameHour, gameMinute, gameSecond);

        return gameTime;
    }


    //TODO:Remove
    /// <summary>
    /// Advance 1 game minute
    /// </summary>
    public void TestAdvanceGameMinute()
    {
        for (int i = 0; i < 60; i++)
        {
            UpdateGameSecond();
        }
    }

    //TODO:Remove
    /// <summary>
    /// Advance 1 day
    /// </summary>
    public void TestAdvanceGameDay()
    {
        for (int i = 0; i < 86400; i++)
        {
            UpdateGameSecond();
        }
    }


    // 实现ISaveable接口的ISaveableRegister方法,ISaveableDeregister方法,ISaveableSave方法,ISaveableLoad方法,ISaveableStoreScene方法,ISaveableRestoreScene方法
    public void SaveableRegister()
    {
        SaveLoadManager.Instance.saveableObjectList.Add(this);
    }

    public void SaveableDeregister()
    {
        SaveLoadManager.Instance.saveableObjectList.Remove(this);
    }

    public GameObjectSave SaveData()
    {
        // 如果存在，则删除现有场景保存
        GameObjectSave.sceneData.Remove(Settings.PersistentScene);

        // 创建新的场景保存
        SceneSave sceneSave = new SceneSave();

        // 创建新的int字典
        sceneSave.intDictionary = new Dictionary<string, int>();

        // 创建新的字符串字典
        sceneSave.stringDictionary = new Dictionary<string, string>();

        // 添加值到int字典
        sceneSave.intDictionary.Add("gameYear", gameYear);
        sceneSave.intDictionary.Add("gameDay", gameDay);
        sceneSave.intDictionary.Add("gameHour", gameHour);
        sceneSave.intDictionary.Add("gameMinute", gameMinute);
        sceneSave.intDictionary.Add("gameSecond", gameSecond);

        // 添加值到字符串字典
        sceneSave.stringDictionary.Add("gameDayOfWeek", gameDayOfWeek);
        sceneSave.stringDictionary.Add("gameSeason", gameSeason.ToString());

        // 添加场景保存到游戏对象
        GameObjectSave.sceneData.Add(Settings.PersistentScene, sceneSave);

        return GameObjectSave;
    }

    public void LoadData(GameSave gameSave)
    {
        // 从gameSave数据中获取保存的游戏对象
        if (gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;

            // 获取保存的场景数据
            if (GameObjectSave.sceneData.TryGetValue(Settings.PersistentScene, out SceneSave sceneSave))
            {
                // 如果int和字符串字典已找到
                if (sceneSave.intDictionary != null && sceneSave.stringDictionary != null)
                {
                    // 填充保存的int值
                    if (sceneSave.intDictionary.TryGetValue("gameYear", out int savedGameYear))
                        gameYear = savedGameYear;

                    if (sceneSave.intDictionary.TryGetValue("gameDay", out int savedGameDay))
                        gameDay = savedGameDay;

                    if (sceneSave.intDictionary.TryGetValue("gameHour", out int savedGameHour))
                        gameHour = savedGameHour;

                    if (sceneSave.intDictionary.TryGetValue("gameMinute", out int savedGameMinute))
                        gameMinute = savedGameMinute;

                    if (sceneSave.intDictionary.TryGetValue("gameSecond", out int savedGameSecond))
                        gameSecond = savedGameSecond;

                    // 填充保存的字符串值
                    if (sceneSave.stringDictionary.TryGetValue("gameDayOfWeek", out string savedGameDayOfWeek))
                        gameDayOfWeek = savedGameDayOfWeek;

                    if (sceneSave.stringDictionary.TryGetValue("gameSeason", out string savedGameSeason))
                    {
                        if (Enum.TryParse<Season>(savedGameSeason, out Season season))
                        {
                            gameSeason = season;
                        }
                    }

                    // 重置游戏刻度
                    gameTick = 0f;

                    // 触发更新游戏分钟事件
                    EventHandler.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);

                    // 刷新游戏时钟
                }
            }
        }
    }
    public void StoreScene(string sceneName)
    {
        // 无需实现因为时间管理器在持久场景中运行
    }

    public void RestoreScene(string sceneName)
    {
        // 无需实现因为时间管理器在持久场景中运行
    }
}
