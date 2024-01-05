using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.IO;

public class TextToSpeech : MonoBehaviour
{
    private string apiKey = "YOUR_API_KEY"; 
    private string baseUrl = "https://api.openai.com/v1/audio/speech";
    private string model = "tts-1";
    private string voice = "alloy";
    private string inputText = "Hello World, This is a test to see the TTS of OpenAI!";
    private string audioFileName = "speech.mp3";

    private void Start()
    {
        StartCoroutine(GenerateSpeech());
    }

    private IEnumerator GenerateSpeech()
    {
       
        var payload = new
        {
            model = model,
            voice = voice,
            input = inputText
        };

        // Convert the payload to a JSON string.
        string jsonPayload = JsonUtility.ToJson(payload);

        using (UnityWebRequest www = new UnityWebRequest(baseUrl, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonPayload));
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Authorization", "Bearer " + apiKey);
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                // Saving the audio data as an MP3 file.
                File.WriteAllBytes(audioFileName, www.downloadHandler.data);
                Debug.Log("Audio file saved as: " + audioFileName);
            }
            else
            {
                Debug.LogError("Failed to generate speech: " + www.error);
            }
        }
    }
}
