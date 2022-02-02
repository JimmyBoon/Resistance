using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using RPG.Core;
using UnityEngine;

namespace RPG.Control
{
    public class AISuspecion : MonoBehaviour
    {
        [SerializeField] CharacterType[] investigateCharaters;
        [SerializeField] HashSet<GameObject> targetCharacterList = new HashSet<GameObject>();
        [SerializeField] bool acceptFriendlyFire = true;

        private void OnEnable()
        {
            GetComponent<Health>().onTakeDamage += OnTakeDameage;
        }

        private void OnDisable()
        {
            GetComponent<Health>().onTakeDamage += OnTakeDameage;
        }

        private void OnTakeDameage(float damage, GameObject instigator)
        {
            if(!GetComponent<Health>().IsAlive())
            {
                return;
            }
            if (FriendlyFire(instigator))
            {
                return;
            };
            if (targetCharacterList.Count == 0)
            {
                targetCharacterList.Add(instigator);
            }
            foreach (GameObject attacker in targetCharacterList)
            {
                if (attacker == instigator)
                {
                    return;
                }
            }

            targetCharacterList.Add(instigator);
        }

        private bool FriendlyFire(GameObject instigator)
        {
            if (!acceptFriendlyFire)
            { return false; }

            if(instigator.GetComponent<Character>().GetCharacterType() == GetComponent<Character>().GetCharacterType())
            {
                return true;
            }
            
            return false;

        }

        public Character CheckThreatLevel(Character character)
        {
            foreach (GameObject attacker in targetCharacterList)
            {
                if (character.gameObject == attacker.gameObject)
                {
                    return character;
                }
            }

            foreach (CharacterType characterTypeToInvestigate in investigateCharaters)
            {
                if (character.GetCharacterType() == characterTypeToInvestigate && character.IsCharacterArmed())
                {
                    return character;
                }
            }
            return null;
        }

        public bool ShouldSoundBeInvestigated(NoiseInstance noise)
        {
            if (noise.noiseSource == null)
            {
                return false;
            }
            if (noise.noiseSource == this.gameObject)
            {
                return false;
            }

            if (noise.noiseType == NoiseType.Gunshot || noise.noiseType == NoiseType.Punching)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public void AddToTargetList(GameObject target)
        {
            targetCharacterList.Add(target);
            print(targetCharacterList.Count + " things on my list " + gameObject );
        }
    }
}

