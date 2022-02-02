using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI.Inventories

{
    public class ShowHideUIButton : MonoBehaviour
    {
        [SerializeField] GameObject uiContainer = null;

        public void ShowHideContainer()
        {
            uiContainer.SetActive(!uiContainer.activeSelf);
        }
    }
}