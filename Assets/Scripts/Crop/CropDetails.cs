using UnityEngine;

[System.Serializable]
public class CropDetails
{
    [ItemCodeDescription]
    public int seedItemCode;  // 这是对应种子的物品代码
    public int[] growthDays; // 每个生长阶段的生长天数
    public GameObject[] growthPrefab;// 生长阶段的预制件
    public Sprite[] growthSprite; // 生长精灵
    public Season[] seasons; // 生长季节
    public Sprite harvestedSprite; // 收获精灵
    [ItemCodeDescription]
    public int harvestedTransformItemCode; // 如果物品在收获时变成另一个物品，这个物品代码将被填充
    public bool hideCropBeforeHarvestedAnimation; // 如果作物在收获动画之前应该禁用
    public bool disableCropCollidersBeforeHarvestedAnimation;// 如果作物上的碰撞器应该禁用以避免收获动画影响任何其他游戏对象
    public bool isHarvestedAnimation; // 如果收获动画要在最终生长阶段的预制件上播放，则为true
    public bool isHarvestActionEffect = false; // 标志来确定是否有一个收获动作效果
    public bool spawnCropProducedAtPlayerPosition;
    public HarvestActionEffect harvestActionEffect; // 作物的收获动作效果
    public SoundName harvestSound; // 作物的收获声音

    [ItemCodeDescription]
    public int[] harvestToolItemCode; // 工具的物品代码数组，可以收获或0数组元素如果没有工具
    public int[] requiredHarvestActions; // 每个工具的收获动作数量
    [ItemCodeDescription]
    public int[] cropProducedItemCode; // 收获的作物生产的物品代码
    public int[] cropProducedMinQuantity; // 收获的作物生产的物品最小数量
    public int[] cropProducedMaxQuantity; // 如果最大数量大于最小数量，则生产


    /// <summary>
    /// 如果工具物品代码可以用来收获这个作物，则返回true，否则返回false
    /// </summary>
    public bool CanUseToolToHarvestCrop(int toolItemCode)
    {
        if (RequiredHarvestActionsForTool(toolItemCode) == -1)
        {
            return false;
        }
        else
        {
            return true;
        }

    }


    /// <summary>
    /// 如果工具不能用来收获这个作物，则返回-1，否则返回这个工具的收获动作数量
    /// </summary>
    public int RequiredHarvestActionsForTool(int toolItemCode)
    {
        for (int i = 0; i < harvestToolItemCode.Length; i++)
        {
            if (harvestToolItemCode[i] == toolItemCode)
            {
                return requiredHarvestActions[i];
            }
        }
        return -1;
    }

}

