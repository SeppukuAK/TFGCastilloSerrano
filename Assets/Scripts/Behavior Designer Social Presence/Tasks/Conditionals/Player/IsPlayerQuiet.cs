using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEditor;

namespace TFG
{
    [TaskDescription("El jugador esta quieto")]
    [TaskCategory("TFG")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}Play.png")]
    public class IsPlayerQuiet : Conditional
    {
        public SharedGameObject targetObject;//Jugador
        public float threshold; //Minima cantidad en la que se asume movimiento

        private Vector3 lastTargetPos;//Posicion anterior del jugador
        private Vector3 actualTargetPos;//Posicion actual del jugador

        private bool positionChanged, rotationChanged;//Variables para la detección de la posicion o rotacion modificadas

        //Offset para ver la diferencia de posiciones y rotaciones
        private Vector3 offsetPos, offsetRot;
        private float offsetSum;//Acumulacion del offset

        //Variables para el contador
        private float nextCheck;//Siguiente momento en el que va a comprobarse si esta quieto
        public float checkRate;//Tiempo de duracion de comprobacion de si esta quieto

        public override void OnStart()
        {
            nextCheck = Time.time + checkRate;

            //Se obtiene la posicion anterior
            lastTargetPos = new Vector3(targetObject.Value.transform.position.x, targetObject.Value.transform.position.y, targetObject.Value.transform.position.z);
            positionChanged = false;
            offsetSum = 0.0f;
        }

        public override TaskStatus OnUpdate()
        {
            if (Time.time > nextCheck && !positionChanged)
                return TaskStatus.Success;

            //Se mueve en la X o en la Z
            else if (Time.time > nextCheck || positionChanged)            
                return TaskStatus.Failure;
                      
            else
            {
                actualTargetPos = new Vector3(targetObject.Value.transform.position.x, targetObject.Value.transform.position.y, targetObject.Value.transform.position.z);
          
                offsetPos = actualTargetPos - lastTargetPos;
                offsetSum += offsetPos.magnitude;//Acumulacion de la variacion de movimiento
                
                positionChanged = offsetSum > threshold;
                Debug.Log("Ultima pos: " + lastTargetPos + "Posicion actual: " + actualTargetPos);

                lastTargetPos = new Vector3(actualTargetPos.x, actualTargetPos.y, actualTargetPos.z); ;//Se intercambian valores
                return TaskStatus.Running;
            }
        }
    }
}
