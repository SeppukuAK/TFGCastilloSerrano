using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace SocialPresenceVR
{
    [TaskDescription("Comprueba si el jugador se ha agachado")]
    [TaskCategory("SocialPresenceVR/PlayerPosition")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}SeekIcon.png")]
    public class IsPlayerCrouching : Conditional
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("Porcentaje de altura a la que se quiere detectar si está agachado el jugador")]
        public SharedInt crouchHeightPercentage;    //Porcentaje de agachamiento al que se quiere detectar si el jugador está agachado

        /// <summary>
        /// Altura del jugador
        /// </summary>
        private float playerHeight;

        /// <summary>
        /// Altura a la que se considera que el jugador está agachado
        /// </summary>
        private float crouchHeight;

        /// <summary>
        /// Obtiene referencias
        /// </summary>
        public override void OnAwake()
        {
            //Se obtiene la altura inicial de la cabeza
            playerHeight = GetComponent<SP_NPC>().PlayerHeight;
        }

        /// <summary>
        /// Inicializa variables
        /// </summary>
        public override void OnStart()
        {
            //En base al porcentaje, se halla la altura a la que se quiere detectar si el jugador está agachado
            crouchHeight = playerHeight - (crouchHeightPercentage.Value * playerHeight / 100);
        }

        /// <summary>
        /// Método que comprueba si el jugador está agachado, en cuyo caso se devuelve éxito
        /// </summary>
        /// <returns></returns>
        public override TaskStatus OnUpdate()
        {
            //Se obtiene el transform del jugador
            Transform targetTransform = Camera.main.transform;

            if (targetTransform.position.y <= crouchHeight)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }
    }
}


