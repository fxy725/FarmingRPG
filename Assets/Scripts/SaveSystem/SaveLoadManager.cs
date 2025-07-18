using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class SaveLoadManager : SingletonMonobehaviour<SaveLoadManager>
{

    public GameSave gameSave;

    public List<ISaveable> saveableObjectList; //可保存的所有对象的列表,由实现ISaveable的类来添加移除元素

    private string filePath;



    protected override void Awake()
    {
        base.Awake();

        gameSave = new GameSave();
        saveableObjectList = new List<ISaveable>();
        filePath = Application.persistentDataPath + "/WildHopeCreek.json";
    }



    public void LoadDataFromFile()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);

            gameSave = JsonConvert.DeserializeObject<GameSave>(json);

            // 遍历所有可保存对象并应用保存数据
            for (int i = saveableObjectList.Count - 1; i > -1; i--)
            {
                if (gameSave.gameObjectData.ContainsKey(saveableObjectList[i].ISaveableUniqueID))
                {
                    saveableObjectList[i].LoadData(gameSave);
                }
                // 如果可保存对象的唯一ID不在gameObjectData的键中，那么销毁该对象。
                // 如果可保存对象的唯一ID不在gameObjectData的键中，那么销毁该对象。
                else
                {
                    Component component = (Component)saveableObjectList[i];
                    Destroy(component.gameObject);
                }
            }
        }
        UIManager.Instance.DisablePauseMenu();
    }

    public void SaveDataToFile()
    {
        gameSave = new GameSave();

        // 遍历所有可保存的对象与生成保存数据
        foreach (ISaveable saveableObject in saveableObjectList)
        {
            gameSave.gameObjectData.Add(saveableObject.ISaveableUniqueID, saveableObject.SaveData());
        }

        string json = JsonConvert.SerializeObject(gameSave, Formatting.Indented);

        File.WriteAllText(filePath, json);

        UIManager.Instance.DisablePauseMenu();
    }


    public void StoreCurrentSceneData()
    {
        // 遍历所有可保存对象并触发存储场景数据
        foreach (ISaveable iSaveableObject in saveableObjectList)
        {
            iSaveableObject.StoreScene(SceneManager.GetActiveScene().name);
        }
    }

    public void RestoreCurrentSceneData()
    {
        // 遍历所有可保存对象并触发恢复场景数据
        foreach (ISaveable iSaveableObject in saveableObjectList)
        {
            iSaveableObject.RestoreScene(SceneManager.GetActiveScene().name);
        }
    }
}
