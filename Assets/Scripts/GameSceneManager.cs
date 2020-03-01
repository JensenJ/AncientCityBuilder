using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void LoadGameMap()
    {
        GameManager manager = GetGameManager();
        if (manager != null)
        {
            manager.LoadScene(SceneIndexes.TITLE_SCREEN, SceneIndexes.GAME_MAP);
        }
    }

    public void LoadMainMenu()
    {
        GameManager manager = GetGameManager();
        if (manager != null)
        {
            manager.LoadScene(SceneIndexes.GAME_MAP, SceneIndexes.TITLE_SCREEN);
        }
    }
}