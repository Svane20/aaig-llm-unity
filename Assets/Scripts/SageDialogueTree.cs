using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ControlledDialogueTree : MonoBehaviour
{
    [Header("UI Elements")]
    public Text npcText; // NPC's response text
    public Text[] optionTexts; // Player's option texts (three options)
    public Button[] optionButtons; // Buttons corresponding to the options

    private string[] LayerOptions = { "Hello", "Who are you?", "Can you tell me about a legend?" };
    private string[] LayerResponses =
    {
        "Greetings, traveler. It is not often I have visitors. What knowledge do you seek from an old sage such as myself?",
        "I am Alaric, the old sage. My knowledge spans the ages, and I am here to guide those seeking the path of legends.",
        "There is one legend that stands above the rest, whispered across generations."
    };

    void Start()
    {
        ShowLayer();
    }

    void ShowLayer()
    {
        for (int i = 0; i < optionTexts.Length; i++)
        {
            if (i < LayerOptions.Length)
            {
                optionTexts[i].text = LayerOptions[i];
                optionButtons[i].gameObject.SetActive(true);

                // Capture the index for the button click
                int optionIndex = i;
                optionButtons[i].onClick.RemoveAllListeners();
                optionButtons[i].onClick.AddListener(() => OnOptionSelected(optionIndex));
            }
            else
            {
                optionTexts[i].text = "";
                optionButtons[i].gameObject.SetActive(false);
            }
        }
    }

    // Handle option selection
    void OnOptionSelected(int optionIndex)
    {
        npcText.text = LayerResponses[optionIndex]; // Show the corresponding response
        DisableOptions(); // Disable buttons after a response
        switch (npcText.text)
        {
            case "Greetings, traveler. It is not often I have visitors. What knowledge do you seek from an old sage such as myself?":
                LayerOptions = new string[] { "Hello", "Who are you?", "Can you tell me about a legend?" };
                LayerResponses = new string[]
                {
                    "Greetings, traveler. It is not often I have visitors. What knowledge do you seek from an old sage such as myself?",
                    "I am Alaric, the old sage. My knowledge spans the ages, and I am here to guide those seeking the path of legends.",
                    "There is one legend that stands above the rest, whispered across generations."
                };
                ShowLayer(); 
                break;
            case "I am Alaric, the old sage. My knowledge spans the ages, and I am here to guide those seeking the path of legends.":
                LayerOptions = new string[] { "What is the path of legends?" };
                LayerResponses = new string[] { "The path of legends is a journey for those with the courage to seek the extraordinary. Few have walked it, and fewer still have succeeded." };
                ShowLayer();
                break;
            case "The path of legends is a journey for those with the courage to seek the extraordinary. Few have walked it, and fewer still have succeeded.":
                LayerOptions = new string[] { "What legend is it?" };
                LayerResponses = new string[] { "It is the tale of a blade like no other, one that holds the power to shape the fate of kingdoms." };
                ShowLayer();
                break;
            case "It is the tale of a blade like no other, one that holds the power to shape the fate of kingdoms.":
                LayerOptions = new string[] { "What is this blade?" };
                LayerResponses = new string[] { "The blade is known as Excalibur, a sword of immense power and mystique. Yet, its story is not for idle ears." };
                ShowLayer();
                break;
            case "The blade is known as Excalibur, a sword of immense power and mystique. Yet, its story is not for idle ears.":
                LayerOptions = new string[] 
                { 
                    "Why do you guard this story?",
                    "How can I earn the story?"
                };
                LayerResponses = new string[] 
                { 
                    "The story of Excalibur is not given lightly. It is a truth that must be earned by those with resolve and determination.",
                    "Ask your questions wisely, for knowledge comes to those who seek with patience and purpose. Only then will I tell you more."
                };
                ShowLayer();
                break;
            case "The story of Excalibur is not given lightly. It is a truth that must be earned by those with resolve and determination.":
                LayerOptions = new string[]
                {
                    "What truths?",
                    "What is the path of legends?"
                };
                LayerResponses = new string[]
                {
                    "Truths that can shape the destiny of those brave enough to uncover them. Some truths lie dormant, waiting for the worthy.",
                    "The path of legends is a journey for those with the courage to seek the extraordinary. Few have walked it, and fewer still have succeeded."
                };
                ShowLayer();
                break;
            case "Truths that can shape the destiny of those brave enough to uncover them. Some truths lie dormant, waiting for the worthy.":
                LayerOptions = new string[]
                {
                    "How can I earn this story?"
                };
                LayerResponses = new string[]
                {
                    "Ask your questions wisely, for knowledge comes to those who seek with patience and purpose. Only then will I tell you more."
                };
                ShowLayer();
                break;
            case "There is one legend that stands above the rest, whispered across generations.":
                LayerOptions = new string[]
                {
                    "What legend is it?",
                    "How can I earn this story?"
                };
                LayerResponses = new string[]
                {
                    "It is the tale of a blade like no other, one that holds the power to shape the fate of kingdoms.",
                    "Ask your questions wisely, for knowledge comes to those who seek with patience and purpose. Only then will I tell you more."
                };
                ShowLayer();
                break;
            case "Ask your questions wisely, for knowledge comes to those who seek with patience and purpose. Only then will I tell you more.":
                LayerOptions = new string[]
                {
                    "Why do you guard this story?",
                    "What is the path of legends?",
                    "What is this blade?"
                };
                LayerResponses = new string[]
                {
                    "The story of Excalibur is not given lightly. It is a truth that must be earned by those with resolve and determination.",
                    "The path of legends is a journey for those with the courage to seek the extraordinary. Few have walked it, and fewer still have succeeded.",
                    "The blade is known as Excalibur, a sword of immense power and mystique. Yet, its story is not for idle ears."
                };
                ShowLayer();
                break;
        }
    }

    // Disable option buttons
    void DisableOptions()
    {
        foreach (var button in optionButtons)
        {
            button.gameObject.SetActive(false);
        }

        StartCoroutine(Waiter());
    }

    IEnumerator Waiter()
    {
        yield return new WaitForSeconds(2);
    }
}
