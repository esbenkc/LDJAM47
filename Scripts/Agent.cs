using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{

    [SerializeField]
    float speed = 2, rotation = 2;

    [SerializeField]
    LayerMask raycastMask;

    private float[] input = new float[10];
    public NeuralNetwork network;

    private float fitness = 0, currentTime = 0;
    private bool touchedTarget = false, spottedTarget = false;

    // Rewards and punishment variables s
    public bool punishForHittingWalls = false, rewardForHittingTarget = false, punishForDistance = false;

    // Booleans for control
    public bool useRays = true;

    // Control
    public int rayAmount = 10;
    public float rayAngle = 20;

    Vector2[] rays = new Vector2[10];
    Transform target;

    [SerializeField]
    GameObject lrprefab;

    LineRenderer[] lrs;

    private void Start()
    {
        rays = new Vector2[rayAmount];
        input = new float[rayAmount];
        lrs = new LineRenderer[rayAmount];
        target = GameObject.Find("Target").transform;
        for (int i = 0; i < rayAmount; i++)
        {
            lrs[i] = Instantiate(lrprefab, this.transform).GetComponent<LineRenderer>();
        }
    }

    private void FixedUpdate()
    {
        if (useRays)
        {
            for (int i = 0; i < rayAmount; i++)
            {
                float angle = (i * rayAngle - (rayAngle * rayAmount / 2) - transform.eulerAngles.z) * Mathf.Deg2Rad;
                rays[i] = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));

                RaycastHit2D hit2D = Physics2D.Raycast(transform.position, rays[i], 10, raycastMask);

                if (hit2D)
                {
                    input[i] = ((10 - hit2D.distance) / 10);
                    // spottedTarget = (hit2D.collider.gameObject.layer == 9 ? true : false);
                }
                else
                {
                    input[i] = 0; // Returns 0 if nothing is detected
                }
            }
        }
        else
        {
            input = new float[rayAmount];
        }
        // if (spottedTarget)
        //     input[10] = Vector2.Distance(transform.position, target.position);
        // else
        //     input[10] = 0;

        float[] output = network.FeedForward(input);

        // transform.rotation = Quaternion.Euler(0, 0, transform.eulerAngles.z + 1 * output[0] * rotation); // Turning
        // transform.position = new Vector2(transform.position.x, transform.position.y + 1 * output[1] * Time.deltaTime); // Moving

        transform.Rotate(new Vector3(0, 0, 1 * output[0] * rotation), Space.World);
        transform.Translate(new Vector2(1 * output[1] * speed * Time.deltaTime, 0), Space.Self);

        // currentTime += Time.deltaTime;
        // if (currentTime > 5)
        // {
        //     Debug.Log("Rotation: " + Quaternion.Euler(0, 0, transform.eulerAngles.z + output[0] * rotation) + "\nPosition: " + new Vector2(transform.position.x, transform.position.y + 1 * output[1] * Time.deltaTime) + "\nOutput 1: " + output[0] + ", output 2: " + output[1]);
        //     currentTime = 0;
        // }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.4f);
        for (int i = 0; i < rays.Length; i++)
        {
            lrs[i].SetPositions(new Vector3[] { transform.position, (new Vector2(transform.position.x, transform.position.y) + rays[i].normalized * 3) });
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (punishForHittingWalls && collision.collider.gameObject.layer == 8)
        {
            fitness -= 0.01f;
        }
        if (rewardForHittingTarget && collision.collider.gameObject.layer == 9)
        {
            fitness += 0.5f;
            touchedTarget = true;
        }

    }

    public void UpdateFitness()
    {
        network.fitness = fitness
        - (punishForDistance ? (Vector2.Distance(transform.position, target.position) * 0.1f) : 0)
        + (touchedTarget ? 1 : -1);
    }
}