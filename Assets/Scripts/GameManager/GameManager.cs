using UnityEngine;

public class GameManager : SingletonMonobehaviour<GameManager>
{
    public Weather currentWeather;

    protected override void Awake()
    {
        base.Awake();

        //设置清晰度，全屏，刷新率
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow, new RefreshRate() { numerator = 60, denominator = 1 });

        // 设置初始天气为晴天
        currentWeather = Weather.dry;


    }
}
