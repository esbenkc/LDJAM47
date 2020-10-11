using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Scene Manipulation")]
    [SerializeField]
    string[] levels;

    [SerializeField]
    int currentLevel = -1;


    [SerializeField]
    Manager manager;

    [Header("Animation")]
    [SerializeField]
    AnimationCurve interpolation;

    [SerializeField]
    float transitionTime = 4;

    [SerializeField]
    Camera cam;

    [SerializeField]
    Transform player, computer, wall;

    [SerializeField]
    Vector2 tPlayer, tComputer, tWall;

    [SerializeField]
    float camSize = 12;

    float currentTime = 0;

    [Header("UI")]
    [SerializeField]
    GameObject titleScreen;

    [SerializeField]
    GameObject gameUI, winPanel, infoPanel;

    public void NextLevel()
    {
        if (currentLevel != -1)
        {
            SceneManager.UnloadSceneAsync(levels[currentLevel]);
            manager.SelectAgent();
        }
        currentLevel++;
        if (currentLevel < levels.Length)
            SceneManager.LoadSceneAsync(levels[currentLevel], LoadSceneMode.Additive);
        else
            WinGameCompletely();
        Time.timeScale = 1;
    }

    public void WinGameCompletely()
    {
        StartTransition(true);
    }

    private void Start()
    {
        winPanel.SetActive(false);
        infoPanel.SetActive(false);
        gameUI.SetActive(false);
    }

    public void StartTransition(bool reverse = false)
    {
        titleScreen.SetActive(false);
        StartCoroutine(Transition(reverse));
    }

    public IEnumerator Transition(bool reverse = false)
    {
        Vector2 pPlayer = player.position, pComputer = computer.position, pWall = wall.position;
        float v = 0, pCamSize = cam.orthographicSize, mult = (reverse ? -1 : 1);


        while (currentTime < transitionTime)
        {
            v = interpolation.Evaluate(currentTime / transitionTime);

            player.position = pPlayer + mult * tPlayer * v;
            computer.position = pComputer + mult * tComputer * v;
            wall.position = pWall + mult * tWall * v;
            cam.orthographicSize = pCamSize + mult * camSize * v;

            currentTime += Time.deltaTime;
            yield return null;
        }
        gameUI.SetActive(true);
        infoPanel.SetActive(true);
        NextLevel();
    }

}
