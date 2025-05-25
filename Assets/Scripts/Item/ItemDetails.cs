
using UnityEngine;

[System.Serializable]
public class ItemDetails
{
    public int itemCode; // 物品代码
    public ItemType itemType; // 物品类型
    public string itemDescription; // 物品描述
    public Sprite itemSprite; // 物品图片
    public string itemLongDescription; // 物品长描述
    public short itemUseGridRadius; // 物品使用网格半径
    public float itemUseRadius; // 物品使用半径
    public bool isStartingItem; // 是否为起始物品
    public bool canBePickedUp; // 是否可以被拾取
    public bool canBeDropped; // 是否可以被丢弃
    public bool canBeEaten; // 是否可以被食用
    public bool canBeCarried; // 是否可以被携带
}
