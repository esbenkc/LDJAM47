using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{

    [SerializeField]
    float reward = 1, speed = 2;

    [SerializeField]
    BoxCollider2D target;

    [SerializeField]
    CircleCollider2D selfCollider;

    [SerializeField]
    Rigidbody2D rb2D;

    [SerializeField]
    Vector4 spawnRand = new Vector4(-1, -1, 1, 1);

    Transform t;

    Vector2[] actions;
    float[] value;

    private void Start()
    {
        t = this.gameObject.transform;
        rb2D.velocity = new Vector2(speed, 0);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("I got it!");
        Reset();
    }

    private void Reset()
    {
        t.position = new Vector2(
            Random.Range(spawnRand.x, spawnRand.z),
            Random.Range(spawnRand.y, spawnRand.w)
        );
    }

    void FixedUpdate()
    {

        // t.position = new Vector2(t.position.x + speed * Time.deltaTime, t.position.y);

    }
}