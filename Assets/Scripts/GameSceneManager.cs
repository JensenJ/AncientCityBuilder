using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JUCL.Utilities;

public class GameSceneManager : MonoBehaviour
{
    private GameManager GetGameManager() 
    {
        GameManager gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("No game manager found");
            return null;
        }
        else
        {
            return gameManager;
        }
    }

    public void LoadGameMap(float delay)
    {
        JUCLTimer.Create(() => { 
            GameManager manager = GetGameManager();
            if (manager != null)
            {
                manager.LoadScene(SceneIndexes.TITLE_SCREEN, SceneIndexes.GAME_MAP);
            }
        }, delay);
    }

    public void LoadMainMenu(float delay)
    {
        JUCLTimer.Create(() => { 
            GameManager manager = GetGameManager();
            if (manager != null)
            {
                manager.LoadScene(SceneIndexes.GAME_MAP, SceneIndexes.TITLE_SCREEN);
            }
        }, delay);
    }
}