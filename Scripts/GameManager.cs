using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Scene Manipulation")]
    [SerializeField]
    UnityEngine.SceneManagement.Scene[] levels;

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



    public IEnumerator Transition()
    {
        Vector2 pPlayer = player.position, pComputer = computer.position, pWall = wall.position;
        float v = 0, pCamSize = cam.orthographicSize;

        while (currentTime < transitionTime)
        {
            v = interpolation.Evaluate(currentTime);

            player.position = pPlayer + tPlayer * v;
            computer.position = pComputer + tComputer * v;
            wall.position = pWall + tWall * v;
            cam.orthographicSize = pCamSize + camSize * v;

            currentTime += Time.deltaTime;
            yield return null;
        }
    }

}
