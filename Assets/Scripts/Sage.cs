using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using UnityEngine.UI;
using LLMUnity;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine.SceneManagement;

public class Sage : SageUI
{
    [Header("Models")] public LLMCharacter llmCharacter;
    public RAG rag;
    public int numRAGResults = 3;

    private const string RagPath = "Sage.zip";
    private readonly Dictionary<string, Dictionary<string, string>> _botQuestionAnswers = new();
    private readonly Dictionary<string, RawImage> _botImages = new();
    private const string CurrentBotName = "Sage";
    private bool _onValidateWarning = true;

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
        _botImages["Sage"] = SageImage;
        _botQuestionAnswers["Sage"] = LoadQuestionAnswers(SageText.text);
    }

    async Task InitRAG()
    {
        // Create the embeddings
        await CreateEmbeddings();

        // Print the number of questions embedded
        Debug.Log($"{CurrentBotName}: {rag.Count(CurrentBotName)} phrases available");
        llmCharacter.AIName = CurrentBotName;
    }

    void InitLLM()
    {
        // warm-up the LLM
        PlayerText.text += "Preparing the model...";
        _ = llmCharacter.Warmup(AIReplyComplete);
    }

    public Dictionary<string, string> LoadQuestionAnswers(string questionAnswersText)
    {
        var questionAnswers = new Dictionary<string, string>();
        foreach (var line in questionAnswersText.Split("\n"))
        {
            if (line == "")
            {
                continue;
            }

            var lineParts = line.Split("|");
            questionAnswers[lineParts[0]] = lineParts[1];
        }

        return questionAnswers;
    }

    public async Task CreateEmbeddings()
    {
        var loaded = await rag.Load(RagPath);
        if (!loaded)
        {
#if UNITY_EDITOR
            var stopwatch = new Stopwatch();

            // Build the embeddings
            foreach (var (botName, botQuestionAnswers) in _botQuestionAnswers)
            {
                PlayerText.text += $"Creating embeddings for {botName} (only once)...\n";
                var questions = botQuestionAnswers.Keys.ToList();

                stopwatch.Start();

                foreach (var question in questions)
                {
                    await rag.Add(question, botName);
                }

                stopwatch.Stop();

                Debug.Log($"Embedded {rag.Count()} phrases in {stopwatch.Elapsed.TotalMilliseconds / 1000f} secs");
            }

            // Store the embeddings
            rag.Save(RagPath);
#else
            // if in play mode throw an error
            throw new System.Exception("The embeddings could not be found!");
#endif
        }
    }

    public async Task<List<string>> Retrieval(string question)
    {
        // Find similar questions for the current bot using the RAG
        var (similarQuestions, _) = await rag.Search(question, numRAGResults, CurrentBotName);

        // Get the answers of the similar questions
        var similarAnswers = similarQuestions
            .Select(similarQuestion => _botQuestionAnswers[CurrentBotName][similarQuestion]).ToList();

        return similarAnswers;
    }

    public async Task<string> ConstructPrompt(string question)
    {
        // Get similar answers from the RAG
        var similarAnswers = await Retrieval(question);

        // Create the prompt using the user question and the similar answers
        var answers = similarAnswers.Aggregate("", (current, similarAnswer) => current + $"\n- {similarAnswer}");

        // Construct the prompt
        var prompt = $"Question: {question}\n\n";
        prompt += $"Possible Answers: {answers}";

        return prompt;
    }

    protected async override void OnInputFieldSubmit(string question)
    {
        PlayerText.interactable = false;
        SetAIText("...");

        var prompt = await ConstructPrompt(question);
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

    public void ReturnToTown()
    {
        SceneManager.LoadScene(0); // Load the scene with build index 0
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
        CheckLLM(rag.search.llmEmbedder, debug);
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