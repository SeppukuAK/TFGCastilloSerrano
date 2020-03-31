using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsPlayerLookingAtNPC : MonoBehaviour
{
    private Transform headTransform;

    void Start()
    {
        headTransform = Camera.main.transform;//Obtenemos el headset activo
    }



    void Update()
    {
        RaycastHit hit;

        //Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.Log("Did Hit");
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            Debug.Log("Did not Hit");
        }
    }

}
