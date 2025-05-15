// 需要保存的对象需要实现这个接口
// 该接口定义了保存和加载的基本方法
// 该接口的实现类需要提供唯一ID和保存数据的GameObjectSave对象
public interface ISaveable
{
    string ISaveableUniqueID { get; set; } // 唯一ID
    GameObjectSave GameObjectSave { get; set; } // 保存数据的GameObjectSave对象

    void ISaveableRegister(); // 注册保存事件

    void ISaveableDeregister(); // 注销保存事件

    GameObjectSave ISaveableSave(); // 保存数据

    void ISaveableLoad(GameSave gameSave); // 加载数据

    void ISaveableStoreScene(string sceneName); // 存储场景数据

    void ISaveableRestoreScene(string sceneName); // 恢复场景数据
}