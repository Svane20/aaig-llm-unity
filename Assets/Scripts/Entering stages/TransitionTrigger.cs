using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionTrigger : MonoBehaviour
{
    public Text homeText;
    public Text castleText;
    public Text townText;
    public Text forrestText;
    public Text catDog;
    public Text enemy;
    
    public Animator transition;

    private void OnMouseDown()
    {
        if (gameObject.name == "StartingArea")
        {
            StartCoroutine(LoadLevel(12));
        }
        
        if (gameObject.name == "KingsCastle")
        {
            StartCoroutine(LoadLevel(20));
        }
        
        if (gameObject.name == "SaveThePrincess")
        {
            StartCoroutine(LoadLevel(3));
        }

        if (gameObject.name == "HerosHome")
        {
            StartCoroutine(LoadLevel(0));
        }

        if (gameObject.name == "MeetingCatdog")
        {
            StartCoroutine(LoadLevel(8));
        }

        if (gameObject.name == "EnemyLair")
        {
            StartCoroutine(LoadLevel(15));
        }
    }

    private void OnMouseEnter()
    {
        transform.localScale += new Vector3(1.1f, 1.1f, 1.1f);

        if (gameObject.name == "HerosHome")
        {
            homeText.gameObject.SetActive(true);
        }
        
        if (gameObject.name == "SaveThePrincess")
        {
            forrestText.gameObject.SetActive(true);
        }
        
        if (gameObject.name == "KingsCastle")
        {
            castleText.gameObject.SetActive(true);
        }
        
        if (gameObject.name == "StartingArea")
        {
            townText.gameObject.SetActive(true);
        }
        
        if (gameObject.name == "MeetingCatdog")
        {
            catDog.gameObject.SetActive(true);
        }
        
        if (gameObject.name == "EnemyLair")
        {
            enemy.gameObject.SetActive(true);
        }
    }

    private void OnMouseExit()
    {
        transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
        
        if (gameObject.name == "HerosHome")
        {
            homeText.gameObject.SetActive(false);
        }
        
        if (gameObject.name == "SaveThePrincess")
        {
            forrestText.gameObject.SetActive(false);
        }
        
        if (gameObject.name == "KingsCastle")
        {
            castleText.gameObject.SetActive(false);
        }
        
        if (gameObject.name == "StartingArea")
        {
            townText.gameObject.SetActive(false);
        }
        
        if (gameObject.name == "MeetingCatdog")
        {
            catDog.gameObject.SetActive(false);
        }
        
        if (gameObject.name == "EnemyLair")
        {
            enemy.gameObject.SetActive(false);
        }
    }
    
    IEnumerator LoadLevel(int levelIndex)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(levelIndex);
    }
}
