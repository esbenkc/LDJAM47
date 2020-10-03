using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : MonoBehaviour
{
    [SerializeField]
    float speed = 1.5f, timeBetweenStates = 1;

    [SerializeField]
    Vector2[] velocityStates;

    private float currentTime = 0;

    private Rigidbody2D rb2d;

    private void Start()
    {
        rb2d = transform.GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        currentTime += Time.deltaTime;
        if (currentTime > timeBetweenStates)
        {
            rb2d.velocity = velocityStates[Random.Range(0, velocityStates.Length)].normalized * speed;
            currentTime = 0;
        }
    }

}