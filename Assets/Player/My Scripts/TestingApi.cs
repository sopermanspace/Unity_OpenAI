using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json;

public class TestingApi : MonoBehaviour
{  // Replace YOUR_API_KEY with your actual API key
    private string apiKey = "OpenAI API Key";
    private string prompt = "Hello, how are you today?";
    private string model = "text-davinci-002";
    private string conversationId = "";

    void Start()
    {
        StartCoroutine(GetResponse());
    }

    IEnumerator GetResponse()
    {
        UnityWebRequest www = UnityWebRequest.Post("https://api.openai.com/v1/chatbot/chat", "");
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", "Bearer " + apiKey);

        // Set the POST data
        string postData = "{\"model\": \"" + model + "\", \"prompt\": \"" + prompt + "\", \"conversation_id\": \"" + conversationId + "\"}";
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(postData);
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Print the response from the ChatGPT API
            Debug.Log(www.downloadHandler.text);
        }
    }
}