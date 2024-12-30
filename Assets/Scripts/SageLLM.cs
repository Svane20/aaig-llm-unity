using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;
using UnityEngine.UI;
using LLMUnity;

public class SageLLM : SageUI
{
    [Header("Models")] public LLMCharacter llmCharacter;
    
    private readonly Dictionary<string, RawImage> _botImages = new();
    private const string CurrentBotName = "Sage";
    private bool _onValidateWarning = true;

    new async void Start()
    {
        base.Start();
        CheckLLMs(false);
        InitElements();
        InitLLM();
    }

    void InitElements()
    {
        PlayerText.interactable = false;
        _botImages["Sage"] = SageImage;
    }

    void InitLLM()
    {
        // warm-up the LLM
        llmCharacter.AIName = CurrentBotName;
        PlayerText.text += "Preparing the model...";
        _ = llmCharacter.Warmup(AIReplyComplete);
    }

    protected async override void OnInputFieldSubmit(string question)
    {
        PlayerText.interactable = false;
        SetAIText("...");
        
        _ = llmCharacter.Chat(question, SetAIText, AIReplyComplete);
    }

    void SetAIText(string text)
    {
        AIText.text = text;
    }

    void AIReplyComplete()
    {
        PlayerText.interactable = true;
        PlayerText.Select();
        PlayerText.text = "";
    }

    public void CancelRequests()
    {
        llmCharacter.CancelRequests();
        AIReplyComplete();
    }

    void CheckLLM(LLMCaller llmCaller, bool debug)
    {
        if (llmCaller.remote || llmCaller.llm == null || llmCaller.llm.model != "")
        {
            return;
        }

        var error = $"Please select a llm model in the {llmCaller.llm.gameObject.name} GameObject!";
        if (debug)
        {
            Debug.LogWarning(error);
        }
        else
        {
            throw new System.Exception(error);
        }
    }

    void CheckLLMs(bool debug)
    {
        CheckLLM(llmCharacter, debug);
    }

    void OnValidate()
    {
        if (!_onValidateWarning)
        {
            return;
        }

        CheckLLMs(true);
        _onValidateWarning = false;
    }

}

public class SageLLMUI : MonoBehaviour
{
    [Header("UI elements")] 
    public InputField PlayerText;
    public Text AIText;

    [Header("Bot images")] 
    public RawImage SageImage;

    protected void Start()
    {
        AddListeners();
    }

    void OnValueChanged(string newText)
    {
        // Get rid of newline character added when we press enter
        if (!Input.GetKey(KeyCode.Return))
        {
            return;
        }
        
        if (PlayerText.text.Trim() == "")
        {
            PlayerText.text = "";
        }
    }

    protected virtual void AddListeners()
    {
        PlayerText.onSubmit.AddListener(OnInputFieldSubmit);
        PlayerText.onValueChanged.AddListener(OnValueChanged);
    }

    protected virtual void OnInputFieldSubmit(string question)
    {
    }
}