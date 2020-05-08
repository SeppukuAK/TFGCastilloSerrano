using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEditor;

namespace TFG
{
    [TaskDescription("Coge el objeto")]
    [TaskCategory("TFG")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}Play.png")]
    public class PickUpObject : Action
    {
        public SharedGameObject targetObject;
        private GameObject hand;
        private Rigidbody rb;

        public override void OnAwake()
        {
            //Obtengo la mano del NPC
            hand = GetComponent<SP_NPC>().Hand;
        }
        public override void OnStart()
        {
            //Obtengo el Rigidbody del ingrediente
            rb = targetObject.Value.GetComponent<Rigidbody>();

            //Unimos la posicion del objeto a la posicion de la mano         
            targetObject.Value.transform.SetParent(hand.transform, true);

            //Se resetean la posicion y la rotación del ingrediente 
            targetObject.Value.transform.localPosition = new Vector3(0, 0, 0);
            targetObject.Value.transform.localRotation = new Quaternion(0, 0, 0, 1);
            rb.isKinematic = true;
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }
    }
}
