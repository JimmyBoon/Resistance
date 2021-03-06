using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;

namespace RPG.SceneManagement

{

    public class SavingWrapper : MonoBehaviour
    {
        const string defaultSaveFile = "save";
        [SerializeField] float fadeInTime = 2f;
        [SerializeField] float fadeOutTime = 0.5f;
        

        private void Awake() 
        {
            //StartCoroutine(LoadLastScene());
            
        }

        private IEnumerator Start() 
        {
            
            yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile);
            FindObjectOfType<Fader>().FadeOutImmediate();
            yield return FindObjectOfType<Fader>().FadeIn(fadeInTime);
            
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                Delete();
            }
        }

        public void Load()
        {
            
            GetComponent<SavingSystem>().Load(defaultSaveFile);
            
        }

        public void Save()
        {
            GetComponent<SavingSystem>().Save(defaultSaveFile);
        }

        public void Delete() 
        {
            GetComponent<SavingSystem>().Delete(defaultSaveFile);
        }
    }
}


