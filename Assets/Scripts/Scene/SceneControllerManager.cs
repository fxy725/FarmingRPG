using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneControllerManager : SingletonMonobehaviour<SceneControllerManager>
{
    private bool isFading;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private CanvasGroup faderCanvasGroup;
    [SerializeField] private Image faderImage;
    public SceneName startingSceneName;


    private IEnumerator Fade(float finalAlpha)
    {
        // 设置淡入淡出标志为true，所以FadeAndSwitchScenes协程不会再次被调用。
        isFading = true;

        // 确保CanvasGroup阻止场景中的射线投射，因此无法接受更多输入。
        faderCanvasGroup.blocksRaycasts = true;

        // 计算CanvasGroup应该以多快的速度淡入淡出，基于其当前alpha，最终alpha和它需要改变的两个之间的长度。
        float fadeSpeed = Mathf.Abs(faderCanvasGroup.alpha - finalAlpha) / fadeDuration;

        // 当CanvasGroup尚未达到最终alpha时...
        while (!Mathf.Approximately(faderCanvasGroup.alpha, finalAlpha))
        {
            // ... 将alpha移动到其目标alpha。
            faderCanvasGroup.alpha = Mathf.MoveTowards(faderCanvasGroup.alpha, finalAlpha,
                fadeSpeed * Time.deltaTime);

            yield return null;
        }

        // 设置标志为false，因为淡入淡出已经完成。
        isFading = false;

        // 停止CanvasGroup阻止射线投射，因此输入不再被忽略。
        faderCanvasGroup.blocksRaycasts = false;
    }

    // 这是脚本的“构建块”的协程。
    private IEnumerator FadeAndSwitchScenes(string sceneName, Vector3 spawnPosition)
    {
        // 调用场景卸载淡出事件
        EventHandler.CallBeforeSceneUnloadFadeOutEvent();

        // 开始淡入黑色并等待它完成后再继续。
        yield return StartCoroutine(Fade(1f));

        // 存储场景数据
        SaveLoadManager.Instance.StoreCurrentSceneData();

        // 设置玩家位置

        Player.Instance.gameObject.transform.position = spawnPosition;

        // 调用场景卸载事件。
        EventHandler.CallBeforeSceneUnloadEvent();

        // 卸载当前活动场景。
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);

        // 开始加载给定的场景并等待它完成。
        yield return StartCoroutine(LoadSceneAndSetActive(sceneName));

        // 调用场景加载事件。
        EventHandler.CallAfterSceneLoadEvent();

        // 恢复新场景数据
        SaveLoadManager.Instance.RestoreCurrentSceneData();

        // 开始淡入并等待它完成后再退出函数。
        yield return StartCoroutine(Fade(0f));

        // 调用场景加载淡入事件。
        EventHandler.CallAfterSceneLoadFadeInEvent();
    }


    private IEnumerator LoadSceneAndSetActive(string sceneName)
    {
        // 允许给定的场景在几帧中加载并将其添加到已经加载的场景中（目前只是持久场景）。
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        // 找到最近加载的场景（加载场景中的最后一个索引）。
        Scene newlyLoadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

        // 将新加载的场景设置为活动场景（这标记为下一个要卸载的场景）。
        SceneManager.SetActiveScene(newlyLoadedScene);
    }

    private IEnumerator Start()
    {
        // 设置初始alpha以开始黑色屏幕。
        faderImage.color = new Color(0f, 0f, 0f, 1f);
        faderCanvasGroup.alpha = 1f;

        // 开始加载第一个场景并等待它完成。
        yield return StartCoroutine(LoadSceneAndSetActive(startingSceneName.ToString()));

        // 如果此事件有任何订阅者，则调用它。
        EventHandler.CallAfterSceneLoadEvent();

        SaveLoadManager.Instance.RestoreCurrentSceneData();


        // 一旦场景加载完成，开始淡入。
        StartCoroutine(Fade(0f));
    }


    // 这是项目外部的主要接触点和影响点。
    // 当玩家想要切换场景时，将调用此方法。
    public void FadeAndLoadScene(string sceneName, Vector3 spawnPosition)
    {
        // 如果淡入淡出没有发生，则开始淡入淡出并切换场景。
        if (!isFading)
        {
            StartCoroutine(FadeAndSwitchScenes(sceneName, spawnPosition));
        }
    }

}
