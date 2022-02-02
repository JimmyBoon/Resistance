
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using RPG.Inventories;
using RPG.Utils.UI.Dragging;

namespace RPG.UI.Inventories
{
    public class InventorySlotUI : MonoBehaviour, IDragContainer<InventoryItem>, IItemHolder
    {
        // CONFIG DATA
        [SerializeField] InventoryItemIcon icon = null;
        

        // STATE
        int index;
        RPG.Inventories.Inventory inventory;

        // PUBLIC

        public void Setup(RPG.Inventories.Inventory inventory, int index)
        {
            this.inventory = inventory;
            this.index = index;
            icon.SetItem(inventory.GetItemInSlot(index), inventory.GetNumberOfItemsInSlot(index));
        }

        public int MaxAcceptable(InventoryItem item)
        {
            if (inventory.HasSpaceFor(item))
            {
                return int.MaxValue;
            }
            return 0;
        }

        public void AddItems(InventoryItem item, int number)
        {
            inventory.AddItemToSlot(index, item, number);
        }

        public InventoryItem GetItem()
        {
            return inventory.GetItemInSlot(index);
        }

        public int GetNumber()
        {
            return inventory.GetNumberOfItemsInSlot(index);
        }

        public void RemoveItems(int number)
        {
            inventory.RemoveFromSlot(index, number);
        }
    }
}
