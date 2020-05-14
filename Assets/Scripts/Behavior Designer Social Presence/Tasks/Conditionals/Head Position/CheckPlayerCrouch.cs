using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;


namespace TFG
{
    [TaskDescription("Comprueba si el jugador se ha agachado")]
    [TaskCategory("TFG")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}SeekIcon.png")]
    public class CheckPlayerCrouch : Conditional
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("Objeto que representa al jugador")]
        public SharedGameObject targetObject;

        [BehaviorDesigner.Runtime.Tasks.Tooltip("Porcentaje de altura a la que se quiere detectar si está agachado el jugador")]
        public SharedInt crouchHeightPercentage;

        private Transform targetTransform;//transform del jugador
        private float defaultHeight;//Altura por defecto del jugador   
        private float crouchHeight;//Altura a la que se quiere detectar si el jugador está agachado

        public override void OnStart()
        {
            //Se obtiene el Transform del jugador
            targetTransform = targetObject.Value.transform;

            //TODO: QUITAR, NO EXISTA YA NPC. Se obtiene la altura inicial de la cabeza del jugador
            defaultHeight = GetComponent<NPC>().StandardPlayerHeight;

            //En base al porcentaje, se halla la altura a la que se quiere detectar si el jugador está agachado
            crouchHeight = defaultHeight - (crouchHeightPercentage.Value * defaultHeight / 100);
        }

        /// <summary>
        /// Método que comprueba si el jugador está agachado, en cuyo caso se devuelve éxito
        /// </summary>
        /// <returns></returns>
        public override TaskStatus OnUpdate()
        {
            if (targetTransform.position.y <= crouchHeight)            
                return TaskStatus.Success;
                      
            else           
                return TaskStatus.Failure;          
        }
    }
}


