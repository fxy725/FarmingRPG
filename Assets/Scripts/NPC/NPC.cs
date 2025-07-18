using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPCMovement))]
[RequireComponent(typeof(GenerateGUID))]
public class NPC : MonoBehaviour, ISaveable
{
    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }

    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }

    private NPCMovement npcMovement;



    private void Awake()
    {
        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }
    private void OnEnable()
    {
        SaveableRegister();
    }

    private void OnDisable()
    {
        SaveableDeregister();
    }

    private void Start()
    {
        npcMovement = GetComponent<NPCMovement>();
    }

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
        // 移除当前场景保存
        GameObjectSave.sceneData.Remove(Settings.PersistentScene);

        // 创建场景保存
        SceneSave sceneSave = new SceneSave();

        sceneSave.vector3Dictionary = new Dictionary<string, Vector3>();

        // 创建字符串字典
        sceneSave.stringDictionary = new Dictionary<string, string>();

        // 存储目标网格位置、目标世界位置和目标场景
        sceneSave.vector3Dictionary.Add("npcTargetGridPosition", new Vector3(npcMovement.npcTargetGridPosition.x, npcMovement.npcTargetGridPosition.y, npcMovement.npcTargetGridPosition.z));
        sceneSave.vector3Dictionary.Add("npcTargetWorldPosition", new Vector3(npcMovement.npcTargetWorldPosition.x, npcMovement.npcTargetWorldPosition.y, npcMovement.npcTargetWorldPosition.z));
        sceneSave.stringDictionary.Add("npcTargetScene", npcMovement.npcTargetScene.ToString());

        // 添加场景保存到游戏对象
        GameObjectSave.sceneData.Add(Settings.PersistentScene, sceneSave);

        return GameObjectSave;
    }

    public void LoadData(GameSave gameSave)
    {
        // 获取游戏对象保存
        if (gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;

            // 获取场景保存
            if (GameObjectSave.sceneData.TryGetValue(Settings.PersistentScene, out SceneSave sceneSave))
            {
                // 如果字典不为空
                if (sceneSave.vector3Dictionary != null && sceneSave.stringDictionary != null)
                {
                    // 目标网格位置
                    if (sceneSave.vector3Dictionary.TryGetValue("npcTargetGridPosition", out Vector3 savedNPCTargetGridPosition))
                    {
                        npcMovement.npcTargetGridPosition = new Vector3Int((int)savedNPCTargetGridPosition.x, (int)savedNPCTargetGridPosition.y, (int)savedNPCTargetGridPosition.z);
                        npcMovement.npcCurrentGridPosition = npcMovement.npcTargetGridPosition;
                    }

                    // 目标世界位置
                    if (sceneSave.vector3Dictionary.TryGetValue("npcTargetWorldPosition", out Vector3 savedNPCTargetWorldPosition))
                    {
                        npcMovement.npcTargetWorldPosition = new Vector3(savedNPCTargetWorldPosition.x, savedNPCTargetWorldPosition.y, savedNPCTargetWorldPosition.z);
                        transform.position = npcMovement.npcTargetWorldPosition;
                    }

                    // 目标场景
                    if (sceneSave.stringDictionary.TryGetValue("npcTargetScene", out string savedTargetScene))
                    {
                        if (Enum.TryParse<SceneName>(savedTargetScene, out SceneName sceneName))
                        {
                            npcMovement.npcTargetScene = sceneName;
                            npcMovement.npcCurrentScene = npcMovement.npcTargetScene;

                        }
                    }

                    // 清除任何当前NPC移动
                    npcMovement.CancelNPCMovement();

                }

            }

        }
    }

    public void StoreScene(string sceneName)
    {
        // 这里不需要做任何事情，因为是在持久场景上
    }

    public void RestoreScene(string sceneName)
    {
        // 这里不需要做任何事情，因为是在持久场景上
    }

}