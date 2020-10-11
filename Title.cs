using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Title : MonoBehaviour
{

    [SerializeField]
    RectTransform RT;

    Vector3 angle;

    private void Start()
    {
        RT.eulerAngles.Set(0, 0, -0.5f);
    }

    void Update()
    {
        RT.Rotate(new Vector3(0, 0, Mathf.Sin(Time.time) * Time.deltaTime));
    }
}
