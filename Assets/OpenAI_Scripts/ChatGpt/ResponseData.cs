using System.Collections.Generic;

[System.Serializable]
public class ResponseData
{
    public List<Choice> choices;
}

[System.Serializable]
public class Choice
{
    public MessageData message;
    public string finish_reason;
    public int index;
}

[System.Serializable]
public class MessageData
{
    public string role;
    public string content;
}
