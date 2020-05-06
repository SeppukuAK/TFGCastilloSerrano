using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEditor;

namespace TFG
{
    [TaskDescription("Suelta el objeto")]
    [TaskCategory("TFG")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}Play.png")]
    public class LeaveObject : Action
    {
        public SharedGameObject targetObject;//Camara
        public SharedGameObject ingredient;//Ingrediente

        private Rigidbody rb;
        private bool picked;

        public override void OnStart()
        {
            picked = false;

            //Obtengo el Rigidbody del ingrediente
            rb = ingredient.Value.GetComponent<Rigidbody>(); // ?? No se sabe si es necesario
        }

        public void DetachIngredient()
        {
            //ESTO ES PARA SIMULAR
            //Unimos la posicion del objeto a la posicion de la mano         
            // ingredient.Value.transform.parent = null;


            //Unimos la posicion del objeto a la posicion de la cámara        
            ingredient.Value.transform.SetParent(targetObject.Value.transform, true);

            //Se resetean la posicion y la rotación del ingrediente 
            ingredient.Value.transform.localPosition = new Vector3(0.0f, 0.0f, 2.0f);
            ingredient.Value.transform.localRotation = new Quaternion(0, 0, 0, 1);
        }

        public override TaskStatus OnUpdate()
        {
            //ESTO ES PARA SIMULAR QUE SE COGE EL INGREDIENTE
            if (Input.GetKeyDown(KeyCode.P))
            {
                picked = !picked;
                if (picked)               
                    DetachIngredient();//Suelta el objeto
                                   
                return TaskStatus.Success;
            }
            else
                return TaskStatus.Running;

        }
    }
}

