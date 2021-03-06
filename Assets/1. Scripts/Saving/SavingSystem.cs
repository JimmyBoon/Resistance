using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;

namespace RPG.Saving
{
    public class SavingSystem : MonoBehaviour
    {

        public IEnumerator LoadLastScene(string saveFile)
        {
            Dictionary<string, object> state = LoadFile(saveFile);
            int buildIndex = SceneManager.GetActiveScene().buildIndex;
            if (state.ContainsKey("lastSceneBuildIndex"))
            {
                buildIndex = (int)state["lastSceneBuildIndex"];
            }
            yield return SceneManager.LoadSceneAsync(buildIndex);
            RestoreState(state);

        }

        public void Save(string saveFile)
        {
            Dictionary<string, object> state = LoadFile(saveFile);

            CaptureState(state);
            SaveFile(saveFile, state);

        }



        public void Load(string saveFile)
        {
            RestoreState(LoadFile(saveFile));
        }

        public void Delete(string saveFile)
        {
            
            string path = GetPathFromSaveFile(saveFile);
            print("Deleting: " + path);

            if (File.Exists(path))
            {
                File.Delete(path);
                print("Save file deleted: " + path);
            }
        }

        private string GetPathFromSaveFile(string saveFile)
        {
            string fileWithExtention = saveFile + ".sav";
            return Path.Combine(Application.persistentDataPath, fileWithExtention);

        }

        private void SaveFile(string saveFile, object state)
        {
            string path = GetPathFromSaveFile(saveFile);
            print("Saving to: " + path);

            using (FileStream stream = File.Open(path, FileMode.Create))
            {

                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, state);
            }
        }

        private void CaptureState(Dictionary<string, object> state)
        {

            foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {
                state[saveable.GetUniqueIdentifier()] = saveable.CaptureState();

            }

            state["lastSceneBuildIndex"] = SceneManager.GetActiveScene().buildIndex;

        }

        private Dictionary<string, object> LoadFile(string saveFile)
        {
            string path = GetPathFromSaveFile(saveFile);
            if (!File.Exists(path))
            {
                return new Dictionary<string, object>();
            }

            using (FileStream stream = File.Open(path, FileMode.Open))
            {

                BinaryFormatter formatter = new BinaryFormatter();
                return (Dictionary<string, object>)formatter.Deserialize(stream);
            }
        }

        private void RestoreState(Dictionary<string, object> state)
        {
            Dictionary<string, object> stateDict = state;
            foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {
                string id = saveable.GetUniqueIdentifier();
                if (state.ContainsKey(id))
                {
                    saveable.RestoreState(stateDict[id]);
                }


            }


        }

    }
}


