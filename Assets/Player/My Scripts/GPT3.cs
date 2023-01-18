using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using TMPro;
using Newtonsoft.Json;
public class GPT3 : MonoBehaviour
{
     [Header("OpenAI API")]
    public string apiKey ="MyKEY";
    public const string API_URL = "https://api.openai.com/v1/completions";
     public int timeBetweenRequests = 5; // time in seconds
    private float lastRequestTime;
    public string prompt="Hello, how are you today?";
    public string  model = "text-davinci-002";
   public int maxTokens = 2048;
    public float temperature = 0.7f;

    [Header("UI")]

[Space(10)]
    public TextMeshProUGUI ErrorText;
    public TextMeshProUGUI ResultText;
 
    void Start()
    {
         lastRequestTime = Time.time;
        StartCoroutine(SendGpt3Request());
        
    }
 
   IEnumerator SendGpt3Request()
{
    // Check if the API key is set
     if (string.IsNullOrEmpty(apiKey)) {
    Debug.LogError("API key is not set. Please provide a valid API key.");
    ErrorText.text = "API key is not set. Please provide a valid API key.";
    yield break;
    }
    if(string.IsNullOrEmpty(prompt))
    {
        Debug.LogError("Prompt is not set. Please provide a valid prompt.");
        ErrorText.text = "Prompt is not set. Please provide a valid prompt.";
        yield break;
    }
 if(maxTokens < 1)
    {
        Debug.LogError("Max Tokens is not set. Please provide a valid Max Tokens.");
        ErrorText.text = "Max Tokens is not set. Please provide a valid Max Tokens.";
        yield break;
    }
    
        // check if the time between requests has passed
        if (Time.time - lastRequestTime < timeBetweenRequests)
        {
            yield return new WaitForSeconds(timeBetweenRequests);
        }


    // Create a new request
     UnityWebRequest request = new UnityWebRequest(API_URL, "POST");
     byte[] bodyRaw = Encoding.UTF8.GetBytes("{\"model\":\"text-davinci-002\", \"prompt\":\"" + prompt + "\", \"max_tokens\":" + maxTokens + ", \"temperature\":" + temperature + "}");
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
        ErrorText.text = request.error;
        // ErrorText.text = request.error.ToString();
    }
    else
    {
        // Get the response text
        string responseText = request.downloadHandler.text;
        Debug.Log(responseText);
        ResultText.text = responseText.ToString();
    
    }
      lastRequestTime = Time.time;
}


}//class
