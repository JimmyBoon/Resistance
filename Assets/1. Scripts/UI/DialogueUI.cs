using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Dialogue;
using TMPro;
using UnityEngine.UI;

namespace RPG.UI
{
    public class DialogueUI : MonoBehaviour
    {
        PlayerConversant playerConversant;
        [SerializeField] TextMeshProUGUI AIText;
        [SerializeField] Button nextButton;
        [SerializeField] Button quitButton;
        [SerializeField] GameObject AIRespose;
        [SerializeField] Transform responsesRoot;
        [SerializeField] GameObject responsePrefab;
        [SerializeField] TextMeshProUGUI conversantName;


        void Start()
        {
            playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();
            nextButton.onClick.AddListener( () => playerConversant.Next());
            quitButton.onClick.AddListener( () => playerConversant.Quit());
            playerConversant.onConversationUpdated += UpdateUI;
            UpdateUI();
        }

        private void UpdateUI()
        {
            gameObject.SetActive(playerConversant.IsActive());
            if (!playerConversant.IsActive())
            { return; }

            conversantName.text = playerConversant.GetCurrentConversantName();
            AIRespose.SetActive(!playerConversant.IsChoosing());
            responsesRoot.gameObject.SetActive(playerConversant.IsChoosing());

            if (playerConversant.IsChoosing())
            {
                BuildResponseList();
            }
            else
            {
                AIText.text = playerConversant.GetText();
                nextButton.gameObject.SetActive(playerConversant.HasNext());
            }


        }

        private void BuildResponseList()
        {
            foreach (Transform item in responsesRoot)
            {
                Destroy(item.gameObject);
            }

            foreach (DialogueNode response in playerConversant.GetResponses())
            {
                GameObject responseInstance = Instantiate(responsePrefab, responsesRoot);
                responseInstance.GetComponentInChildren<TextMeshProUGUI>().text = response.GetText();
                Button responseButton = responseInstance.GetComponent<Button>();
                responseButton.onClick.AddListener(() =>
                {
                    playerConversant.SelectResponse(response);
                    //UpdateUI();
                });
            }
        }
    }
}
