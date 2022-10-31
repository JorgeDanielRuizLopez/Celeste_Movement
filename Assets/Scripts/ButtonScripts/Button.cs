using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public bool buttonActivation;
    public GameObject door;
    public GameObject button;


    // Start is called before the first frame update
    void Start()
    {
        buttonActivation = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activation()
    {
        buttonActivation = true;

        door.transform.position = new Vector3(0,8,0);

        button.transform.position = new Vector3(-1.7f,-6,0);
    }




}
