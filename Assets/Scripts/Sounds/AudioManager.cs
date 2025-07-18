using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : SingletonMonobehaviour<AudioManager>
{
    [SerializeField] private GameObject soundPrefab = null;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource ambientSoundAudioSource = null;

    [SerializeField] private AudioSource gameMusicAudioSource = null;

    [Header("Audio Mixers")]
    [SerializeField] private AudioMixer gameAudioMixer = null;

    [Header("Audio Snapshots")]
    [SerializeField] private AudioMixerSnapshot gameMusicSnapshot = null;

    [SerializeField] private AudioMixerSnapshot gameAmbientSnapshot = null;

    [Header("Other")]
    // 声音列表和字典
    [SerializeField] private SO_SoundList so_soundList = null;

    [SerializeField] private SO_SceneSoundsList so_sceneSoundsList = null;
    [SerializeField] private float defaultSceneMusicPlayTimeSeconds = 120f;
    [SerializeField] private float sceneMusicStartMinSecs = 20f;
    [SerializeField] private float sceneMusicStartMaxSecs = 40f;
    [SerializeField] private float musicTransitionSecs = 8f;


    private Dictionary<SoundName, SoundItem> soundDictionary;
    private Dictionary<SceneName, SceneSoundsItem> sceneSoundsDictionary;

    private Coroutine playSceneSoundsCoroutine;

    protected override void Awake()
    {
        base.Awake();

        // 初始化声音字典
        soundDictionary = new Dictionary<SoundName, SoundItem>();

        // 加载声音字典
        foreach (SoundItem soundItem in so_soundList.soundDetails)
        {
            soundDictionary.Add(soundItem.soundName, soundItem);
        }

        // 初始化场景声音字典
        sceneSoundsDictionary = new Dictionary<SceneName, SceneSoundsItem>();

        // 加载场景声音字典
        foreach (SceneSoundsItem sceneSoundsItem in so_sceneSoundsList.sceneSoundsDetails)
        {
            sceneSoundsDictionary.Add(sceneSoundsItem.sceneName, sceneSoundsItem);
        }

    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += PlaySceneSounds;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= PlaySceneSounds;
    }

    private void PlaySceneSounds()
    {
        SoundItem musicSoundItem = null;
        SoundItem ambientSoundItem = null;

        float musicPlayTime = defaultSceneMusicPlayTimeSeconds;

        // 尝试获取当前场景
        if (Enum.TryParse<SceneName>(SceneManager.GetActiveScene().name, true, out SceneName currentSceneName))
        {
            // 获取场景的音乐和环境声音
            if (sceneSoundsDictionary.TryGetValue(currentSceneName, out SceneSoundsItem sceneSoundsItem))
            {
                soundDictionary.TryGetValue(sceneSoundsItem.musicForScene, out musicSoundItem);
                soundDictionary.TryGetValue(sceneSoundsItem.ambientSoundForScene, out ambientSoundItem);
            }
            else
            {
                return;
            }

            // 停止任何正在播放的场景声音
            if (playSceneSoundsCoroutine != null)
            {
                StopCoroutine(playSceneSoundsCoroutine);
            }

            // 播放场景环境声音和音乐
            playSceneSoundsCoroutine = StartCoroutine(PlaySceneSoundsRoutine(musicPlayTime, musicSoundItem, ambientSoundItem));
        }
    }

    private IEnumerator PlaySceneSoundsRoutine(float musicPlaySeconds, SoundItem musicSoundItem, SoundItem ambientSoundItem)
    {

        if (musicSoundItem != null && ambientSoundItem != null)
        {
            // 从环境声音开始
            PlayAmbientSoundClip(ambientSoundItem, 0f);

            // 等待随机范围的秒数再播放音乐
            yield return new WaitForSeconds(UnityEngine.Random.Range(sceneMusicStartMinSecs, sceneMusicStartMaxSecs));

            // 播放音乐
            PlayMusicSoundClip(musicSoundItem, musicTransitionSecs);

            // 等待音乐播放秒数再过渡到环境声音
            yield return new WaitForSeconds(musicPlaySeconds);

            // Play ambient sound clip
            PlayAmbientSoundClip(ambientSoundItem, musicTransitionSecs);
        }
    }

    private void PlayMusicSoundClip(SoundItem musicSoundItem, float transitionTimeSeconds)
    {
        // 设置音量
        gameAudioMixer.SetFloat("MusicVolume", ConvertSoundVolumeDecimalFractionToDecibels(musicSoundItem.soundVolume));

        // 设置剪辑并播放
        gameMusicAudioSource.clip = musicSoundItem.soundClip;
        gameMusicAudioSource.Play();

        // 过渡到音乐快照
        gameMusicSnapshot.TransitionTo(transitionTimeSeconds);
    }

    private void PlayAmbientSoundClip(SoundItem ambientSoundItem, float transitionTimeSeconds)
    {
        // 设置音量
        gameAudioMixer.SetFloat("AmbientVolume", ConvertSoundVolumeDecimalFractionToDecibels(ambientSoundItem.soundVolume));

        // 设置剪辑并播放
        ambientSoundAudioSource.clip = ambientSoundItem.soundClip;
        ambientSoundAudioSource.Play();

        // 过渡到环境
        gameAmbientSnapshot.TransitionTo(transitionTimeSeconds);
    }

    /// <summary>
    /// 将volumeDecimalFraction转换为-80到+20分贝范围
    /// </summary>
    private float ConvertSoundVolumeDecimalFractionToDecibels(float volumeDecimalFraction)
    {
        // 将音量从十进制分数转换为-80到+20分贝范围

        return (volumeDecimalFraction * 100f - 80f);
    }


    public void PlaySound(SoundName soundName)
    {
        if (soundDictionary.TryGetValue(soundName, out SoundItem soundItem) && soundPrefab != null)
        {
            GameObject soundGameObject = PoolManager.Instance.ReuseObject(soundPrefab, Vector3.zero, Quaternion.identity);

            Sound sound = soundGameObject.GetComponent<Sound>();

            sound.SetSound(soundItem);
            soundGameObject.SetActive(true);
            StartCoroutine(DisableSound(soundGameObject, soundItem.soundClip.length));
        }
    }

    private IEnumerator DisableSound(GameObject soundGameObject, float soundDuration)
    {
        yield return new WaitForSeconds(soundDuration);
        soundGameObject.SetActive(false);
    }
}