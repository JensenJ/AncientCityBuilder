using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using JUCL.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject loadingScreen;
    public JUCLProgressBar progressBar;
    public Camera loadingCamera;

    List<AsyncOperation> scenesLoading = new List<AsyncOperation>();

    float totalSceneProgress;

    private void Awake()
    {
        instance = this;

        SceneManager.LoadSceneAsync((int)SceneIndexes.TITLE_SCREEN, LoadSceneMode.Additive);
    }

    public void LoadScene(SceneIndexes from, SceneIndexes to)
    {
        loadingCamera.gameObject.SetActive(true);

        loadingScreen.gameObject.SetActive(true);

        scenesLoading.Add(SceneManager.UnloadSceneAsync((int)from));
        scenesLoading.Add(SceneManager.LoadSceneAsync((int)to, LoadSceneMode.Additive));

        StartCoroutine(GetSceneLoadProgress());
    }

    public IEnumerator GetSceneLoadProgress()
    {
        for(int i = 0; i < scenesLoading.Count; i++)
        {
            while (!scenesLoading[i].isDone)
            {
                totalSceneProgress = 0;
                foreach(AsyncOperation operation in scenesLoading)
                {
                    totalSceneProgress += operation.progress;
                }

                totalSceneProgress = (totalSceneProgress / scenesLoading.Count) * 100.0f;

                progressBar.SetCurrentFill(Mathf.RoundToInt(totalSceneProgress), 0, 100);
                yield return null;
            }
        }

        loadingCamera.gameObject.SetActive(false);

        //Try to get animator
        JUCLUIAnimator animator = loadingScreen.GetComponent<JUCLUIAnimator>();
        //If animator is present
        if (animator != null)
        {
            //Run disable animation
            animator.Disable();
        }
        else
        {
            //Disable body window
            loadingScreen.SetActive(false);
        }
    }
}

public enum SceneIndexes
{
    MANAGER = 0,
    TITLE_SCREEN = 1,
    GAME_MAP = 2,
}