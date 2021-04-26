using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAgents : MonoBehaviour
{

    [Header("Camara lenta")]
    [Range(0, 1)]
    [SerializeField] private float slowmo = 0.5f;

    [Header("Parámetros de la simulación")]
    [SerializeField] private float numberOfAgents = 10;
    [Range(0, 1)]
    [SerializeField] private float beta = 0.5f;
    [Range(0, 1)]
    [SerializeField] private float sigma = 0.5f;
    [Range(0, 1)]
    [SerializeField] private float gamma = 0.5f;
    [Range(0, 1)]
    [SerializeField] private float visionPercentage;
    private float agentVision;
    [Range(0, 50)]
    [SerializeField] private float agentSpeed;
    private float smoothMovement = 0.2f;


    [SerializeField] private GameObject agentPrefab;
    private List<GameObject> agents;
    private ToroidalWorld world;
    private Vector2 velocityRef;

    private Graph graph;
    private int timeCounter = 0;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = slowmo;
        world = GameObject.Find("Main Camera").GetComponent<ToroidalWorld>();
        agentVision = visionPercentage * (new Vector2(world.getWorldWidth(), world.getWorldHeight())).magnitude;
        Debug.Log("Agent vision");
        //Inicializamos los agentes
        agents = new List<GameObject>();
        for (int i = 0; i < numberOfAgents; i++)
        {
            float randPositionX = Random.Range(world.getAnchorPoint().x, world.getAnchorPoint().x + world.getWorldWidth());
            float randPositionY = Random.Range(world.getAnchorPoint().y, world.getAnchorPoint().y + world.getWorldHeight());



            GameObject agent = Instantiate(agentPrefab, new Vector2(randPositionX, randPositionY), Quaternion.Euler(new Vector3(0, 0, 0)));
            agent.GetComponent<AgentScript>().setVelocity(Random.insideUnitCircle*agentSpeed);
            if(i == numberOfAgents - 1)
            {
                agent.GetComponent<AgentScript>().setState(AgentState.INFECTED);
            }
            agents.Add(agent);
        }
        //Inicializamos los gráficos de series de tiempo
        graph = GameObject.Find("Graph").GetComponent<Graph>();
        graph.clearAllTimeSeries();
        TimeSeries susceptibleSeries = new TimeSeries("Susceptibles");
        susceptibleSeries.setColor("#146eff");
        susceptibleSeries.setLineWidth(2);
        TimeSeries exposedSeries = new TimeSeries("Expuestos");
        exposedSeries.setColor("#ffb624");
        exposedSeries.setLineWidth(2);
        TimeSeries infectedSeries = new TimeSeries("Infectados");
        infectedSeries.setColor("#ff2424");
        infectedSeries.setLineWidth(2);
        TimeSeries recoveredSeries = new TimeSeries("Recuperados");
        recoveredSeries.setColor("#2bff36");
        recoveredSeries.setLineWidth(2);
        graph.addTimeSeries(susceptibleSeries);
        graph.addTimeSeries(exposedSeries);
        graph.addTimeSeries(infectedSeries);
        graph.addTimeSeries(recoveredSeries);

    }

    // Update is called once per frame
    void Update()
    {
        float randAngle = 0;
        float numberOfSusceptibles = 0;
        float numberOfExposed = 0;
        float numberOfInfected = 0;
        float numberOfRecovered = 0;


        for (int i = 0; i < agents.Count; i++)
        {
            
            //Actualizamos el estado de los agentes
            AgentScript agentInfo = agents[i].GetComponent<AgentScript>();
            agentInfo.updateAgentState(areNeighboursSick(agents[i]), beta, sigma, gamma);
            //Aumentamos la cuenta de susceptibles, expuestos infectados y recuperados
            switch (agentInfo.getState())
            {
                case AgentState.SUSCEPTIBLE:
                    numberOfSusceptibles += 1;
                    break;
                case AgentState.EXPOSED:
                    numberOfExposed += 1;
                    break;
                case AgentState.INFECTED:
                    numberOfInfected += 1;
                    break;
                case AgentState.RECOVERED:
                    numberOfRecovered += 1;
                    break;
                default:
                    break;
            }

            //Movemos un poco al gente
            Vector3 vec3Vel = agentInfo.getVelocity();
            agents[i].transform.position = world.getToroidalPosition(agents[i].transform.position + vec3Vel * Time.deltaTime);

            //También vamos a cambiar aleatoriamente su velocidad
            // randDirection = Vector2.SmoothDamp(agentInfo.getVelocity(), randDirection, ref velocityRef, smoothMovement);
            agentInfo.setVelocity(Random.insideUnitCircle*agentSpeed);
        }
        if(this.timeCounter < TimeSeries.MAX_POINTS)
        {
            graph.addPointToTimeSeries("Susceptibles", new Point(timeCounter, numberOfSusceptibles));
            graph.addPointToTimeSeries("Expuestos", new Point(timeCounter, numberOfExposed));
            graph.addPointToTimeSeries("Infectados", new Point(timeCounter, numberOfInfected));
            graph.addPointToTimeSeries("Recuperados", new Point(timeCounter, numberOfRecovered));

            timeCounter += 1;
        }
    }


    public bool areNeighboursSick(GameObject agent)
    {
        bool areNeighboursSick = false;

        for(int i = 0; i<agents.Count; i++)
        {
            if(agents[i] != agent && world.getToroidalDistance(agent.transform.position, agents[i].transform.position)<= agentVision && agents[i].GetComponent<AgentScript>().getState()== AgentState.INFECTED )
            {
                return true;
            }
        }
        return areNeighboursSick;
    }
}
