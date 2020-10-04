using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*
You world need to formulate your problem into a MDP. You need to design the state space, action space, reward function and so on. Your agent will do what it is rewarded to do under the constraints. You may not get the results you want if you design the things differently.
*/
public class Manager : MonoBehaviour
{
    public float timeframe;
    public int populationSize; //creates population size
    public GameObject prefab; //holds bot prefab

    [SerializeField]
    TMP_Text console;

    string consoleContent;

    public string ConsoleContent
    {
        get { return consoleContent; }
        set
        {
            consoleContent = value;
            console.text = consoleContent;
        }
    }

    int iteration = 0;

    public int[] layers = new int[3] { 5, 3, 2 }; //initializing network to the right size

    [Range(0.0001f, 1f)] public float MutationChance = 0.01f;

    [Range(0f, 1f)] public float MutationStrength = 0.5f;

    [Header("Settings")]

    // Rewards and punishment variables
    public bool punishForHittingWalls = false;

    public bool rewardForHittingTarget = false, punishForDistance = false;

    // Booleans for control
    public bool useRays = true;

    // Control
    public int rayAmount = 10;
    public float rayAngle = 20;

    //public List<Bot> Bots;
    public List<NeuralNetwork> networks;
    private List<Agent> agents;

    void Start() // Start is called before the first frame update
    {
        console.text = consoleContent;
        // if (populationSize % 2 != 0)
        //     populationSize = 50;//if population size is not even, sets it to fifty

        InitNetworks();
        InvokeRepeating("CreateAgents", 0.1f, timeframe); //repeating function
    }

    /// <summary>
    /// Creates new neural networks based on population size. Loads the file 'Save.txt'.
    /// </summary>
    public void InitNetworks()
    {
        layers[0] = rayAmount;
        Debug.Log(layers[0]);

        networks = new List<NeuralNetwork>();
        for (int i = 0; i < populationSize; i++)
        {
            NeuralNetwork net = new NeuralNetwork(layers);
            // net.Load("Assets/Save.txt");//on start load the network save
            net.GenerateRandom();
            networks.Add(net);
        }
    }

    /// <summary>
    /// Sets timeScale to Gamespeed, removes all existing agents, and instantiates new ones (population size) with given network from networks[i] that has been sorted.
    /// </summary>
    public void CreateAgents()
    {
        if (agents != null)
        {
            for (int i = 0; i < agents.Count; i++)
            {
                GameObject.Destroy(agents[i].gameObject); //if there are Prefabs in the scene this will get rid of them
            }

            SortNetworks(); //this sorts networks and mutates them
        }

        agents = new List<Agent>();
        for (int i = 0; i < populationSize; i++)
        {
            Agent agent = (Instantiate(prefab, new Vector2(-5, -2), new Quaternion(0, 0, 1, 0))).GetComponent<Agent>(); //create agents
            agent.network = networks[i]; //deploys network to each learner
            agent.punishForDistance = punishForDistance;
            agent.punishForHittingWalls = punishForHittingWalls;
            agent.rewardForHittingTarget = rewardForHittingTarget;
            agent.useRays = useRays;
            agent.rayAmount = rayAmount;
            agent.rayAngle = rayAngle;
            agents.Add(agent);
        }
    }

    /// <summary>
    /// Sorts the networks based on fitness rate given by agent script. When sorted, saves the highest and mutates the bottom half networks.
    /// </summary>
    public void SortNetworks()
    {
        for (int i = 0; i < populationSize; i++)
        {
            agents[i].UpdateFitness(); //gets bots to set their corrosponding networks fitness
        }
        networks.Sort();
        networks[populationSize - 1].Save("Assets/Save.txt"); //saves networks weights and biases to file, to preserve network performance
        ConsoleContent += iteration + ".best-fitness=" + networks[populationSize - 1].fitness + "\n";
        iteration++;
        for (int i = 0; i < populationSize / 2; i++)
        {
            networks[i] = networks[i + populationSize / 2].copy(new NeuralNetwork(layers));
            networks[i].Mutate((int)(1 / MutationChance), MutationStrength * (-networks[i + populationSize / 2].fitness));
        }
    }

    public void SetTime(float time)
    {
        Time.timeScale = time;
    }
}