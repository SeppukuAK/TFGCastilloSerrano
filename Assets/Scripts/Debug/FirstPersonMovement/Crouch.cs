using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crouch : MonoBehaviour
{
    private Transform headTransform;
    private bool crouched;

    void Awake()
    {
        //Camara del XRRig
        headTransform = GetComponentInChildren<Camera>().transform;
    }
    private void Start()
    {
        crouched = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            crouched = !crouched;
            if (crouched)
            {
                Debug.Log("Agachado");
                headTransform.transform.localPosition = new Vector3(headTransform.transform.localPosition.x, 0.01f, headTransform.transform.localPosition.z);
            }
            else
            {
                Debug.Log("Levantado");
                headTransform.transform.localPosition = new Vector3(headTransform.transform.localPosition.x, 0.4f, headTransform.transform.localPosition.z);
            }
        }
        
    }
}
