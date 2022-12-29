using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;

public class APILoader : MonoBehaviour
{
   private string apiKey = "myAPI" ; // Your OpenAI API key
    private string chatGptModel ="text-davinci-003"; // The ID of the ChatGPT model to use
   [SerializeField] private string userName = "jao"; // The user's name
    private string botName = "Jarvis"; // The bot's name
    public string conversationId; // The ID of the conversation

    // UI elements for displaying the conversation
 [SerializeField]   public Text conversationText;
   [SerializeField]  public InputField messageInput;

    // A queue to store the messages that have not yet been sent to ChatGPT
    Queue<string> messageQueue = new Queue<string>();

    // A flag to indicate whether ChatGPT is currently processing a request
    bool waitingForResponse = false;
    bool requestSent = false;
    public void AddMessage(string name, string message)
    {
        // Add a new message to the conversation
        conversationText.text += $"{name}: {message}\r\n";
    }
   
    void Start()
    {
        // Initialize the conversation ID if it is not already set
    if (string.IsNullOrEmpty(conversationId))
{
    conversationId = Guid.NewGuid().ToString();
}

        // Display a greeting message from the bot
        AddMessage(botName, "Hello! How can I help you today?");
          Debug.Log(conversationId);
    }
void Update()
{ 
   
    // Check if the user has entered a new message
    if (messageInput.isFocused && Input.GetKeyDown(KeyCode.Return))
    {
        Debug.Log("Enter key was pressed");
        // Get the message from the input field
        string message = messageInput.text;

        // Add the user's message to the conversation
        AddMessage(userName, message);

        // Clear the input field
        messageInput.text = "";

        // Add the message to the message queue
        messageQueue.Enqueue(message);

        // Send the next message in the queue to ChatGPT if it is not already processing a request
        if (!waitingForResponse)
        {
            string nextMessage = messageQueue.Dequeue();
            SendMessageToChatGPT(nextMessage);
        }
    }
 }
public void OnEndEdit()
{
    if (waitingForResponse || requestSent)
    {
        return;
    }

    // Get the message from the InputField component
    string message = messageInput.text;

    // Clear the InputField component
    messageInput.text = "";

    // Send the message to ChatGPT
    SendMessageToChatGPT(message);

    requestSent = true;
}
void SendMessageToChatGPT(string message)
{
    // Set the waitingForResponse flag to indicate that ChatGPT is processing a request
    waitingForResponse = true;

    // Create a HTTP request to the OpenAI API
    UnityWebRequest request = new UnityWebRequest(
        "https://api.openai.com/v1/models/chat/generate", "POST");
    request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes("model=" + chatGptModel + "&prompt=" + botName + ": " + message + "&conversation_id=" + conversationId + "&max_tokens=256"));
    request.downloadHandler = new DownloadHandlerBuffer();
    request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
    request.SetRequestHeader("Authorization", $"Bearer {apiKey}");
      Debug.Log(request.uploadHandler);
      Debug.Log(request.downloadHandler);
      
    // Check if the request has already been sent
    if (!request.isDone)
    {
        // Send the request and wait for the response
        StartCoroutine(WaitForResponse(request));
    }
}


IEnumerator WaitForResponse(UnityWebRequest request)
{
  Debug.Log("Waiting for response");

    yield return request.SendWebRequest();

    // Check for errors
    if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
    {
        Debug.LogError(request.error);
    }
    if (request.responseCode == 404)
    {
        Debug.LogError("API endpoint not found: " + request.error);
    }
    else
    {
        // Parse the response
        JObject response = JObject.Parse(request.downloadHandler.text);
        string botMessage = (string)response["data"]["text"];

        // Add the bot's message to the conversation
        AddMessage(botName, botMessage);

        // Set the waitingForResponse flag to false
        waitingForResponse = false;

        // Set the requestSent flag to false
        requestSent = false;

        // Send the next message in the queue if there are any left
        if (messageQueue.Count > 0)
        {
            string message = messageQueue.Dequeue();
            SendMessageToChatGPT(message);
        }
    }
}


}//class

