using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {

        [SerializeField] string playerName = "Player";

        Dialogue currentDialogue;
        DialogueNode currentNode = null;
        AIConversant currentConversant = null;
        bool isChoosing = false;

        public event Action onConversationUpdated;

        public bool IsActive()
        {
            return currentDialogue != null;
        }

        public void StartDialogue(AIConversant newConversant, Dialogue newDialogue)
        {
            currentDialogue = newDialogue;
            currentConversant = newConversant;
            currentNode = currentDialogue.GetRootNode();
            TriggerEnterAction();
            onConversationUpdated();
        }

        public void Quit()
        {
            TriggerExitAction();
            currentConversant = null;
            currentDialogue = null;

            currentNode = null;
            isChoosing = false;
            onConversationUpdated();
        }

        public bool IsChoosing()
        {
            return isChoosing;
        }


        public string GetText()
        {
            if (currentNode == null)
            {
                Debug.LogError("Current Dialogue is not set on player");
                return "";
            }
            return currentNode.GetText();
        }

        public IEnumerable<DialogueNode> GetResponses()
        {
            return currentDialogue.GetPlayerChildern(currentNode);
        }

        public void SelectResponse(DialogueNode responseNode)
        {
            currentNode = responseNode;
            TriggerEnterAction();
            isChoosing = false;
            Next();
        }

        public void Next()
        {
            int numPlayerResponses = currentDialogue.GetPlayerChildern(currentNode).Count();
            if (numPlayerResponses > 0)
            {
                isChoosing = true;
                TriggerExitAction();
                onConversationUpdated();
                return;
            }

            DialogueNode[] childern = currentDialogue.GetAIChildern(currentNode).ToArray();
            int randomChild = UnityEngine.Random.Range(0, childern.Length);
            TriggerExitAction();
            currentNode = childern[randomChild];
            TriggerEnterAction();
            onConversationUpdated();
        }

        public bool HasNext()
        {
            DialogueNode[] childern = currentDialogue.GetAllChildern(currentNode).ToArray();
            if (childern.Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public string GetCurrentConversantName()
        {
            if(isChoosing)
            {
                return playerName;
            }
            else
            {
                return currentConversant.GetConversantName();
            }
        }

        private void TriggerEnterAction()
        {
            if (currentNode != null)
            {
                TriggerAction(currentNode.GetOnEnterAction());
            }
        }

        private void TriggerExitAction()
        {
            if (currentNode != null)
            {
                TriggerAction(currentNode.GetOnExitAction());
            }
        }

        private void TriggerAction(string action)
        {
            if (action == "") { return; }

            foreach (DialogueTrigger trigger in currentConversant.GetComponents<DialogueTrigger>())
            {
                trigger.Trigger(action);
            }
        }

    }
}
