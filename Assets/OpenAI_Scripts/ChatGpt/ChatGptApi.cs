using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using UnityEngine.UI;
using Newtonsoft.Json;

public class ChatGptApi : MonoBehaviour
{

    [Header("OpenAI API Requirements : ")]
[Space(5)] 
    public string API_KEY = "MYKey";
    public const string API_URL = "https://api.openai.com/v1/chat/completions";
    public const string Model = "gpt-3.5-turbo";  
    [SerializeField]
    private string prompt = "";
    public int MaxTokens;
    public float Temperature;

[Space(10)] 
    [Header("UI : ")]
[Space(5)]
    public Text errorText;
    public Text userResponseText;
    public Text botResponseText;
    public InputField messageInput; 

[Space(10)]   
    [Header("Request Time : ")]
[Space(5)] 
    public int timeBetweenRequests; //time in seconds
    private float lastRequestTime;

private void Start(){
        lastRequestTime = Time.time;
       errorText.enabled = false;
    }

 public void SendRequest(){
    
        prompt = messageInput.text;
        //play animation
        StartCoroutine(SendRequestToChatGpt());
        //wait for response
        //stop animation

    }
private IEnumerator SendRequestToChatGpt(){

    // Wait for the time between requests to pass
    if (Time.time - lastRequestTime < timeBetweenRequests)
    {
        yield return new WaitForSeconds(timeBetweenRequests);
    }

    // Create the request
    UnityWebRequest request = new UnityWebRequest("https://api.openai.com/v1/chat/completions", "POST");
    request.SetRequestHeader("Content-Type", "application/json");
    request.SetRequestHeader("Authorization", "Bearer " + API_KEY);

    // Create the request body
    var requestBody = new
    {
        model = Model,
        messages = new[]
        {
            new { role = "user", content = prompt }
        },
        temperature = Temperature,
        max_tokens = MaxTokens
    };

    string jsonRequestBody = JsonConvert.SerializeObject(requestBody);
    byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonRequestBody);

    request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
    request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

    // Send the request and wait for the response
    yield return request.SendWebRequest();

    // Check for errors
    if (request.result == UnityWebRequest.Result.ConnectionError ||
        request.result == UnityWebRequest.Result.ProtocolError ||
        request.result == UnityWebRequest.Result.DataProcessingError)
    {
        Debug.LogError(request.error);
        errorText.text = request.error;
        errorText.enabled = true;
    }
    else
    {
        // Parse the response to get the text
        string responseJson = request.downloadHandler.text;
        ResponseData responseData = JsonConvert.DeserializeObject<ResponseData>(responseJson);
        string text = responseData.choices[0].message.content;  

        // Update the UI
        userResponseText.text = prompt;
        botResponseText.text = text;

        // Create a JSON object with the "role" and "content" keys
        string json = "{ \"messages\": [";
        json += "{\"role\": \"user\", \"content\": \"" + prompt + "\"},";
        json += "{\"role\": \"bot\", \"content\": \"" + text + "\"}";
        json += "] }";

        // Print the JSON object
        Debug.Log(json);
        Debug.Log("Response JSON: " + responseJson);
       
    }

    lastRequestTime = Time.time;
}

    
}
