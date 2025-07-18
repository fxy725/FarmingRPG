using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(GenerateGUID))]
public class SceneItemsManager : SingletonMonobehaviour<SceneItemsManager>, ISaveable
{
    private Transform parentItem;
    [SerializeField] private GameObject itemPrefab = null;

    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }

    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }

    private void AfterSceneLoad()
    {
        parentItem = GameObject.FindGameObjectWithTag(Tags.ItemsParentTransform).transform;
    }

    protected override void Awake()
    {
        base.Awake();

        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    /// <summary>
    /// 销毁当前场景中的物品
    /// </summary>
    private void DestroySceneItems()
    {
        // 获取场景中的所有物品
        Item[] itemsInScene = GameObject.FindObjectsByType<Item>(FindObjectsSortMode.None);

        // 遍历所有场景物品并销毁它们
        for (int i = itemsInScene.Length - 1; i > -1; i--)
        {
            Destroy(itemsInScene[i].gameObject);
        }
    }

    public void InstantiateSceneItem(int itemCode, Vector3 itemPosition)
    {
        GameObject itemGameObject = Instantiate(itemPrefab, itemPosition, Quaternion.identity, parentItem);
        Item item = itemGameObject.GetComponent<Item>();
        item.Init(itemCode);
    }

    private void InstantiateSceneItems(List<SceneItem> sceneItemList)
    {
        GameObject itemGameObject;

        foreach (SceneItem sceneItem in sceneItemList)
        {
            itemGameObject = Instantiate(itemPrefab, new Vector3(sceneItem.position.x, sceneItem.position.y, sceneItem.position.z), Quaternion.identity, parentItem);

            Item item = itemGameObject.GetComponent<Item>();
            item.ItemCode = sceneItem.itemCode;
            item.name = sceneItem.itemName;
        }
    }

    private void OnDisable()
    {
        SaveableDeregister();
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoad;
    }

    private void OnEnable()
    {
        SaveableRegister();
        EventHandler.AfterSceneLoadEvent += AfterSceneLoad;
    }

    public void SaveableDeregister()
    {
        SaveLoadManager.Instance.saveableObjectList.Remove(this);
    }

    public void LoadData(GameSave gameSave)
    {
        if (gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;

            // 恢复当前场景的数据
            RestoreScene(SceneManager.GetActiveScene().name);
        }
    }



    public void RestoreScene(string sceneName)
    {
        if (GameObjectSave.sceneData.TryGetValue(sceneName, out SceneSave sceneSave))
        {
            if (sceneSave.listSceneItem != null)
            {
                // 场景列表物品找到 - 销毁场景中的现有物品
                DestroySceneItems();

                // 现在实例化场景物品列表
                InstantiateSceneItems(sceneSave.listSceneItem);
            }
        }
    }

    public void SaveableRegister()
    {
        SaveLoadManager.Instance.saveableObjectList.Add(this);
    }

    public GameObjectSave SaveData()
    {
        // 存储当前场景数据
        StoreScene(SceneManager.GetActiveScene().name);

        return GameObjectSave;
    }



    public void StoreScene(string sceneName)
    {
        // 如果存在，则删除游戏对象的旧场景保存
        GameObjectSave.sceneData.Remove(sceneName);

        // 获取场景中的所有物品
        List<SceneItem> sceneItemList = new List<SceneItem>();
        Item[] itemsInScene = FindObjectsByType<Item>(FindObjectsSortMode.None);

        // 遍历所有场景物品
        foreach (Item item in itemsInScene)
        {
            SceneItem sceneItem = new SceneItem();
            sceneItem.itemCode = item.ItemCode;
            sceneItem.position = new Vector3(item.transform.position.x, item.transform.position.y, item.transform.position.z);
            sceneItem.itemName = item.name;

            // 添加场景物品到列表
            sceneItemList.Add(sceneItem);
        }

        // 创建场景物品列表并在场景保存中设置为场景物品列表
        SceneSave sceneSave = new SceneSave();
        sceneSave.listSceneItem = sceneItemList;

        // 添加场景保存到游戏对象
        GameObjectSave.sceneData.Add(sceneName, sceneSave);
    }
}
