using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentsStoplight : MonoBehaviour
{
    [Header("Camara lenta")]
    [Range(0, 1)]
    [SerializeField] private float slowmo = 0.5f;

    [Header("Parámetros de la simulación")]
    [SerializeField] private float numberOfAgents = 10;
    [Range(0, 1)]
    [SerializeField] private float beta = 0.69f;
    [Range(0, 1)]
    [SerializeField] private float sigma = 0.5f;
    [Range(0, 1)]
    [SerializeField] private float gamma = 0.08f;
    [Range(0, 1)]
    [SerializeField] private float hospitalProb = 0.5f;
    [Range(0, 1)]
    [SerializeField] private float visionPercentage;
    private float agentVision;
    [Range(0, 50)]
    [SerializeField] private float agentSpeed;
    private float smoothMovement = 0.2f;
    private float repulsionRadius = 10;

    //Los parámetros para el semaforo
    private float socialDistancing;
    public static bool stopLight = false;
    public static int patientsLimit;
    private float LOW_SOCIAL_DISTANCING = 0;
    private float HIGH_SOCIAL_DISTANCING = 10;
    public static float hospitalCount = 0;


    [SerializeField] private GameObject agentPrefab;
    private List<GameObject> agents;
    private ToroidalWorld world;
    private Vector2 velocityRef;

    private Graph graph;
    private int timeCounter = 0;

    [SerializeField] private Sprite greenLightSprite;
    [SerializeField] private Sprite redLightSprite;
    private SpriteRenderer lightRenderer;
    // Start is called before the first frame update
    void Start()
    {
        lightRenderer = GameObject.Find("Spotlight").GetComponent<SpriteRenderer>();
        patientsLimit = (int)Mathf.Round((float)this.numberOfAgents*0.01f);
        Debug.Log("limite de pacientes " + patientsLimit);
        socialDistancing = LOW_SOCIAL_DISTANCING;
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
            agent.GetComponent<HospitalizableScript>().setVelocity(Random.insideUnitCircle*agentSpeed);
            if (i == numberOfAgents - 1)
            {
                agent.GetComponent<HospitalizableScript>().setState(AgentState.INFECTED);
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

        hospitalCount = 0;

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
            HospitalizableScript agentInfo = agents[i].GetComponent<HospitalizableScript>();
            agentInfo.updateAgentState(areNeighboursSick(agents[i]), beta, sigma, gamma,hospitalProb);
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
            Vector3 repulsingVector = repulsionDirection(agents[i]);
            //Movemos un poco al gente
            Vector3 vec3Vel = agentInfo.getVelocity();
            Vector3 totalVel = ((vec3Vel.normalized + socialDistancing * repulsingVector.normalized).normalized) * agentSpeed;
            agents[i].transform.position = world.getToroidalPosition(agents[i].transform.position + totalVel * Time.deltaTime);

            //También vamos a cambiar aleatoriamente su velocidad
            Vector2 randDirection = Random.insideUnitCircle;
            // randDirection = Vector2.SmoothDamp(agentInfo.getVelocity(), randDirection, ref velocityRef, smoothMovement);
            Vector2 newVelocity = randDirection * agentSpeed;
            agentInfo.setVelocity(newVelocity);
        }

        //Ya que actualizamos los agentes vamos a actualizar el semaforo
        if(!stopLight && isHospitalNearFull())
        {
            stopLight = true;
            socialDistancing = HIGH_SOCIAL_DISTANCING;
            lightRenderer.sprite = redLightSprite;
            Debug.Log("Stop light activated");
        }
        else if (stopLight && !isHospitalNearFull())
        {
            stopLight = false;
            lightRenderer.sprite = greenLightSprite;
            socialDistancing = LOW_SOCIAL_DISTANCING;
            Debug.Log("Stop light shut down");
        }


        if (this.timeCounter < TimeSeries.MAX_POINTS)
        {
            graph.addPointToTimeSeries("Susceptibles", new Point(timeCounter, numberOfSusceptibles));
            graph.addPointToTimeSeries("Expuestos", new Point(timeCounter, numberOfExposed));
            graph.addPointToTimeSeries("Infectados", new Point(timeCounter, numberOfInfected));
            graph.addPointToTimeSeries("Recuperados", new Point(timeCounter, numberOfRecovered));

            timeCounter += 1;
        }


    }

    public Vector2 repulsionDirection(GameObject agent)
    {
        Vector3 repulsionVector = Vector3.zero;
        int neighbours = 0;
        Vector3 diffVector;
        for (int i = 0; i < agents.Count; i++)
        {
            diffVector = agent.transform.position - agents[i].transform.position;
            if (agents[i] != agent && diffVector.magnitude <= repulsionRadius)
            {
                repulsionVector += diffVector / diffVector.sqrMagnitude;
                neighbours += 1;
            }
        }

        return repulsionVector / neighbours;

    }

    public bool areNeighboursSick(GameObject agent)
    {
        bool areNeighboursSick = false;

        for (int i = 0; i < agents.Count; i++)
        {
            if (agents[i] != agent && world.getToroidalDistance(agent.transform.position, agents[i].transform.position) <= agentVision && agents[i].GetComponent<HospitalizableScript>().getState() == AgentState.INFECTED && !agents[i].GetComponent<HospitalizableScript>().isHospitalized())
            {
                return true;
            }
        }
        return areNeighboursSick;
    }

    public static bool isHospitalFull()
    {
        return hospitalCount >= patientsLimit;
    }

    public bool isHospitalNearFull()
    {
        return hospitalCount > 0.6 * patientsLimit;
    }


}
