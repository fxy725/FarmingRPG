using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "so_AnimationType", menuName = "Scriptable Objects/Animation/Animation Type")]
public class SO_AnimationType : ScriptableObject
{

    public AnimationClip animationClip; // 存放对anim资源的引用
    public AnimationName animationName; // 枚举类型字段，表示动画名称
    public CharacterPartAnimator characterPart; // 枚举类型字段，表示要参与动画的角色部位
    public PartVariantColor partVariantColour; // 枚举类型字段，表示角色部件的颜色
    public PartVariantType partVariantType; // 枚举类型字段，表示角色部件的类型
}

// 该类定义用于存储动画类型相关数据的ScriptableObject
