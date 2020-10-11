using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{

    [SerializeField]
    AudioSource AS;

    // Update is called once per frame
    void FixedUpdate()
    {
        AS.panStereo = Mathf.Sin(Time.time);
    }
}