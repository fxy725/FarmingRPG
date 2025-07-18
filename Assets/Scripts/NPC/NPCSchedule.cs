using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPCPath))]
public class NPCSchedule : MonoBehaviour
{
    [SerializeField] private SO_NPCScheduleEventList so_NPCScheduleEventList = null;
    private SortedSet<NPCScheduleEvent> npcScheduleEventSet;
    private NPCPath npcPath;

    private void Awake()
    {
        // 加载NPC计划事件列表到排序集
        npcScheduleEventSet = new SortedSet<NPCScheduleEvent>(new NPCScheduleEventSort());

        foreach (NPCScheduleEvent npcScheduleEvent in so_NPCScheduleEventList.npcScheduleEventList)
        {
            npcScheduleEventSet.Add(npcScheduleEvent);
        }

        // 获取NPC路径组件
        npcPath = GetComponent<NPCPath>();

    }

    private void OnEnable()
    {
        EventHandler.AdvanceGameMinuteEvent += GameTimeSystem_AdvanceMinute;
    }

    private void OnDisable()
    {
        EventHandler.AdvanceGameMinuteEvent -= GameTimeSystem_AdvanceMinute;
    }

    private void GameTimeSystem_AdvanceMinute(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        int time = (gameHour * 100) + gameMinute;

        // 尝试获取匹配计划

        NPCScheduleEvent matchingNPCScheduleEvent = null;

        foreach (NPCScheduleEvent npcScheduleEvent in npcScheduleEventSet)
        {
            if (npcScheduleEvent.Time == time)
            {
                // 时间匹配现在检查参数是否匹配
                if (npcScheduleEvent.day != 0 && npcScheduleEvent.day != gameDay)
                    continue;

                if (npcScheduleEvent.season != Season.none && npcScheduleEvent.season != gameSeason)
                    continue;

                if (npcScheduleEvent.weather != Weather.none && npcScheduleEvent.weather != GameManager.Instance.currentWeather)
                    continue;

                // 计划匹配
                // Debug.Log("Schedule Matches! " + npcScheduleEvent);
                matchingNPCScheduleEvent = npcScheduleEvent;
                break;
            }
            else if (npcScheduleEvent.Time > time)
            {
                break;
            }
        }

        // 现在测试匹配计划是否不为空并执行某些操作;
        if (matchingNPCScheduleEvent != null)
        {
            // 为匹配计划构建路径
            npcPath.BuildPath(matchingNPCScheduleEvent);
        }
    }
}