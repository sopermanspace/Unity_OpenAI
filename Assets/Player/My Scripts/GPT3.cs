using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class GPT3 : MonoBehaviour
{
    public string apiKey = "OpenAI API Key";
    // public string model;
     private const string API_URL = "https://api.openai.com/v1/completions";
    public string prompt="Hello, how are you today?";
   public int maxTokens = 2048;
    public float temperature = 0.7f;

    void Start()
    {
        StartCoroutine(SendGpt3Request());
    }

   IEnumerator SendGpt3Request()
{
     UnityWebRequest request = new UnityWebRequest(API_URL, "POST");
       byte[] bodyRaw = Encoding.UTF8.GetBytes("{\"model\": \"text-curie-001\", \"prompt\": \"" + prompt + "\", \"max_tokens\": " + maxTokens + ", \"temperature\": " + temperature + "}");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);
 
    // Send the request and wait for the response
    yield return request.SendWebRequest();

    // Check for errors
    if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.DataProcessingError)
    {
        Debug.LogError(request.error);
    }
    else
    {
        // Get the response text
        string responseText = request.downloadHandler.text;
        Debug.Log(responseText);
    }
}

}//class
