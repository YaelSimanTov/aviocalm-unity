using UnityEngine;
using UnityEngine.UI;

public class SceneFadeIn : MonoBehaviour
{
    public CanvasGroup fadeGroup;
    public float fadeDuration = 5f;
    public float targetVolume = 1f;

    System.Collections.IEnumerator Start()
    {
        // Tagged as a system event so the timeline shows exactly when a new visual phase begins
        Debug.Log("[System Event] Scene transition started (Fade-in).");
        
        // Wait one frame to ensure all objects are initialized
        yield return null;

        // 1. Ensure the scene starts completely pitch black and fully silent
        if (fadeGroup != null)
        {
            fadeGroup.alpha = 1f;
            fadeGroup.gameObject.SetActive(true);
        }
        AudioListener.volume = 0f;

        // 2. Begin the smooth transition into the scene
        StartCoroutine(FadeInRoutine());
    }
    
    private System.Collections.IEnumerator FadeInRoutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            // Calculate the curve
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);
            float smoothAlpha = Mathf.SmoothStep(1f, 0f, t);

            // Gradually reveal the environment
            if (fadeGroup != null)
            {
                fadeGroup.alpha = smoothAlpha;
            }

            // Gradually bring the music and ambient sounds back up
            AudioListener.volume = Mathf.Lerp(0f, targetVolume, t);

            yield return null;
        }

        // Clean up completely once the fade is done
        if (fadeGroup != null)
        {
            fadeGroup.alpha = 0f;
            fadeGroup.gameObject.SetActive(false);
        }

        AudioListener.volume = targetVolume;
        
        // Clinical log: Marks the exact moment the patient is fully exposed to the new visual/audio environment
        Debug.Log("[System Event] Scene fade-in complete. Patient is fully immersed.");
    }
}