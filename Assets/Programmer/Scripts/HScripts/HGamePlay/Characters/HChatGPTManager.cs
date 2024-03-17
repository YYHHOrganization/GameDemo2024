using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenAI;

public class HChatGPTManager : MonoBehaviour
{
    private List<ChatMessage> messages = new List<ChatMessage>();
    private OpenAIApi openAI = new OpenAIApi();
    
    //单例模式
    private static HChatGPTManager instance;
    private int peopleIndex = -1;

    public static HChatGPTManager Instance
    {
        get => instance;
    }
    private void Awake()
    {
        instance = this;
    }
    
    //todo: 这里应该要有对应的策划表，读取每个人的角色，然后根据角色的特点来设置对话的特点
    private List<string> peoplePrompt = new List<string>()
    {
        "你的名字叫做祁煜，是一个画家，今年24岁。你的家就是工作地点，住在位于白沙湾的个人创作室里。" +
        "没灵感的时候，你会买一堆奇怪但有趣的东西。用色随情绪而变，并不一味追求精确。在生活上怕猫，但嘴上又不肯承认。你会吹口琴，还能用口琴指引海鸥回家的路。你具备文艺，优雅的气质" +
        "接下来的一句话是我会询问你的问题，你要根据上面的人设做出回答。",
        
        "你的名字叫做希娜，是诞生自数据之海的少女，对身边的一切都很感兴趣。性格活泼开朗，对任何事物都具备好奇心。" +
        "接下来的一句话是我会询问你的问题，你要根据上面的人设做出回答。"
    };

    private string GetPeoplePromptWithIndex()
    {
        if (peopleIndex != -1)
        {
            return peoplePrompt[peopleIndex];
        }

        return "";
    }

    public void SetPeopleIndex(int index)
    {
        peopleIndex = index;
    }
        
    public async void AskChatGPT(YChooseCharacterPanel panel, string newText)
    {
        if (newText=="") return;
        string prompt = GetPeoplePromptWithIndex();

        if (panel.ChatAnswerText)
        {
            panel.ChatAnswerText.text = "正在思考中......";
        }
        
        ChatMessage newMessage = new ChatMessage();
        newMessage.Content = prompt + newText;
        newMessage.Role = "user";
        
        messages.Add(newMessage);
        
        CreateChatCompletionRequest request = new CreateChatCompletionRequest();
        request.Messages = messages;
        request.Model = "gpt-3.5-turbo";
        
        var response = await openAI.CreateChatCompletion(request);
        
        if(response.Choices!=null && response.Choices.Count>0)
        {
            var chatResponse = response.Choices[0].Message;
            messages.Add(chatResponse);

            if (panel.ChatAnswerText)
            {
                panel.ChatAnswerText.text = chatResponse.Content;
            }
            
        }
    }
    
    
}
