using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;

namespace RPG.Inventories
{
    /// <summary>
    /// To be placed on anything that wishes to drop pickups into the world.
    /// Tracks the drops for saving and restoring.
    /// </summary>
    public class ItemDropper : MonoBehaviour, ISaveable
    {
        // STATE
        private List<Pickup> droppedItems = new List<Pickup>();

        // PUBLIC

        /// <summary>
        /// Create a pickup at the current position.
        /// </summary>
        /// <param name="item">The item type for the pickup.</param>
        public void DropItem(InventoryItem item, int numberOfItems)
        {
            SpawnPickup(item, numberOfItems, GetDropLocation());
        }

        // PROTECTED

        /// <summary>
        /// Override to set a custom method for locating a drop.
        /// </summary>
        /// <returns>The location the drop should be spawned.</returns>
        protected virtual Vector3 GetDropLocation()
        {
            return transform.position;
        }

        // PRIVATE

        public void SpawnPickup(InventoryItem item, int numberOfItems, Vector3 spawnLocation)
        {
            var pickup = item.SpawnPickup(spawnLocation, numberOfItems);
            droppedItems.Add(pickup);
        }

        [System.Serializable]
        private struct DropRecord
        {
            public string itemID;
            public SerializableVector3 position;
            public int number;
        }

        public object CaptureState()
        {
            RemoveDestroyedDrops();
            var droppedItemsList = new DropRecord[droppedItems.Count];
            for (int i = 0; i < droppedItemsList.Length; i++)
            {
                droppedItemsList[i].itemID = droppedItems[i].GetItem().GetItemID();
                droppedItemsList[i].position = new SerializableVector3(droppedItems[i].transform.position);
                droppedItemsList[i].number = droppedItems[i].GetNumberOfItems();
            }
            return droppedItemsList;
        }

        public void RestoreState(object state)
        {
            RemoveDestroyedDrops();
            var droppedItemsList = (DropRecord[])state;
            foreach (var item in droppedItemsList)
            {
                var pickupItem = InventoryItem.GetFromID(item.itemID);
                Vector3 position = item.position.ToVector();
                int numberOfItems = item.number;
                SpawnPickup(pickupItem, numberOfItems, position);
            }
        }

        /// <summary>
        /// Remove any drops in the world that have subsequently been picked up.
        /// </summary>
        private void RemoveDestroyedDrops()
        {
            var newList = new List<Pickup>();
            foreach (var item in droppedItems)
            {
                if (item != null)
                {
                    newList.Add(item);
                }
            }
            droppedItems = newList;
        }
    }
}
