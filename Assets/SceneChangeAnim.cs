using System;
using System.Collections;
using GameSystems.Combat;
using GameSystems.CustomEventSystems;
using GameSystems.CustomEventSystems.Interaction;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

public class SceneChangeAnim : Singleton<SceneChangeAnim>
{
    private Animator _transition;
    private static readonly int StartAnimation = Animator.StringToHash("Start");
    private SceneLoadManager _sceneManager;
    private GameObject _levelLoader;
    
    private void OnEnable()
    {
        UpdateRef();
        _sceneManager = SceneLoadManager.Instance;
        InteractionHandler.Instance.LevelAnimInt += StartLevelLoadInt;
        InteractionHandler.Instance.LevelAnimName += StartLevelLoadName;
        InteractionHandler.Instance.LevelAnimPrevious += StartLevelLoadPrevious;
    }

    private void UpdateRef()
    {
        _levelLoader = GameObject.Find("LevelLoader");
        _transition = _levelLoader.transform.GetChild(0).GetComponent<Animator>();
    }

    private void OnDisable()
    {
        if (InteractionHandler.Instance != null)
        {
            InteractionHandler.Instance.LevelAnimInt -= StartLevelLoadInt;
            InteractionHandler.Instance.LevelAnimName -= StartLevelLoadName;
            InteractionHandler.Instance.LevelAnimPrevious -= StartLevelLoadPrevious;
        }
        
    }

    private void StartLevelLoadInt(int index)
    {
        StartCoroutine(LoadLevel(index));
    }

    private void StartLevelLoadName(string levelName)
    {
        StartCoroutine(LoadLevel(levelName));
    }

    private void StartLevelLoadPrevious()
    {
        StartCoroutine(LoadPreviousLevel());
    }


    private IEnumerator LoadLevel(int levelIndex)
    {
        UpdateRef();
        if (GameObject.Find("Player"))
        {
            SceneLoadHandler.Instance.OnStoreLastPosition(GameObject.Find("Player").transform.position);
            SceneLoadHandler.Instance.OnStoreCamera(GameObject.Find("Main Camera").transform.position);
        }
        SceneLoadHandler.Instance.OnStoreLastSceneName(SceneManager.GetActiveScene().name);
        _transition.SetTrigger(StartAnimation);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(levelIndex);
    }
    
    private IEnumerator LoadLevel(string levelName)
    {
        UpdateRef();
        if (GameObject.Find("Player"))
        {
            SceneLoadHandler.Instance.OnStoreLastPosition(GameObject.Find("Player").transform.position);
            SceneLoadHandler.Instance.OnStoreCamera(GameObject.Find("Main Camera").transform.position);
        }
        SceneLoadHandler.Instance.OnStoreLastSceneName(SceneManager.GetActiveScene().name);
        _transition.SetTrigger(StartAnimation);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(levelName);
    }

    private IEnumerator LoadPreviousLevel()
    {
        UpdateRef();
        var lastScene = SceneLoadHandler.Instance.OnGetLastSceneName();
        _transition.SetTrigger(StartAnimation);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(lastScene);
    }
    
}
