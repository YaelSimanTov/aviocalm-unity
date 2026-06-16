using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using SocketIOClient;
using System;
using UnityEngine.SceneManagement;

[Serializable]
public class CalibrationPayload
{
    public int durationSeconds { get; set; }
    public string startMessage { get; set; }
    public string endMessage { get; set; }
}

public class CalibrationController : MonoBehaviour
{
    public TextMeshProUGUI statusText;
    public Image progressRing;
    public CanvasGroup fadeGroup;
    public float fadeDuration = 4f;

    private Queue<Action> mainThreadActions = new Queue<Action>();
    private string finalMessage = "";

    System.Collections.IEnumerator Start()
    {
        // Wait until SocketManager is initialized and the socket is fully connected
        while (SocketManager.instance == null || SocketManager.instance.socket == null || !SocketManager.instance.socket.Connected)
        {
            yield return new WaitForSeconds(0.1f);
        }

        SocketManager.instance.socket.On("CALIBRATION_SETUP", (response) =>
        {
            try
            {
                CalibrationPayload data = response.GetValue<CalibrationPayload>(0);
                mainThreadActions.Enqueue(() =>
                {
                    ExecuteCalibration(data.durationSeconds, data.startMessage, data.endMessage);
                });
            }
            catch (Exception e)
            {
                // Tagged as Dev to prevent cluttering the clinical timeline
                Debug.LogError("[Dev] Error parsing server response: " + e.Message);
            }
        });

        // Wait exactly 15 seconds to collect a stable heart rate reading before requesting
        yield return new WaitForSeconds(15f);

        // Request the calibration parameters from the server
        SocketManager.instance.SendEvent("CALIBRATION_START", "");
    }

    void Update()
    {
        while (mainThreadActions.Count > 0)
        {
            mainThreadActions.Dequeue().Invoke();
        }
    }

    private void ExecuteCalibration(int duration, string startMessage, string endMessage)
    {
        // Clinical log: Marks the exact time the baseline calculation begins
        Debug.Log($"[System Event] Heart rate calibration started. Target duration: {duration} seconds.");

        // Save the ending message for later use
        finalMessage = endMessage;

        // Display start message and ensure it is fully opaque
        statusText.text = startMessage;
        statusText.alpha = 1f;

        // Start the UI fade out after 7 seconds
        StartCoroutine(FadeOutTextAfter(7f));

        // Start the progress ring visual animation
        StartCoroutine(FillProgressRing(duration));
    }

    private System.Collections.IEnumerator FadeOutTextAfter(float delay)
    {
        yield return new WaitForSeconds(delay);

        float elapsedTime = 0f;
        float fadeTime = 2f;

        // Lerp the alpha of the TextMeshPro component to 0
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            statusText.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeTime);
            yield return null;
        }

        statusText.alpha = 0f;
    }

    private System.Collections.IEnumerator FillProgressRing(int totalSeconds)
    {
        float elapsedTime = 0f;

        while (elapsedTime < totalSeconds)
        {
            elapsedTime += Time.deltaTime;
            progressRing.fillAmount = elapsedTime / totalSeconds;
            yield return null;
        }

        // Calibration time is fully up - handle completion UI and network events
        statusText.text = finalMessage;
        statusText.alpha = 1f; // Show the text again

        // Inform Node.js so it can average the accumulated HR data and save to DB
        SocketManager.instance.SendEvent("CALIBRATION_END", "");

        // Clinical log: Marks the completion of the baseline phase
        Debug.Log("[System Event] Heart rate calibration completed successfully.");

        // Let the user read the end message for 4 seconds before transitioning
        yield return new WaitForSeconds(4f);

        TransitionToFlightScene();
    }

    private void TransitionToFlightScene()
    {
        // Tagged for the timeline to show when the user moves to the next active phase
        Debug.Log("[System Event] Transitioning from Calibration to Tutorial Scene.");
        StartCoroutine(FadeOutAndLoadScene());
    }

    private System.Collections.IEnumerator FadeOutAndLoadScene()
    {
        float elapsedTime = 0f;
        float startVolume = AudioListener.volume;

        if (fadeGroup != null) fadeGroup.gameObject.SetActive(true);

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);
            float smoothAlpha = Mathf.SmoothStep(0f, 1f, t);

            if (fadeGroup != null)
            {
                fadeGroup.alpha = smoothAlpha;
            }

            AudioListener.volume = Mathf.Lerp(startVolume, 0f, t);
            yield return null;
        }

        if (fadeGroup != null) fadeGroup.alpha = 1f;
        AudioListener.volume = 0f;

        yield return new WaitForSeconds(4.0f);
        SceneManager.LoadScene("TutorialScene");
    }
}