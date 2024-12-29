using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using LLMUnity;
using System;
using System.Linq;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using UnityEngine.UI;

public class testRag : MonoBehaviour
{
    public RAG rag;
    public TextAsset SageText;
    string ragPath = "SageTest.zip";

    [Header("UI elements")]
    public InputField PlayerText;
    public Text AIText;

    Dictionary<string, Dictionary<string, string>> botQuestionAnswers = new Dictionary<string, Dictionary<string, string>>();

    async void Start()
    {
        AddListeners();
        botQuestionAnswers["Sage"] = LoadQuestionAnswers(SageText.text);
        await InitRAG();
        // await Game();
    }

    void AddListeners()
    {
        PlayerText.onSubmit.AddListener(OnInputFieldSubmit);
        PlayerText.onValueChanged.AddListener(OnValueChanged);
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

    async void OnInputFieldSubmit(string message)
    {
        Debug.Log($"Input message: {message}");
        PlayerText.interactable = false;
        AIText.text = "...";
        (string[] similarQuestions, float[] distances) = await rag.Search(message, 1, "Sage");
        // get the answers of the similar questions
        List<string> similarAnswers = new List<string>();

        foreach (string similarQuestion in similarQuestions) similarAnswers.Add(botQuestionAnswers["Sage"][similarQuestion]);
        Debug.Log($"RAG retrieved {similarAnswers.Count} results for question: {message}");
        foreach (string similarAnswer in similarAnswers)
        {
            Debug.Log($"Similar answers for question: {similarAnswer}");
        }

        AIText.text = similarAnswers.Count > 0 ? similarAnswers[0] : "Apologies young adventurer, I am unsure what you are asking. Can you ask again in another way?";
        PlayerText.interactable = true;
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
        Debug.Log(questionAnswers.Count);
        return questionAnswers;
    }

    async Task InitRAG()
    {
        bool loaded = await rag.Load(ragPath);
        if (!loaded)
        {
#if UNITY_EDITOR
            Stopwatch stopwatch = new Stopwatch();
            // build the embeddings
            foreach ((string botName, Dictionary<string, string> botQuestionAnswers) in botQuestionAnswers)
            {
                // PlayerText.text += $"Creating Embeddings for {botName} (only once)...\n";
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

    // Sanity test
    async Task Game()
    {
        string message = "Hello";
        (string[] similarQuestions, float[] distances) = await rag.Search(message, 1, "Sage");
        // get the answers of the similar questions
        List<string> similarAnswers = new List<string>();

        foreach (string similarQuestion in similarQuestions) similarAnswers.Add(botQuestionAnswers["Sage"][similarQuestion]);
        Debug.Log($"RAG retrieved {similarAnswers.Count} results for question: {message}");

        foreach (string similarAnswer in similarAnswers)
        {
            Debug.Log($"Similar answers for question: {similarAnswer}");
        }

        //foreach (string similarQuestion in similarQuestions)
        //{
        //    Debug.Log(similarQuestion);
        //}

        //if (similarPhrases.Length > 0)
        //{
        //    foreach(var phrase in similarPhrases)
        //    {
        //        Debug.Log($"Response: {phrase}");
        //    }
        //    // Debug.Log($"Response: {questionAnswers[similarPhrases[0]]}");
        //}
        //else
        //{
        //    Debug.Log("Found no matching phrases.");
        //}
    }
}
