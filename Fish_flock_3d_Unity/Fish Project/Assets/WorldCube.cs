using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCube : MonoBehaviour

{

    [SerializeField] private GameObject frontWall;
    [SerializeField] private GameObject backWall;
    [SerializeField] private GameObject topWall;
    [SerializeField] private GameObject bottomWall;
    [SerializeField] private GameObject rightWall;
    [SerializeField] private GameObject leftWall;
    [SerializeField] private GameObject scale;
    // Start is called before the first frame update

    private float worldWidth;
    private float worldHeight;
    private float worldDepth;

    private Vector3 anchorPoint;

    private float worldRadius = 0;
    private Vector3 midVector;

    void Awake()
    {
        frontWall = Instantiate(this.frontWall);
        backWall = Instantiate(this.backWall);
        topWall = Instantiate(this.topWall);
        bottomWall = Instantiate(this.bottomWall);
        rightWall = Instantiate(this.rightWall);
        leftWall = Instantiate(this.leftWall);



        anchorPoint.Set(leftWall.transform.position.x, bottomWall.transform.position.y, frontWall.transform.position.z);
        worldWidth = rightWall.transform.position.x - leftWall.transform.position.x;
        worldHeight = topWall.transform.position.y - bottomWall.transform.position.y;
        worldDepth = backWall.transform.position.z - frontWall.transform.position.z;

        print(worldWidth);
        print(worldHeight);
        print(worldDepth);

        worldRadius = Mathf.Min(worldWidth / 2, worldHeight / 2, worldDepth / 2);
        print(worldRadius);
        midVector = new Vector3(anchorPoint.x + (worldWidth / 2), anchorPoint.y + (worldHeight / 2), anchorPoint.z + (worldDepth / 2));
    }

    // Update is called once per frame

    public float getWorldWidth()
    {
        return worldWidth;
    }

    public float getWorldHeight()
    {
        return worldHeight;
    }

    public float getWorldDepth()
    {
        return worldDepth;
    }

    public Vector3 getAnchorPoint()
    {
        return anchorPoint;
    }

    public float getWorldRadius()
    {
        return worldRadius;
    }


    float nfmod(float a, float b)
    {
        return (float)(a - b * Mathf.Floor(a / b));
    }

    public Vector3 getToroidalPosition(Vector3 position)
    {
        Vector3 toroidalPosition = new Vector3();

        toroidalPosition.x = anchorPoint.x + nfmod(position.x - anchorPoint.x, worldWidth);
        toroidalPosition.y = anchorPoint.y + nfmod(position.y - anchorPoint.y, worldHeight);
        toroidalPosition.z = anchorPoint.z + nfmod(position.z - anchorPoint.z, worldDepth);
        return toroidalPosition;
    }

    public float getDistanceWithinCube(Vector3 position1, Vector3 position2)
    {
        float distance = 0;
        float dx = Mathf.Abs(position1.x - position2.x);
        float dy = Mathf.Abs(position1.y - position2.y);
        float dz = Mathf.Abs(position1.z - position2.z);


        if(dx > worldWidth/2)
        {
            dx = worldWidth - dx;
        }

        if(dy > worldHeight/2)
        {
            dy = worldHeight - dy;
        }

        if(dz > worldDepth/2)
        {
            dz = worldDepth - dz;
        }


        Vector3 differential = new Vector3(dx, dy, dz);
        distance = differential.magnitude;

        return distance;
    }
}
