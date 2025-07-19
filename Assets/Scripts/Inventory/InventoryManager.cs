using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : SingletonMonobehaviour<InventoryManager>, ISaveable
{
    private UIInventoryBar inventoryBar; //库存栏UI组件的引用

    private Dictionary<int, ItemDetails> itemDetailsDictionary; // 物品详情字典，键为物品代码，值为物品详情

    private int[] selectedInventoryItem;

    public List<InventoryItem>[] inventoryLists;

    [HideInInspector] public int[] inventoryListCapacityIntArray; // the index of the array is the inventory list (from the InventoryLocation enum), and the value is the capacity of that inventory list

    [SerializeField] private SO_ItemList itemList = null;

    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }

    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }


    protected override void Awake()
    {
        base.Awake();

        // 创建库存列表
        CreateInventoryLists();

        // 创建物品详情字典
        CreateItemDetailsDictionary();

        // 初始化选定库存项数组
        selectedInventoryItem = new int[(int)InventoryLocation.count];

        for (int i = 0; i < selectedInventoryItem.Length; i++)
        {
            selectedInventoryItem[i] = -1;
        }

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
        inventoryBar = FindFirstObjectByType<UIInventoryBar>();
    }

    private void CreateInventoryLists()
    {
        inventoryLists = new List<InventoryItem>[(int)InventoryLocation.count];

        for (int i = 0; i < (int)InventoryLocation.count; i++)
        {
            inventoryLists[i] = new List<InventoryItem>();
        }

        // 初始化库存列表容量数组
        inventoryListCapacityIntArray = new int[(int)InventoryLocation.count];

        // 初始化玩家库存列表容量
        inventoryListCapacityIntArray[(int)InventoryLocation.player] = Settings.playerInitialInventoryCapacity;
    }

    /// <summary>
    /// 从脚本对象物品列表填充 itemDetailsDictionary
    /// </summary>
    private void CreateItemDetailsDictionary()
    {
        itemDetailsDictionary = new Dictionary<int, ItemDetails>();

        foreach (ItemDetails itemDetails in itemList.itemDetails)
        {
            itemDetailsDictionary.Add(itemDetails.itemCode, itemDetails);
        }
    }

    /// <summary>
    /// 添加一个物品到库存列表，然后销毁 gameObjectToDelete
    /// </summary>
    public void AddItem(InventoryLocation inventoryLocation, Item item, GameObject gameObjectToDelete)
    {
        AddItem(inventoryLocation, item);

        Destroy(gameObjectToDelete);
    }

    /// <summary>
    /// 添加一个物品到库存列表
    /// </summary>
    public void AddItem(InventoryLocation inventoryLocation, Item item)
    {
        int itemCode = item.ItemCode;
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        // 检查库存是否已经包含该物品
        int itemPosition = FindItemInInventory(inventoryLocation, itemCode);

        if (itemPosition != -1)
        {
            AddItemAtPosition(inventoryList, itemCode, itemPosition);
        }
        else
        {
            AddItemAtPosition(inventoryList, itemCode);
        }

        // 发送事件，库存已更新
        EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
    }

    /// <summary>
    /// 添加一个物品到库存列表
    /// </summary>
    public void AddItem(InventoryLocation inventoryLocation, int itemCode)
    {
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        // 检查库存是否已经包含该物品
        int itemPosition = FindItemInInventory(inventoryLocation, itemCode);

        if (itemPosition != -1)
        {
            AddItemAtPosition(inventoryList, itemCode, itemPosition);
        }
        else
        {
            AddItemAtPosition(inventoryList, itemCode);
        }

        // 发送事件，库存已更新
        EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
    }




    /// <summary>
    /// 添加一个物品到库存列表的末尾
    /// </summary>
    private void AddItemAtPosition(List<InventoryItem> inventoryList, int itemCode)
    {
        InventoryItem inventoryItem = new InventoryItem();

        inventoryItem.itemCode = itemCode;
        inventoryItem.itemQuantity = 1;
        inventoryList.Add(inventoryItem);

        //DebugPrintInventoryList(inventoryList);
    }

    /// <summary>
    /// 添加一个物品到库存列表的指定位置
    /// </summary>
    private void AddItemAtPosition(List<InventoryItem> inventoryList, int itemCode, int position)
    {
        InventoryItem inventoryItem = new InventoryItem();

        int quantity = inventoryList[position].itemQuantity + 1;
        inventoryItem.itemQuantity = quantity;
        inventoryItem.itemCode = itemCode;
        inventoryList[position] = inventoryItem;


        //DebugPrintInventoryList(inventoryList);
    }

    ///<summary>
    /// 交换 inventoryLocation 库存列表中 fromItem 索引处的物品和 toItem 索引处的物品
    ///</summary>

    public void SwapInventoryItems(InventoryLocation inventoryLocation, int fromItem, int toItem)
    {
        // if fromItem index and toItemIndex are within the bounds of the list, not the same, and greater than or equal to zero
        if (fromItem < inventoryLists[(int)inventoryLocation].Count && toItem < inventoryLists[(int)inventoryLocation].Count
             && fromItem != toItem && fromItem >= 0 && toItem >= 0)
        {
            InventoryItem fromInventoryItem = inventoryLists[(int)inventoryLocation][fromItem];
            InventoryItem toInventoryItem = inventoryLists[(int)inventoryLocation][toItem];

            inventoryLists[(int)inventoryLocation][toItem] = fromInventoryItem;
            inventoryLists[(int)inventoryLocation][fromItem] = toInventoryItem;

            // 发送事件，库存已更新
            EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
        }
    }

    /// <summary>
    /// 清除 inventoryLocation 的选定库存项
    /// </summary>
    public void ClearSelectedInventoryItem(InventoryLocation inventoryLocation)
    {
        selectedInventoryItem[(int)inventoryLocation] = -1;
    }



    /// <summary>
    /// 查找库存中是否包含 itemCode，如果包含则返回物品位置，否则返回-1
    /// </summary>
    public int FindItemInInventory(InventoryLocation inventoryLocation, int itemCode)
    {
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        for (int i = 0; i < inventoryList.Count; i++)
        {
            if (inventoryList[i].itemCode == itemCode)
            {
                return i;
            }
        }

        return -1;
    }


    // 返回 itemCode 对应的 itemDetails（从 SO_ItemList），如果 itemCode 不存在则返回 null
    public ItemDetails GetItemDetails(int itemCode)
    {
        ItemDetails itemDetails;

        if (itemDetailsDictionary.TryGetValue(itemCode, out itemDetails))
        {
            return itemDetails;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 返回 inventoryLocation 的选定库存项的物品详情，如果选定项不存在则返回 null
    /// </summary>
    public ItemDetails GetSelectedInventoryItemDetails(InventoryLocation inventoryLocation)
    {
        int itemCode = GetSelectedInventoryItem(inventoryLocation);

        if (itemCode == -1)
        {
            return null;
        }
        else
        {
            return GetItemDetails(itemCode);
        }
    }


    /// <summary>
    /// 获取 inventoryLocation 的选定库存项的物品代码，如果选定项不存在则返回-1
    /// </summary>
    private int GetSelectedInventoryItem(InventoryLocation inventoryLocation)
    {
        return selectedInventoryItem[(int)inventoryLocation];
    }




    // 获取物品类型描述 - 返回给定 ItemType 的物品类型描述字符串
    public string GetItemTypeDescription(ItemType itemType)
    {
        string itemTypeDescription = itemType switch
        {
            ItemType.Breaking_tool => Settings.BreakingTool,
            ItemType.Chopping_tool => Settings.ChoppingTool,
            ItemType.Hoeing_tool => Settings.HoeingTool,
            ItemType.Reaping_tool => Settings.ReapingTool,
            ItemType.Watering_tool => Settings.WateringTool,
            ItemType.Collecting_tool => Settings.CollectingTool,
            _ => itemType.ToString(),
        };
        return itemTypeDescription;
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
        // 创建新的场景保存
        SceneSave sceneSave = new SceneSave();

        // 移除任何现有的场景保存，用于持久场景中的这个游戏对象
        GameObjectSave.sceneData.Remove(Settings.PersistentScene);

        // 添加库存列表数组到持久场景保存
        sceneSave.listInvItemArray = inventoryLists;

        // 添加库存列表容量数组到持久场景保存
        sceneSave.intArrayDictionary = new Dictionary<string, int[]>
        {
            { "inventoryListCapacityArray", inventoryListCapacityIntArray }
        };

        // 添加场景保存到游戏对象
        GameObjectSave.sceneData.Add(Settings.PersistentScene, sceneSave);

        return GameObjectSave;
    }


    public void LoadData(GameSave gameSave)
    {
        if (gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;

            // 需要找到库存列表 - 首先尝试定位游戏对象的保存场景
            if (gameObjectSave.sceneData.TryGetValue(Settings.PersistentScene, out SceneSave sceneSave))
            {
                // 持久场景中存在列表库存项数组
                if (sceneSave.listInvItemArray != null)
                {
                    inventoryLists = sceneSave.listInvItemArray;

                    // 发送事件，库存已更新
                    for (int i = 0; i < (int)InventoryLocation.count; i++)
                    {
                        EventHandler.CallInventoryUpdatedEvent((InventoryLocation)i, inventoryLists[i]);
                    }

                    // 清除玩家携带的物品
                    Player.Instance.ClearCarriedItem();

                    // 清除库存栏上的任何高亮
                    inventoryBar.ClearHighlightOnInventorySlots();
                }

                // 场景中存在整数数组字典
                if (sceneSave.intArrayDictionary != null && sceneSave.intArrayDictionary.TryGetValue("inventoryListCapacityArray", out int[] inventoryCapacityArray))
                {
                    inventoryListCapacityIntArray = inventoryCapacityArray;
                }
            }

        }
    }

    public void StoreScene(string sceneName)
    {
        // 这里不需要做任何事情，因为库存管理器在持久场景中
    }

    public void RestoreScene(string sceneName)
    {
        // 这里不需要做任何事情，因为库存管理器在持久
    }




    /// <summary>
    /// 从库存中移除一个物品，并在它被丢弃的位置创建一个游戏对象
    /// </summary>
    public void RemoveItem(InventoryLocation inventoryLocation, int itemCode)
    {
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        // 检查库存是否已经包含该物品
        int itemPosition = FindItemInInventory(inventoryLocation, itemCode);

        if (itemPosition != -1)
        {
            RemoveItemAtPosition(inventoryList, itemCode, itemPosition);
        }

        // 发送事件，库存已更新
        EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);

    }

    private void RemoveItemAtPosition(List<InventoryItem> inventoryList, int itemCode, int position)
    {
        InventoryItem inventoryItem = new InventoryItem();

        int quantity = inventoryList[position].itemQuantity - 1;

        if (quantity > 0)
        {
            inventoryItem.itemQuantity = quantity;
            inventoryItem.itemCode = itemCode;
            inventoryList[position] = inventoryItem;
        }
        else
        {
            inventoryList.RemoveAt(position);
        }
    }

    /// <summary>
    /// 设置 inventoryLocation 的选定库存项为 itemCode
    /// </summary>
    public void SetSelectedInventoryItem(InventoryLocation inventoryLocation, int itemCode)
    {
        selectedInventoryItem[(int)inventoryLocation] = itemCode;
    }


    //private void DebugPrintInventoryList(List<InventoryItem> inventoryList)
    //{
    //    foreach (InventoryItem inventoryItem in inventoryList)
    //    {
    //        Debug.Log("Item Description:" + InventoryManager.Instance.GetItemDetails(inventoryItem.itemCode).itemDescription + "    Item Quantity: " + inventoryItem.itemQuantity);
    //    }
    //    Debug.Log("******************************************************************************");
    //}

}
