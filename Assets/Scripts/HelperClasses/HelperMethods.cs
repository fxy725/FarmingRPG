using System.Collections.Generic;
using UnityEngine;

public static class HelperMethods
{

    // 在要检查的位置获取类型为T的组件。返回true表示至少找到一个组件，并将找到的组件返回在componentAtPositionList中
    public static bool GetComponentsAtCursorLocation<T>(out List<T> componentsAtPositionList, Vector3 positionToCheck)
    {
        bool found = false;

        List<T> componentList = new List<T>();

        Collider2D[] collider2DArray = Physics2D.OverlapPointAll(positionToCheck);

        // 遍历所有碰撞器以获取类型为T的对象

        T tComponent = default;

        for (int i = 0; i < collider2DArray.Length; i++)
        {
            tComponent = collider2DArray[i].gameObject.GetComponentInParent<T>();
            if (tComponent != null)
            {
                found = true;
                componentList.Add(tComponent);
            }
            else
            {
                tComponent = collider2DArray[i].gameObject.GetComponentInChildren<T>();
                if (tComponent != null)
                {
                    found = true;
                    componentList.Add(tComponent);
                }
            }
        }

        componentsAtPositionList = componentList;

        return found;
    }



    // 在指定的中心点、尺寸和角度的盒子中获取类型为T的组件。返回true表示至少找到一个组件，并将找到的组件返回在list中
    public static bool GetComponentsAtBoxLocation<T>(out List<T> listComponentsAtBoxPosition, Vector2 point, Vector2 size, float angle)
    {
        bool found = false;
        List<T> componentList = new List<T>();

        Collider2D[] collider2DArray = Physics2D.OverlapBoxAll(point, size, angle);

        // 遍历所有碰撞器以获取类型为T的对象
        for (int i = 0; i < collider2DArray.Length; i++)
        {
            T tComponent = collider2DArray[i].gameObject.GetComponentInParent<T>();
            if (tComponent != null)
            {
                found = true;
                componentList.Add(tComponent);
            }
            else
            {
                tComponent = collider2DArray[i].gameObject.GetComponentInChildren<T>();
                if (tComponent != null)
                {
                    found = true;
                    componentList.Add(tComponent);
                }
            }
        }

        listComponentsAtBoxPosition = componentList;

        return found;
    }

    // 返回指定中心点、尺寸和角度的盒子中类型为T的组件数组。numberOfCollidersToTest作为参数传递。找到的组件将返回在数组中。
    public static T[] GetComponentsAtBoxLocationNonAlloc<T>(int numberOfCollidersToTest, Vector2 point, Vector2 size, float angle)
    {
        // 声明并初始化一个长度为numberOfCollidersToTest的Collider2D数组，数组中的每个元素初始值为null
        Collider2D[] collider2DArray = new Collider2D[numberOfCollidersToTest];

        // 使用Physics2D.OverlapBoxAll方法填充collider2DArray数组，返回的Collider2D数组将覆盖collider2DArray
        collider2DArray = Physics2D.OverlapBoxAll(point, size, angle);

        // 声明T类型的变量tComponent，并使用default初始化为默认值
        T tComponent = default;
        //T tComponent = default(T);

        T[] componentArray = new T[collider2DArray.Length];

        for (int i = 0; i < collider2DArray.Length; i++)
        {
            if (collider2DArray[i] != null)
            {
                tComponent = collider2DArray[i].gameObject.GetComponent<T>();

                if (tComponent != null)
                {
                    componentArray[i] = tComponent;
                }
            }
        }

        return componentArray;
    }

}
