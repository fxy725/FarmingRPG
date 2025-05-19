
using UnityEngine;

// 单例(Monobehaviour版本)的抽象基类，其他单例，管理器等都应该派生自该类
public abstract class SingletonMonobehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance; // 私有静态字段存放类的当前实例

    public static T Instance // 公共静态属性提供访问实例的接口
    {
        get
        {
            return instance;
        }
    }

    protected virtual void Awake() // 脚本实例化后且脚本被启用时首先调用Awake方法，该方法为受保护的虚拟方法，可以被派生类重写与调用
    {
        if (instance == null)
        {
            instance = this as T; // 将this的类型由SingletonMonobehaviour<T>转换为T类型
        }
        else
        {
            Destroy(gameObject); // 如果实例已经存在，销毁当前脚本挂载的游戏对象及其关联组件
        }
    }
}
