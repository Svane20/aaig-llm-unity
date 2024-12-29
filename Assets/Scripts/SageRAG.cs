using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using UnityEngine.UI;
using LLMUnity;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Linq;

public class SageRAG : MonoBehaviour
{
    public RAG rag;
    public InputField PlayerText;
    public Text AIText;
    public TextAsset SageText;
    List<string> phrases;
    string ragPath = "SageRag.zip";
    Dictionary<string, Dictionary<string, string>> botQuestionAnswers = new Dictionary<string, Dictionary<string, string>>();

    async void Start()
    {
        CheckLLMs(false);
        PlayerText.interactable = false;
        LoadPhrases();
        await CreateEmbeddings();
        PlayerText.onSubmit.AddListener(onInputFieldSubmit);
        AIReplyComplete();
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

    public void LoadPhrases()
    {
        // phrases = RAGUtils.ReadGutenbergFile(Sage.text)["Sage"];
        botQuestionAnswers["Sage"] = LoadQuestionAnswers(SageText.text);
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
        else
        {
            Debug.Log("Loaded premade embeddings");
        }
    }

    protected async virtual void onInputFieldSubmit(string message)
    {
        Debug.Log($"Message: {message}");
        PlayerText.interactable = false;
        AIText.text = "...";
        (string[] similarQuestions, float[] distances) = await rag.Search(message, 1);
        List<string> similarAnswers = new List<string>();
        foreach (string similarQuestion in similarQuestions) similarAnswers.Add(botQuestionAnswers["Sage"][similarQuestion]);
        Debug.Log($"RAG retrieved {similarAnswers.Count} results for question: {message}");
        foreach (string similarQuestion in similarAnswers)
        {
            Debug.Log(similarQuestion);
        }

        AIText.text = similarQuestions.Length > 0 ? similarQuestions[0]: "Apologies young adventurer, I am unsure what you are asking. Can you ask again in another way?";
        // return similarAnswers;
    }

    public void SetAIText(string text)
    {
        AIText.text = text;
    }

    public void AIReplyComplete()
    {
        PlayerText.interactable = true;
        PlayerText.Select();
        PlayerText.text = "";
    }

    protected void CheckLLM(LLMCaller llmCaller, bool debug)
    {
        if (!llmCaller.remote && llmCaller.llm != null && llmCaller.llm.model == "")
        {
            string error = $"Please select a llm model in the {llmCaller.llm.gameObject.name} GameObject!";
            if (debug) Debug.LogWarning(error);
            else throw new System.Exception(error);
        }
    }

    protected virtual void CheckLLMs(bool debug)
    {
        CheckLLM(rag.search.llmEmbedder, debug);
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

