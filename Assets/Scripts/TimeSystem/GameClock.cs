using TMPro;
using UnityEngine;


public class GameClock : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText = null;
    [SerializeField] private TextMeshProUGUI dateText = null;
    [SerializeField] private TextMeshProUGUI seasonText = null;
    [SerializeField] private TextMeshProUGUI yearText = null;


    private void OnEnable()
    {
        EventHandler.AdvanceGameMinuteEvent += UpdateGameTime;
    }

    private void OnDisable()
    {
        EventHandler.AdvanceGameMinuteEvent -= UpdateGameTime;
    }

    // AdvanceGameMinuteEvent事件的处理程序UpdateGameTime
    private void UpdateGameTime(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        // 更新时间

        gameMinute = gameMinute - (gameMinute % 10);

        string ampm = "";
        string minute;

        if (gameHour >= 12)
        {
            ampm = " 下午";
        }
        else
        {
            ampm = " 上午";
        }

        if (gameHour >= 13)
        {
            gameHour -= 12;
        }

        if (gameMinute < 10)
        {
            minute = "0" + gameMinute.ToString();
        }
        else
        {
            minute = gameMinute.ToString();
        }

        string time = gameHour.ToString() + " : " + minute + ampm;


        timeText.SetText(time);
        dateText.SetText(gameDayOfWeek + ". " + gameDay.ToString());
        seasonText.SetText(gameSeason.ToString());
        yearText.SetText("第" + gameYear + "年");
    }

}

/*
 这个cs脚本为游戏时钟组件。
 通过每分钟更新timeText,dateText,seasonText,yearText来刷新游戏时间、日期、季节和年份。
 它使用TextMeshProUGUI组件（即TextMeshPro-Text组件）来显示这些信息。

 该脚本在挂载游戏对象与组件都启用时订阅了一个事件（AdvanceGameMinuteEvent），并在禁用时取消订阅。
 当AdvanceGameMinuteEvent事件被触发时，被绑定的UpdateGameTime方法会被调用，更新显示的时间、日期、季节和年份。 
*/
