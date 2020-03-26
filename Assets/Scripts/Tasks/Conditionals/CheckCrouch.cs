using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;


namespace TFG
{
    [TaskDescription("Comprueba si el jugador se ha agachado")]
    [TaskCategory("TFG")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}SeekIcon.png")]
    public class CheckCrouch : Conditional
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The object that we are searching for")]
        public SharedGameObject targetObject;//Cámara (Su posición varía según el tracking

        [BehaviorDesigner.Runtime.Tasks.Tooltip("The crouch height we want to detect")]
        public SharedInt crouchHeightPercentage;//Porcentaje de agachamiento al que se quiere detectar si el jugador está agachado

        private Transform targetTransform;//transform de la cámara
        private float defaultHeight;//Altura por defecto del jugador   
        private float crouchHeight;


        public override void OnStart()
        {
            targetTransform = targetObject.Value.transform;//Se obtiene el componente Transform del jugador

            //Se obtiene la altura inicial de la cabeza
            defaultHeight = GetComponent<NPC>().StandardPlayerHeight;
            Debug.Log(defaultHeight);
            crouchHeight = defaultHeight - (crouchHeightPercentage.Value * defaultHeight / 100);
        }

        public override TaskStatus OnUpdate()
        {
            //Debug.Log("ALTURA ACTUAL: " + targetTransform.position.y);
            //Debug.Log("ALTURA DE AGACHAMIENTO: " + crouchHeight.Value);
            //Debug.Log("ALTURA ESTANDAR: " + defaultHeight.position.y);

            if (targetTransform.position.y <= crouchHeight)
            {
                Debug.Log("El jugador está AGACHADO");
                return TaskStatus.Success;

            }
            else
            {
                Debug.Log("El jugador NO está AGACHADO");
                return TaskStatus.Failure;
            }
        }
    }
}


