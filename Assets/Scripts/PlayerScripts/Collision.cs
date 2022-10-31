using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{

    [Header("Layers")]
    public LayerMask groundLayer;
    public float raycastDistanceGround;
    public float raycastDistanceWall;

    [Space]

    [Header("Booleans")]
    public bool onGround;
    public bool onWall;
    public bool onRightWall;
    public bool onLeftWall;
    public int wallSide;


    void Update()
    {  
        onGround = Physics.Raycast((Vector3)transform.position,Vector3.down, raycastDistanceGround, groundLayer);
        onWall = Physics.Raycast((Vector3)transform.position, Vector3.left, raycastDistanceWall, groundLayer) 
            || Physics.Raycast((Vector3)transform.position, Vector3.right, raycastDistanceWall, groundLayer);

        /*onWall = Physics.Raycast((Vector3)transform.position, Vector3.forward, raycastDistanceWall, groundLayer) 
            || Physics.Raycast((Vector3)transform.position, Vector3.back, raycastDistanceWall, groundLayer);*/

        onRightWall = Physics.Raycast((Vector3)transform.position, Vector3.right, raycastDistanceWall, groundLayer);
        onLeftWall = Physics.Raycast((Vector3)transform.position, Vector3.left, raycastDistanceWall, groundLayer);

        /*onRightWall = Physics.Raycast((Vector3)transform.position, Vector3.back, raycastDistanceWall, groundLayer);
        onLeftWall = Physics.Raycast((Vector3)transform.position, Vector3.forward, raycastDistanceWall, groundLayer);*/

        wallSide = onRightWall ? -1 : 1;

        //Debug.DrawRay((Vector3)transform.position, Vector3.down,Color.red);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;      

        Gizmos.DrawRay(transform.position, Vector3.down * raycastDistanceGround);
        Gizmos.DrawRay(transform.position, Vector3.right * raycastDistanceWall);
        Gizmos.DrawRay(transform.position, Vector3.left * raycastDistanceWall);  
        
        Gizmos.DrawRay(transform.position, Vector3.back * raycastDistanceWall);
        Gizmos.DrawRay(transform.position, Vector3.forward * raycastDistanceWall);
    }
}
