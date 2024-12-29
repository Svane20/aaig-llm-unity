using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using UnityEngine.UI;
using LLMUnity;
using System.Threading.Tasks;
using System.Linq;
using System;

public class Sage : SageUI
{
    [Header("Models")]
    public LLMCharacter llmCharacter;
    public RAG rag;
    public int numRAGResults = 3;

    string ragPath = "Sage.zip";
    Dictionary<string, Dictionary<string, string>> botQuestionAnswers = new Dictionary<string, Dictionary<string, string>>();
    Dictionary<string, RawImage> botImages = new Dictionary<string, RawImage>();
    string currentBotName = "Sage";

    new async void Start()
    {
        base.Start();
        CheckLLMs(false);
        InitElements();
        await InitRAG();
        InitLLM();
    }

    void InitElements()
    {
        PlayerText.interactable = false;
        botImages["Sage"] = SageImage;
        botQuestionAnswers["Sage"] = LoadQuestionAnswers(SageText.text);
    }

    async Task InitRAG()
    {
        // create the embeddings
        await CreateEmbeddings();
    }

    void InitLLM()
    {
        // warm-up the LLM
        PlayerText.text += "Warming up the model...";
        _ = llmCharacter.Warmup(AIReplyComplete);
    }

    public Dictionary<string, string> LoadQuestionAnswers(string questionAnswersText)
    {
        Dictionary<string, string> questionAnswers = new Dictionary<string, string>();
        foreach (string line in questionAnswersText.Split("\n"))
        {
            if (line == "") continue;
            string[] lineParts = line.Split("|");
            questionAnswers[lineParts[0]] = lineParts[1];
        }
        return questionAnswers;
    }

    public async Task CreateEmbeddings()
    {
        bool loaded = await rag.Load(ragPath);
        if (!loaded)
        {
#if UNITY_EDITOR
            Stopwatch stopwatch = new Stopwatch();
            // build the embeddings
            foreach ((string botName, Dictionary<string, string> botQuestionAnswers) in botQuestionAnswers)
            {
                PlayerText.text += $"Creating Embeddings for {botName} (only once)...\n";
                List<string> questions = botQuestionAnswers.Keys.ToList();
                stopwatch.Start();
                foreach (string question in questions) await rag.Add(question, botName);
                stopwatch.Stop();
                Debug.Log($"embedded {rag.Count()} phrases in {stopwatch.Elapsed.TotalMilliseconds / 1000f} secs");
            }
            // store the embeddings
            rag.Save(ragPath);
#else
            // if in play mode throw an error
            throw new System.Exception("The embeddings could not be found!");
#endif
        }
    }

    public async Task<List<string>> Retrieval(string question)
    {
        llmCharacter.AIName = currentBotName;
        // find similar questions for the current bot using the RAG
        (string[] similarQuestions, _) = await rag.Search(question, numRAGResults, currentBotName);
        // get the answers of the similar questions
        List<string> similarAnswers = new List<string>();
        foreach (string similarQuestion in similarQuestions) similarAnswers.Add(botQuestionAnswers[currentBotName][similarQuestion]);
        Debug.Log($"RAG retrieved {similarAnswers.Count} results for question: {question}");
        foreach (string similarQuestion in similarAnswers)
        {
            Debug.Log(similarQuestion);
        }
            
        return similarAnswers;
    }

    public async Task<string> ConstructPrompt(string question)
    {
        // get similar answers from the RAG
        List<string> similarAnswers = await Retrieval(question);
        // create the prompt using the user question and the similar answers
        string answers = "";
        foreach (string similarAnswer in similarAnswers) answers += $"\n- {similarAnswer}";
        // string prompt = $"Robot: {currentBotName}\n\n";
        string prompt = $"Question: {question}\n\n";
        prompt += $"Possible Answers: {answers}";
        return prompt;
    }

    protected async override void OnInputFieldSubmit(string question)
    {
        Debug.Log(question);
        PlayerText.interactable = false;
        Debug.Log("Setting AI text");
        SetAIText("...");
        Debug.Log("Contructing prompt");
        string prompt = await ConstructPrompt(question);
        Debug.Log($"Prompt: {prompt}");
        _ = llmCharacter.Chat(prompt, SetAIText, AIReplyComplete);
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
        if (!llmCaller.remote && llmCaller.llm != null && llmCaller.llm.model == "")
        {
            string error = $"Please select a llm model in the {llmCaller.llm.gameObject.name} GameObject!";
            if (debug) Debug.LogWarning(error);
            else throw new System.Exception(error);
        }
    }

    void CheckLLMs(bool debug)
    {
        CheckLLM(rag.search.llmEmbedder, debug);
        CheckLLM(llmCharacter, debug);
    }

    bool onValidateWarning = true;
    void OnValidate()
    {
        if (onValidateWarning)
        {
            CheckLLMs(true);
            onValidateWarning = false;
        }
    }
}

public class SageUI : MonoBehaviour
{
    [Header("UI elements")]
    public InputField PlayerText;
    public Text AIText;

    [Header("Bot texts")]
    public TextAsset SageText;

    [Header("Bot images")]
    public RawImage SageImage;

    protected void Start()
    {
        AddListeners();
    }

    void OnValueChanged(string newText)
    {
        // Get rid of newline character added when we press enter
        if (Input.GetKey(KeyCode.Return))
        {
            if (PlayerText.text.Trim() == "")
                PlayerText.text = "";
        }
    }

    protected virtual void AddListeners()
    {
        PlayerText.onSubmit.AddListener(OnInputFieldSubmit);
        PlayerText.onValueChanged.AddListener(OnValueChanged);
    }

    protected virtual void OnInputFieldSubmit(string question) {}

}
