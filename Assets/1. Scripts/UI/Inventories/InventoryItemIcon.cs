using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.Inventories;
using TMPro;

namespace RPG.UI.Inventories
{

    [RequireComponent(typeof(Image))]
    public class InventoryItemIcon : MonoBehaviour
    {

        [SerializeField] GameObject textContainer = null;
        [SerializeField] TextMeshProUGUI itemNumber = null;


        public void SetItem(InventoryItem item, int number)
        {
            var iconImage = GetComponent<Image>();
            if (item == null)
            {
                iconImage.enabled = false;
            }
            else
            {
                iconImage.enabled = true;
                iconImage.sprite = item.GetIcon();

            }

            if (itemNumber)
            {
                if (number > 1)
                {
                    textContainer.SetActive(true);
                    itemNumber.text = number.ToString();
                }
                else
                {
                    textContainer.SetActive(false);
                }
            }

        }
    }
}