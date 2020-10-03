using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
You world need to formulate your problem into a MDP. You need to design the state space, action space, reward function and so on. Your agent will do what it is rewarded to do under the constraints. You may not get the results you want if you design the things differently.
*/
public class Environment : MonoBehaviour
{
    public void SetTime(float timeVar)
    {
        Time.timeScale = timeVar;
    }
}
