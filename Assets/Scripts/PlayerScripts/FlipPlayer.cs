using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipPlayer : MonoBehaviour
{
    public static int rotationValue = 0;
    private static int maxRotation = 4;


    public float rightRotationValue = -90;
    public float leftRotationValue = 90;


    public KeyCode leftRotate = KeyCode.LeftArrow;
    public KeyCode rightRotate = KeyCode.RightArrow;


    private void Start()
    {
        rotationValue = 0;
    }

    private void Update()
    {
        RotateWithKey();
    }

    protected void RotateWithKey()
    {
        if (Input.GetKeyDown(rightRotate) )
        {
            Rotate(rightRotationValue);
            rotationValue = (rotationValue + 1) % maxRotation;
            
            return;
        }
        else if (Input.GetKeyDown(leftRotate))
        {
            if (rotationValue == 0)
            {
                Rotate(leftRotationValue);
                rotationValue = 3;
            }
            else
            {
                Rotate(leftRotationValue);
                rotationValue = (rotationValue - 1) % maxRotation;
            }      
            return;
        }
        return;
    }

    private void Rotate(float value)
    {
        transform.Rotate(Vector3.up,transform.rotation.y + value);
    }

}
