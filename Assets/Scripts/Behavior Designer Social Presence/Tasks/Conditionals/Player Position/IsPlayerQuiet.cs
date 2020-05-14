using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEditor;

namespace SocialPresenceVR
{
    [TaskDescription("El jugador esta quieto")]
    [TaskCategory("SocialPresenceVR/PlayerPosition")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}Play.png")]
    public class IsPlayerQuiet : Conditional
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("Objeto que representa al jugador")]
        public SharedGameObject targetObject;

        [BehaviorDesigner.Runtime.Tasks.Tooltip("Minima cantidad en la que se asume movimiento")]
        public float threshold;

        [BehaviorDesigner.Runtime.Tasks.Tooltip("Tiempo de duracion en el cual se quiere comprobar si el jugador ha estado quieto")]
        public float checkRate;

        private Vector3 lastTargetPos;//Posicion anterior del jugador
        private Vector3 actualTargetPos;//Posicion actual del jugador

        private bool positionChanged, rotationChanged;//Variables para la detección de la posicion o rotacion modificadas

        //Variables de tipo offset para observar la diferencia de posiciones y rotaciones
        private Vector3 offsetPos, offsetRot;
        private float offsetSum;//Acumulacion del offset

        private float nextCheck;//Siguiente momento en el que va a comprobarse si esta quieto

        public override void OnStart()
        {
            //Se inicializa el siguiente instante en el que termina la comprobacion de si el jugador está quieto
            nextCheck = Time.time + checkRate;

            //Se obtiene la posicion anterior
            lastTargetPos = new Vector3(targetObject.Value.transform.position.x, targetObject.Value.transform.position.y, targetObject.Value.transform.position.z);
            positionChanged = false;
            offsetSum = 0.0f;
        }

        /// <summary>
        /// Método que devuelve éxito si el jugador se ha mantenido quieto durante un tiempo 
        /// </summary>
        /// <returns></returns>
        public override TaskStatus OnUpdate()
        {
            if (Time.time > nextCheck && !positionChanged)
                return TaskStatus.Success;

            //Si el jugador se mueve en el momento o despues de un tiempo su posición ha cambiado, devuelve fallo
            else if (Time.time > nextCheck || positionChanged)            
                return TaskStatus.Failure;
                      
            else
            {
                //Se obtiene la nueva posición del jugador
                actualTargetPos = new Vector3(targetObject.Value.transform.position.x, targetObject.Value.transform.position.y, targetObject.Value.transform.position.z);
                
                //Se halla la diferencia de movimiento entre la posición actual y la anterior
                offsetPos = actualTargetPos - lastTargetPos;

                //Acumulacion de la variacion de movimiento a lo largo del tiempo
                offsetSum += offsetPos.magnitude;

                //Se comprueba si la diferencia de movimiento supera el threshold de movimiento establecido
                positionChanged = offsetSum > threshold;

                //Se intercambian valores. La posición anterior ahora es la actual
                lastTargetPos = new Vector3(actualTargetPos.x, actualTargetPos.y, actualTargetPos.z); ;
                return TaskStatus.Running;
            }
        }
    }
}
