// 需要保存的对象需要实现这个接口
// 该接口定义了保存和加载的基本方法
// 该接口的实现类需要提供唯一ID和保存数据的GameObjectSave对象
public interface ISaveable
{
    string ISaveableUniqueID { get; set; } // 唯一ID
    GameObjectSave GameObjectSave { get; set; } // 保存数据的GameObjectSave对象

    void SaveableRegister(); // 注册保存事件
    void SaveableDeregister(); // 注销保存事件

    GameObjectSave SaveData(); // 保存数据
    void LoadData(GameSave gameSave); // 加载数据

    void StoreScene(string sceneName); // 存储场景数据
    void RestoreScene(string sceneName); // 恢复场景数据
}