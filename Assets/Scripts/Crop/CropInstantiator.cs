using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 将Crop预制件附加到作物上，以设置网格属性字典中的值
/// </summary>
public class CropInstantiator : MonoBehaviour
{
    private UnityEngine.Grid grid;

    [SerializeField] private int daysSinceDug = -1;
    [SerializeField] private int daysSinceWatered = -1;
    [ItemCodeDescription]
    [SerializeField] private int seedItemCode = 0;
    [SerializeField] private int growthDays = 0;




    private void OnEnable()
    {
        EventHandler.InstantiateCropPrefabsEvent += InstantiateCropPrefabs;
    }

    private void OnDisable()
    {
        EventHandler.InstantiateCropPrefabsEvent -= InstantiateCropPrefabs;
    }

    private void InstantiateCropPrefabs()
    {
        // 获取网格游戏对象
        grid = GameObject.FindFirstObjectByType<UnityEngine.Grid>();

        // 获取作物网格位置
        Vector3Int cropGridPosition = grid.WorldToCell(transform.position);

        // 设置作物网格属性
        SetCropGridProperties(cropGridPosition);

        // 销毁这个游戏对象
        Destroy(gameObject);
    }


    private void SetCropGridProperties(Vector3Int cropGridPosition)
    {
        if (seedItemCode > 0)
        {
            GridPropertyDetails gridPropertyDetails;

            gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cropGridPosition.x, cropGridPosition.y);

            if (gridPropertyDetails == null)
            {
                gridPropertyDetails = new GridPropertyDetails();
            }

            gridPropertyDetails.daysSinceDug = daysSinceDug;
            gridPropertyDetails.daysSinceWatered = daysSinceWatered;
            gridPropertyDetails.seedItemCode = seedItemCode;
            gridPropertyDetails.growthDays = growthDays;

            GridPropertiesManager.Instance.SetGridPropertyDetails(cropGridPosition.x, cropGridPosition.y, gridPropertyDetails);

        }
    }

}
