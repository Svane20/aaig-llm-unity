using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using LLMUnity;
using System.Linq;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using UnityEngine.UI;

public class testRag : RAGUI
{
    public RAG rag;
    public int numRAGResults = 1;

    private const string RagPath = "SageRAG.zip";
    private readonly Dictionary<string, Dictionary<string, string>> _botQuestionAnswers = new();
    private readonly Dictionary<string, RawImage> _botImages = new();
    private const string CurrentBotName = "Sage";
    private bool _onValidateWarning = true;

    new async void Start()
    {
        base.Start();
        InitElements();
        await InitRAG();
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
        AIReplyComplete();
        Debug.Log($"{CurrentBotName}: {rag.Count(CurrentBotName)} phrases available");
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

    async Task CreateEmbeddings()
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

    protected async override void OnInputFieldSubmit(string question)
    {
        PlayerText.interactable = false;
        SetAIText("...");

        // Get the similar answers
        var similarAnswers = await Retrieval(question);

        // Set the text of the AI
        var text = "";
        if (similarAnswers.Count > 0)
        {
            if (similarAnswers[0] != "Response")
            {
                text = similarAnswers[0];
            }
            else
            {
                text = "Apologies young adventurer, I am unsure what you are asking. Can you ask again in another way?";
            }
        }
        SetAIText(text);

        // Reset the input field
        AIReplyComplete();
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
        CheckLLM(rag.search.llmEmbedder, debug);
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

public class RAGUI : MonoBehaviour
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