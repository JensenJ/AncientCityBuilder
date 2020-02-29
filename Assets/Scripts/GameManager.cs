using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject loadingScreen;

    private void Awake()
    {
        instance = this;

        SceneManager.LoadSceneAsync((int)SceneIndexes.TITLE_SCREEN, LoadSceneMode.Additive);
    }

    public void LoadGame()
    {
        loadingScreen.gameObject.SetActive(true);
        SceneManager.UnloadSceneAsync((int)SceneIndexes.TITLE_SCREEN);
        SceneManager.LoadSceneAsync((int)SceneIndexes.GAME_MAP, LoadSceneMode.Additive);
    }
}

public enum SceneIndexes
{
    MANAGER = 0,
    TITLE_SCREEN = 1,
    GAME_MAP = 2,
}