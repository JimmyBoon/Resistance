using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue", order = 0)]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] List<DialogueNode> nodes = new List<DialogueNode>();
        [SerializeField] Vector2 newNodeOffset = new Vector2(200,50);
        Dictionary<string, DialogueNode> nodeLookup = new Dictionary<string, DialogueNode>();

        private void OnValidate()
        {
            nodeLookup.Clear();
            foreach (DialogueNode node in GetAllNodes())
            {
                nodeLookup[node.name] = node;
            }
            if (nodeLookup.Count == 0)
            {
                CreateNode(null);
            }
        }

        public IEnumerable<DialogueNode> GetAllNodes()
        {
            return nodes;
        }

        public DialogueNode GetRootNode()
        {
            return nodes[0];
        }

        public IEnumerable<DialogueNode> GetAllChildern(DialogueNode parentNode)
        {
            //List<DialogueNode> result = new List<DialogueNode>(); //replaced by yeild return
            foreach (string childID in parentNode.GetChildern())
            {
                if (nodeLookup.ContainsKey(childID))
                {
                    yield return nodeLookup[childID]; // This does the same thing as the list, however it will return a single value, then continue the loop to look for other values.

                    //result.Add(nodeLookup[childID]); //replaced by yeild return
                }
            }
            //return result; //replaced by yeild return.
        }

        public IEnumerable<DialogueNode> GetPlayerChildern(DialogueNode currentNode)
        {
            foreach (DialogueNode node in GetAllChildern(currentNode))
            {
                if (node.IsPlayerSpeaking())
                {
                    yield return node;
                }

            }

        }

        public IEnumerable<DialogueNode> GetAIChildern(DialogueNode currentNode)
        {
            foreach (DialogueNode node in GetAllChildern(currentNode))
            {
                if (!node.IsPlayerSpeaking())
                {
                    yield return node;
                }

            }

        }

#if UNITY_EDITOR

        public void CreateNode(DialogueNode parentNode)
        {
            DialogueNode newNode = MakeNode(parentNode);

            Undo.RegisterCreatedObjectUndo(newNode, "Created Dialogue node");

            Undo.RecordObject(this, "Added Dialogue Node");

            AddNode(newNode);
        }





        public void DeleteNode(DialogueNode nodeToDelete)
        {
            Undo.RecordObject(this, "Deleted Dialogue Node");
            nodes.Remove(nodeToDelete);
            OnValidate();
            CleanDanglingChildern(nodeToDelete);
            Undo.DestroyObjectImmediate(nodeToDelete);
        }

        private void CleanDanglingChildern(DialogueNode nodeToDelete)
        {
            foreach (DialogueNode node in GetAllNodes())
            {
                node.RemoveChild(nodeToDelete.name);
            }
        }        
        
        private DialogueNode MakeNode(DialogueNode parentNode)
        {
            DialogueNode newNode = CreateInstance<DialogueNode>();
            newNode.name = System.Guid.NewGuid().ToString();


            if (parentNode != null)
            {
                parentNode.AddChild(newNode.name);
                newNode.SetIsPlayerSpeaking(!parentNode.IsPlayerSpeaking());
                newNode.SetPosition(parentNode.GetRect().position + newNodeOffset);
            }

            return newNode;
        }

        private void AddNode(DialogueNode newNode)
        {
            nodes.Add(newNode);
            OnValidate();
        }

#endif

        public void OnBeforeSerialize()
        {
            if(nodes.Count == 0)
            {
                DialogueNode newNode = MakeNode(null);
                AddNode(newNode);
            }


            if (AssetDatabase.GetAssetPath(this) != "")
            {
                foreach (DialogueNode node in GetAllNodes())
                {
                    if (AssetDatabase.GetAssetPath(node) == "")
                    {
                        AssetDatabase.AddObjectToAsset(node, this);
                    }

                }
            }
        }

#if UNITY_EDITOR
        public void OnAfterDeserialize()
        {
            //nothing needed here today.
        }
#endif        
    }
}

