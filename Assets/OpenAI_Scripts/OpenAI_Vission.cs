using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class AIVision : MonoBehaviour
{
    [SerializeField] private string openAIUrl = "https://api.openai.com/v1/chat/completions";
    [SerializeField] private string apiKey = "YOUR_API_KEY"; 

    public string[] imageUrls;
    public string queryMessage = "What are in these images? Is there any difference between them?";

    void Start()
    {
        if (imageUrls.Length > 0)
        {
            StartCoroutine(PostImageQueryRequest(imageUrls));
        }
    }

   public void OnClickSend()
   {
       StartCoroutine(PostImageQueryRequest(imageUrls));
   }

    IEnumerator PostImageQueryRequest(string[] urls)
    {
        var requestBody = new
        {
            model = "gpt-4-vision-preview",
            messages = BuildImageQueryMessages(urls),
            max_tokens = 300
        };

        string json = JsonUtility.ToJson(requestBody);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(openAIUrl, "POST"))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
            webRequest.uploadHandler.contentType = "application/json";
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Authorization", "Bearer " + apiKey);

            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + webRequest.error);
            }
            else
            {
                Debug.Log("Response: " + webRequest.downloadHandler.text);
            }
        }
    }

    private object[] BuildImageQueryMessages(string[] urls)
    {
        var messages = new List<object>
        {
            new { type = "text", text = queryMessage }
        };

        foreach (var url in urls)
        {
            messages.Add(new { type = "image_url", image_url = url });
        }

        return messages.ToArray();
    }

    public void LoadImagesFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("File not found: " + filePath);
            return;
        }

        try
        {
            string[] lines = File.ReadAllLines(filePath);
            StartCoroutine(PostImageQueryRequest(lines));
        }
        catch (IOException e)
        {
            Debug.LogError("Error reading the file: " + e.Message);
        }
    }
}
