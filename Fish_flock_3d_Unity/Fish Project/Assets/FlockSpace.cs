using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockSpace : MonoBehaviour
{
    //El prefab de nuestro pez 
    [SerializeField] private GameObject fishPrefab;
    [SerializeField] private float smoothMovement = 0.2f;
    //El mundo toroidal cúbico donde vamos a poner los peces
    private WorldCube world;
    //La lista de peces
    private List<GameObject> fishList;
    [Header("Parámetros generales")]
    //El número de peces
    [SerializeField] private int NUMBER_OF_FISH = 30;
    //La visión de los peces 
    [Range(0, 1)]
    [SerializeField] private float visionPercentage = 0.2f;
    [Range(0, 1)]
    [SerializeField] private float velocityPercentage = 0.2f;
    private float MAX_VELOCITY =100;
    private float fishVelocity;
    private Vector3 velocityRef;

    private float visionRadius;

    [Header("Pesos")]
    [Range(0, 1)]
    [SerializeField] private float cohesionWeight = 0.3f;
    [Range(0,1)]
    [SerializeField] private float alignmentWeight = 0.3f;
    [Range(0, 1)]
    [SerializeField] private float repulsionWeight = 0.3f;


    private Vector3 cohesionVector;
    private Vector3 alignmentVector;
    private Vector3 repulsionVector;

    void Start()
    {
        //Inicializamos la lista de peces 
        fishList = new List<GameObject>();
        //Inicializamos el mundo
        world = GameObject.FindGameObjectWithTag("world_cube").GetComponent<WorldCube>();
        visionRadius = visionPercentage*world.getWorldRadius();
        fishVelocity = MAX_VELOCITY * velocityPercentage;
        Debug.Log(world.getAnchorPoint());
        //Inicializamos los peces dentro del cubo
        initializeFish();
    }

    // Update is called once per frame
    void Update()
    {
        moveFish();
    }

    private void moveFish()
    {
        for(int i = 0; i< NUMBER_OF_FISH; i++)
        {

            Fish fish = fishList[i].GetComponent<Fish>();
            //Obtenemos la lista de vecinos del pez
            List<GameObject> neighbours = fish.getNeighbours(fishList, world, visionRadius);
            if (neighbours.Count != 0)
            {
                cohesionVector = fish.getCohesionVector(neighbours);
                repulsionVector = fish.getRepulsionVector(neighbours);
                alignmentVector = fish.getAlignmentVector(neighbours);

                Vector3 velocityVector = cohesionWeight * cohesionVector + repulsionWeight * repulsionVector + alignmentWeight * alignmentVector;
                velocityVector = Vector3.SmoothDamp(fishList[i].transform.forward, velocityVector, ref velocityRef, smoothMovement);
                velocityVector = velocityVector.normalized * fishVelocity;
                fishList[i].transform.forward = velocityVector;
                fishList[i].transform.position = world.getToroidalPosition(fishList[i].transform.position + velocityVector*Time.deltaTime);
                fish.setVelocity(velocityVector);
            }
            else
            {
                fishList[i].transform.position = world.getToroidalPosition(fishList[i].transform.position + fish.getVelocity() * Time.deltaTime);
            }
        }
    }

    private void initializeFish()
    {
        for (int i = 0; i < NUMBER_OF_FISH; i++)
        {
            float randPositionX = Random.Range(world.getAnchorPoint().x, world.getAnchorPoint().x + world.getWorldWidth());
            float randPositionY = Random.Range(world.getAnchorPoint().y, world.getAnchorPoint().y + world.getWorldHeight());
            float randPositionZ = Random.Range(world.getAnchorPoint().z, world.getAnchorPoint().z + world.getWorldDepth());
            Vector3 randPosition = new Vector3(randPositionX, randPositionY, randPositionZ);
            float randAngleX = Random.Range(0, 360);
            float randAngleY = Random.Range(0, 360);
            float randAngleZ = Random.Range(0, 360);


            Vector3 eulerRandomAngles = new Vector3(randAngleX, randAngleY, randAngleZ);

            GameObject fish = Instantiate(fishPrefab, randPosition, Quaternion.Euler(eulerRandomAngles));

            fish.GetComponent<Fish>().setVelocity(Random.insideUnitSphere*fishVelocity);
            fishList.Add(fish);
        }
    }
}
