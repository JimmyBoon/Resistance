using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using UnityEngine;

namespace RPG.Dialogue
{
    public class AIConversant : MonoBehaviour, IRaycastable
    {
        [SerializeField] Dialogue characterDialogue;
        [SerializeField] string characterName = "Hans von Generic Character";

        public CursorType GetCursorType()
        {
            return CursorType.Dialogue;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (characterDialogue == null)
            {
                return false;
            }
            if (Input.GetMouseButtonDown(0))
            {
                PlayerConversant playerConversant = callingController.GetComponent<PlayerConversant>();
                playerConversant.StartDialogue(this, characterDialogue);
            }
            return true;
        }

        public string GetConversantName(){
            return characterName;
        }


    }
}