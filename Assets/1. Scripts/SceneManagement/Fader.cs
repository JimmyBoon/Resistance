using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.SceneManagement
{

    public class Fader : MonoBehaviour
    {

        float deltaAlpha = 0;
        Coroutine currentlyActiveFade = null;
        CanvasGroup canvasGroup;

        private void Awake() 
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public Coroutine FadeOut(float fadeTime)
        {
            return Fade(1f, fadeTime);
        }


        public Coroutine FadeIn(float fadeTime)
        {
            return Fade(0f, fadeTime);
        }

        private Coroutine Fade(float target, float fadeTime)
        {
            if (currentlyActiveFade != null)
            {
                StopCoroutine(currentlyActiveFade);
            }
            currentlyActiveFade = StartCoroutine(FadeRoutine(target, fadeTime));
            return currentlyActiveFade;
        }

        private IEnumerator FadeRoutine (float target, float fadeTime)
        {
            while (!Mathf.Approximately(canvasGroup.alpha, target))
            {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, target, Time.deltaTime / fadeTime);

                yield return null;
            }
        }

        public void FadeOutImmediate()
        {
            canvasGroup.alpha = 1;
        }


    }
}
