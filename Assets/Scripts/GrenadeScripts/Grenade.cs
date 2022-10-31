using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float TimeExplo = 1f;
    public float explosionMomet;
    public GameObject prefabExploion;


    void Start()
    {
        explosionMomet = Time.time + TimeExplo;
    }

    void Update()
    {
        if (Time.time > explosionMomet) 
        {
            Explosion();
        }
    }
    private void Explosion()
    {
        Destroy(gameObject);

        Instantiate(prefabExploion, transform.position, transform.rotation);

        Collider[] allObjets = Physics.OverlapSphere(transform.position, 2f);

        foreach (Collider obj in allObjets)
        {
            if (obj.gameObject.CompareTag("Button"))
            {
                obj.gameObject.GetComponent<Button>().Activation();
            }
        }
    }
}
