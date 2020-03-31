using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsPlayerLookingAtNPC : MonoBehaviour
{
    void Update()
    {
        RaycastHit hit;
        Vector3 headPosition = Camera.main.transform.position;

        //Se comprueba si el rayo que se lanza desde el jugador colisiona con el NPC
        if (Physics.Raycast(headPosition, Camera.main.transform.forward, out hit))
        {
            Debug.DrawRay(headPosition, Camera.main.transform.forward * hit.distance, Color.green);
           // Debug.Log(hit.collider.name);
            //empezar contador
        }
        else
        {
            Debug.DrawRay(headPosition, Camera.main.transform.forward, Color.blue);
           // Debug.Log("Did not Hit");
        }
    }

}
