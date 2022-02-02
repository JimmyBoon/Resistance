using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using UnityEngine;

namespace RPG.Core

{
    public class Noise : MonoBehaviour
    {
        [SerializeField] float noiseOutput = 0f;



        private void Update()
        {
            UpdateNoiseOutput();
        }

        public void UpdateNoiseOutput()
        {


            float maxNoiseOutput = 0;
            NoiseInstance maxNoiseInstance = new NoiseInstance();

            foreach (INoise noise in GetComponents<INoise>())
            {
                if (maxNoiseOutput < noise.GetNoiseInstance().noiseAmount)
                {
                    maxNoiseOutput = noise.GetNoiseInstance().noiseAmount;
                    maxNoiseInstance = noise.GetNoiseInstance();
                }
            }

            noiseOutput = maxNoiseOutput;

            RaycastHit[] hits = Physics.SphereCastAll(transform.position, noiseOutput, Vector3.up, 0);
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.GetComponent<AISenses>())
                {
                    hit.transform.GetComponent<AISenses>().SoundWasHeard(maxNoiseInstance);
                }
            }

        }


        public float GetNoiseOutput()
        {
            return noiseOutput;
        }

    }
}