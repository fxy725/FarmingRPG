using System.Collections.Generic;
using UnityEngine;

public class UIInventoryBar : MonoBehaviour
{
    [SerializeField] private Sprite blank16x16sprite;

    private RectTransform rectTransform;

    [SerializeField] private UIInventorySlot[] inventorySlots;

    public GameObject inventoryBarDraggedItem;
    [HideInInspector] public GameObject inventoryTextBoxGameobject;

    private bool _isInventoryBarPositionBottom = true;
    public bool IsInventoryBarPositionBottom { get => _isInventoryBarPositionBottom; set => _isInventoryBarPositionBottom = value; }




    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        EventHandler.InventoryUpdatedEvent += InventoryUpdated;
    }

    private void OnDisable()
    {
        EventHandler.InventoryUpdatedEvent -= InventoryUpdated;
    }

    private void Update()
    {
        // 根据玩家位置切换库存栏位置
        SwitchInventoryBarPosition();
    }

    /// <summary>
    /// 清除库存栏所有高亮
    /// </summary>
    public void ClearHighlightOnInventorySlots()
    {
        if (inventorySlots.Length > 0)
        {
            // 遍历库存栏并清除高亮
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                if (inventorySlots[i].isSelected)
                {
                    inventorySlots[i].isSelected = false;
                    inventorySlots[i].inventorySlotHighlight.color = new Color(0f, 0f, 0f, 0f);
                    // 更新库存以显示项目未选中
                    InventoryManager.Instance.ClearSelectedInventoryItem(InventoryLocation.player);
                }
            }
        }
    }


    private void ClearInventorySlots()
    {
        if (inventorySlots.Length > 0)
        {
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                inventorySlots[i].inventorySlotImage.sprite = blank16x16sprite;
                inventorySlots[i].textMeshProUGUI.text = "";
                inventorySlots[i].itemDetails = null;
                inventorySlots[i].itemQuantity = 0;
                SetHighlightedInventorySlots(i);
            }
        }
    }


    private void InventoryUpdated(InventoryLocation inventoryLocation, List<InventoryItem> inventoryList)
    {
        if (inventoryLocation == InventoryLocation.player)
        {
            ClearInventorySlots();

            if (inventorySlots.Length > 0 && inventoryList.Count > 0)
            {
                // 遍历库存栏并更新对应库存列表项
                for (int i = 0; i < inventorySlots.Length; i++)
                {
                    if (i < inventoryList.Count)
                    {
                        int itemCode = inventoryList[i].itemCode;

                        // ItemDetails itemDetails = InventoryManager.Instance.itemList.itemDetails.Find(x => x.itemCode == itemCode);
                        ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(itemCode);

                        if (itemDetails != null)
                        {
                            // 添加图像和详细信息到库存项槽
                            inventorySlots[i].inventorySlotImage.sprite = itemDetails.itemSprite;
                            inventorySlots[i].textMeshProUGUI.text = inventoryList[i].itemQuantity.ToString();
                            inventorySlots[i].itemDetails = itemDetails;
                            inventorySlots[i].itemQuantity = inventoryList[i].itemQuantity;
                            SetHighlightedInventorySlots(i);

                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }


    /// <summary>
    ///  设置所有库存项目位置上的选中高亮
    /// </summary>
    public void SetHighlightedInventorySlots()
    {
        if (inventorySlots.Length > 0)
        {
            // 遍历库存栏并清除高亮
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                SetHighlightedInventorySlots(i);
            }
        }
    }

    /// <summary>
    ///  设置给定库存项目位置上的选中高亮
    /// </summary>
    public void SetHighlightedInventorySlots(int itemPosition)
    {
        if (inventorySlots.Length > 0 && inventorySlots[itemPosition].itemDetails != null)
        {
            if (inventorySlots[itemPosition].isSelected)
            {
                inventorySlots[itemPosition].inventorySlotHighlight.color = new Color(1f, 1f, 1f, 1f);

                // 更新库存以显示项目已选中
                InventoryManager.Instance.SetSelectedInventoryItem(InventoryLocation.player, inventorySlots[itemPosition].itemDetails.itemCode);
            }
        }
    }


    private void SwitchInventoryBarPosition()
    {
        Vector3 playerViewportPosition = Player.Instance.GetPlayerViewportPosition();

        if (playerViewportPosition.y > 0.3f && IsInventoryBarPositionBottom == false)
        {
            // transform.position = new Vector3(transform.position.x, 7.5f, 0f); // 这被改变为控制recttransform，见下面
            rectTransform.pivot = new Vector2(0.5f, 0f);
            rectTransform.anchorMin = new Vector2(0.5f, 0f);
            rectTransform.anchorMax = new Vector2(0.5f, 0f);
            rectTransform.anchoredPosition = new Vector2(0f, 2.5f);

            IsInventoryBarPositionBottom = true;
        }
        else if (playerViewportPosition.y <= 0.3f && IsInventoryBarPositionBottom == true)
        {
            //transform.position = new Vector3(transform.position.x, mainCamera.pixelHeight - 120f, 0f);// 这被改变为控制recttransform，见下面
            rectTransform.pivot = new Vector2(0.5f, 1f);
            rectTransform.anchorMin = new Vector2(0.5f, 1f);
            rectTransform.anchorMax = new Vector2(0.5f, 1f);
            rectTransform.anchoredPosition = new Vector2(0f, -2.5f);

            IsInventoryBarPositionBottom = false;
        }
    }

    public void DestroyCurrentlyDraggedItems()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].draggedItem != null)
            {
                Destroy(inventorySlots[i].draggedItem);
            }
        }
    }

    public void ClearCurrentlySelectedItems()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            inventorySlots[i].ClearSelectedItem();
        }
    }


}
