using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Item item = collision.GetComponent<Item>();

        if (item != null)
        {
            // 获取物品详情
            ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(item.ItemCode);

            // 如果物品可以被拾取
            if (itemDetails.canBePickedUp == true)
            {
                // 添加物品到库存
                InventoryManager.Instance.AddItem(InventoryLocation.player, item, collision.gameObject);

                // 播放拾取声音
                AudioManager.Instance.PlaySound(SoundName.effectPickupSound);

            }
        }
    }
}