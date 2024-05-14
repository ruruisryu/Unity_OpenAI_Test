using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class OpenAIController : MonoBehaviour
{
    public TMP_Text textField;
    public TMP_InputField inputField;
    public Button okButton;

    private OpenAIAPI _api;
    private List<ChatMessage> _messages;

    void Start()
    {
        _api = new OpenAIAPI("");
        StartConversation();
        okButton.onClick.AddListener(() => GetResponse());
    }

    private void StartConversation()
    {
        _messages = new List<ChatMessage>
        {
            new ChatMessage(ChatMessageRole.System, "You are a minstrel of the DnD world. You are a mysterious and rarely tell your own story. When someone talks to you, they write poems with rhetorical expressions.")
        };
        inputField.text = "";
        string startString = "It's Derga, a minstrel. He might tell you a beautiful story?";
        textField.text = startString;
        Debug.Log(startString);
    }

    private async void GetResponse()
    {
        if (inputField.text.Length < 1)
        {
            return;
        }
        
        // Disable the OK Button
        okButton.enabled = false;
        
        // Fill the user message from the input field
        ChatMessage userMessage = new ChatMessage();
        userMessage.Role = ChatMessageRole.User;
        userMessage.Content = inputField.text;
        if (userMessage.Content.Length > 100)
        {
            // Limit messages to 100 characters
            userMessage.Content = userMessage.Content.Substring(0, 100);
        }
        Debug.Log($"{userMessage.rawRole}: {userMessage.Content}");
        
        // Add the message to the list
        _messages.Add(userMessage);
        
        // Update the text field with the user message
        textField.text = textField.text + string.Format("\nYou: {0}", userMessage.Content);
        
        // Clear the input field
        inputField.text = "";
        
        // Send the entire chat to OpenAI to get the next message
        var chatResult = await _api.Chat.CreateChatCompletionAsync(new ChatRequest()
        {
            Model = Model.ChatGPTTurbo,
            Temperature = 0.1,
            MaxTokens = 50,
            Messages = _messages
        });
        
        // Get the response message
        ChatMessage responseMessage = new ChatMessage();
        responseMessage.Role = chatResult.Choices[0].Message.Role;
        responseMessage.Content = chatResult.Choices[0].Message.Content;
        Debug.Log($"{userMessage.rawRole}: {userMessage.Content}");
        
        // Add the response to the list of messages
        _messages.Add(responseMessage);
        
        // Update the text field with the response
        textField.text = textField.text + string.Format("\nDerga: {0}", responseMessage.Content);
        
        // Re-enable the ok-button
        okButton.enabled = true;
    }
}
