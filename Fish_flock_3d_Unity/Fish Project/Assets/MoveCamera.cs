using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{

    private float speed = 100;


    private KeyCode fwdCode = KeyCode.W;
    private KeyCode backCode = KeyCode.S;
    private KeyCode rightCode = KeyCode.D;
    private KeyCode leftCode = KeyCode.A;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")).normalized;


        this.transform.Rotate(new Vector3(-mouseDelta.y * speed * Time.deltaTime, mouseDelta.x*speed*Time.deltaTime,0));

        if (Input.GetKey(rightCode))
        {
            this.transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
            print("Right");
        }
        if (Input.GetKey(leftCode))
        {
            this.transform.Translate(new Vector3(-speed * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(fwdCode))
        {
            this.transform.Translate(new Vector3(0, 0, speed * Time.deltaTime));
        }
        if (Input.GetKey(backCode))
        {
            this.transform.Translate(new Vector3(0, 0, -speed * Time.deltaTime));
        }
    }
}
