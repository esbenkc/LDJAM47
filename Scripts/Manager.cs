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
    public GameObject prefab, selectedPrefab; //holds bot prefab

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

    public bool rewardForHittingTarget = false, punishForDistance = false, rewardForPath = false;

    // Booleans for control
    public bool useRays = true;

    // Control
    public int rayAmount = 10;
    public float rayAngle = 20;

    //public List<Bot> Bots;
    public List<NeuralNetwork> networks;
    private Agent[] agents;
    private bool selected = false;

    string buttonText = "Try with best agent", buttonTextSelected = "Go back to training";
    [Header("UI")]
    [SerializeField]
    TMP_Text button;

    public void PunishForHittingWalls(bool val)
    {
        punishForHittingWalls = val;
    }

    public void RewardForHittingTarget(bool val)
    {
        rewardForHittingTarget = val;
    }

    public void PunishForDistance(bool val)
    {
        punishForDistance = val;
    }
    public void RewardForPath(bool val)
    {
        rewardForPath = val;
    }

    public float trainingTime = 0, seekTime = 0;
    private bool trainingTimeGo = false, seekTimeGo = false;

    [SerializeField]
    TMP_Text winText;
    string winTextText;

    [SerializeField]
    GameObject winUI;

    void Start() // Start is called before the first frame update
    {
        winTextText = winText.text;
        console.text = consoleContent;
        console.fontSize = 10;
        // if (populationSize % 2 != 0)
        //     populationSize = 50;//if population size is not even, sets it to fifty

        InitNetworks();
    }

    private void FixedUpdate()
    {
        if (trainingTimeGo) trainingTime += Time.deltaTime;
        if (seekTimeGo) seekTime += Time.deltaTime;
    }

    public void StartLoopTraining()
    {
        trainingTimeGo = true;
        StartCoroutine(LoopTraining());
    }

    IEnumerator LoopTraining()
    {
        yield return new WaitForSeconds(0.1f);
        while (!selected)
        {
            CreateAgents();
            yield return new WaitForSeconds(timeframe);
        }
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
    /// Removes all existing agents and instantiates new ones (population size) with given network from networks[i] that has been sorted.
    /// </summary>
    public void CreateAgents()
    {
        if (agents != null)
        {
            for (int i = 0; i < agents.Length; i++)
            {
                GameObject.Destroy(agents[i].gameObject); //if there are Prefabs in the scene this will get rid of them
            }

            SortNetworks(); //this sorts networks and mutates them
        }

        agents = new Agent[populationSize];
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
            agents[i] = agent;
        }
    }

    /// <summary>
    /// Selects highest grade agent and plays the level with that guy.
    /// </summary>
    public void SelectAgent()
    {
        if (!selected)
        {
            seekTimeGo = true;
            selected = true;
            if (agents != null)
            {
                for (int i = 0; i < agents.Length; i++)
                {
                    GameObject.Destroy(agents[i].gameObject); //if there are Prefabs in the scene this will get rid of them
                }

                SortNetworks(); //this sorts networks and mutates them
            }
            agents = new Agent[1];
            Debug.Log(networks[networks.Count - 1].fitness);
            Agent agent = (Instantiate(selectedPrefab, new Vector2(-5, -2), new Quaternion(0, 0, 1, 0))).GetComponent<Agent>(); //create agent
            agent.network = networks[networks.Count - 1]; //deploys network to selected agent
            agent.punishForDistance = punishForDistance;
            agent.punishForHittingWalls = punishForHittingWalls;
            agent.rewardForHittingTarget = rewardForHittingTarget;
            agent.useRays = useRays;
            agent.rayAmount = rayAmount;
            agent.rayAngle = rayAngle;
            agents[0] = agent;

            buttonText = button.text;
            button.text = buttonTextSelected;
        }
        else
        {
            seekTimeGo = false;
            seekTime = 0;
            selected = false;
            button.text = buttonText;
            StartLoopTraining();
        }
    }

    /// <summary>
    /// Activates pop-up noting how quickly the level was finished (time to training, time to target) and offering the next level.
    /// </summary>
    /// <param name="winner">The selected agent</param>
    public void WinGame(Agent winner)
    {
        seekTimeGo = false;
        trainingTimeGo = false;
        winText.text = string.Format(winTextText, trainingTime.ToString(), seekTime.ToString());
        Time.timeScale = 0;
        winUI.SetActive(true);
    }

    /// <summary>
    /// Sorts the networks based on fitness rate given by agent script. When sorted, saves the highest and mutates the bottom half networks.
    /// </summary>
    public void SortNetworks()
    {
        if (agents.Length == populationSize)
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
                networks[i].Mutate((int)(1 / MutationChance), MutationStrength);
            }
        }
        else
        {
            ConsoleContent += iteration + ".best-fitness=" + networks[populationSize - 1].fitness + "\n";
            for (int i = 0; i < populationSize; i++)
            {
                networks[i] = networks[networks.Count - 1].copy(new NeuralNetwork(layers));
                networks[i].Mutate((int)(1 / MutationChance), MutationStrength);
            }
        }
    }

    public void SetTime(float time)
    {
        Time.timeScale = time;
    }
}