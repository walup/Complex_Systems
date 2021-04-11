using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{


    // Start is called before the first frame update
    private Vector3 velocity;
    void Start()
    {
        this.transform.localScale = new Vector3(10, 10, 10);
        Debug.Log("Escalado");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 getCohesionVector(List<GameObject> neighbours)
    {
        Vector3 cohesionVector = Vector3.zero;
        //Obtenemos el centroide del grupo de vecinos
        int n = neighbours.Count;
        print("Count " + n.ToString());
        for(int i = 0; i < n; i++)
        {
            cohesionVector = cohesionVector + neighbours[i].transform.position / n;
        }

        cohesionVector = cohesionVector - this.transform.position;

        return cohesionVector.normalized;
    }


    public Vector3 getAlignmentVector(List<GameObject> neighbours)
    {
        Vector3 alignmentVector = transform.forward;

        int n = neighbours.Count;
        for (int i = 0; i < n; i++)
        {
            alignmentVector = alignmentVector + neighbours[i].transform.forward / (n + 1);
        }

        return alignmentVector.normalized;
    }

    public Vector3 getRepulsionVector(List<GameObject> neighbours)
    {
        Vector3 repulsionVector = Vector3.zero;
        int n = neighbours.Count;
        for(int i = 0; i < n; i++)
        {
            repulsionVector = repulsionVector + (this.transform.position - neighbours[i].transform.position) / n;
        }

        return repulsionVector.normalized;
    }


    public List<GameObject> getNeighbours(List<GameObject> allFish, WorldCube cube, float visionRadius)
    {
        List<GameObject> neighbours = new List<GameObject>();
        int n = allFish.Count;
        for(int i = 0; i <n; i++)
        {
            float distance = cube.getDistanceWithinCube(this.transform.position, allFish[i].transform.position);

            if(distance <= visionRadius)
            {
                neighbours.Add(allFish[i]);
            }
        }
        return neighbours;


    }

    public void setVelocity(Vector3 velocity)
    {
        this.velocity = velocity;
    }

    public Vector3 getVelocity() {
        return velocity;
    }

}
