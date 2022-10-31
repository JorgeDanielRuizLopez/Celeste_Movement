using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    public GameObject grenade;
    public float forceGrenade;

    public Transform firePoint;


    void Update()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            GrenadeInstance();
        }
    }

    private void GrenadeInstance()
    {
        GameObject newGrenade = Instantiate(grenade, transform.position, Quaternion.identity);

        Vector3 dir = (firePoint.position - transform.position).normalized;

        newGrenade.GetComponent<Rigidbody>().AddForce(forceGrenade * dir);

    }
}
