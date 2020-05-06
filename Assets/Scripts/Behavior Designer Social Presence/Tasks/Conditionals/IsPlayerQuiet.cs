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
        public Vector3 lastTargetPos;//Posicion anterior del jugador
        public SharedVector3 lastTargetRot;//Rotacion anterior del jugador

        private Transform cameraTransform;
        private bool positionChanged, rotationChanged;//Posicion o rotacion modificadas

        public float threshold; //Minima cantidad en la que se asume movimiento

        private Vector3 offsetPos, offsetRot;
        private float offsetSum;//Acumulacion del offset

        private float nextCheck;
        public float checkRate;

        public override void OnStart()
        {

            nextCheck = Time.time + checkRate;

            //Se obtiene la posicion anterior
            lastTargetPos = targetObject.Value.transform.position;
            positionChanged = false;
            offsetSum = 0.0f;


            ////------------- ROTATION ----------------

            ////Se obtiene la rotacion
            //Vector3 actualCameraRotation =cameraTransform.rotation.eulerAngles;
            //Vector3 lastCameraRotation = lastTargetTransform.Value.rotation.eulerAngles;

            ////Se haya la diferencia de rotaciones
            //offsetRot = (actualCameraRotation - lastCameraRotation);

            //                                        //No se si esto tiene sentido (-threshold)
            //rotationChanged = offsetRot.x > threshold || offsetRot.x < -threshold || offsetRot.y > threshold || offsetRot.y < -threshold || offsetRot.z > threshold || offsetRot.z < -threshold;

            ////------------- ROTATION ----------------
        }

        public override TaskStatus OnUpdate()
        {
            //Se mueve en la X o en la Z o rota en cualquier angulo
            if (Time.time > nextCheck || positionChanged /*|| rotationChanged*/)
            {
                return TaskStatus.Failure;
            }

            else if (Time.time > nextCheck && !positionChanged)
            {
                return TaskStatus.Success;
            }

            else
            {
                Vector3 actualTargetPos = targetObject.Value.transform.position;
          
                offsetPos = actualTargetPos - lastTargetPos;
                offsetSum += offsetPos.magnitude;//Acumulacion de la variacion de movimiento

                positionChanged = offsetSum > threshold;
                lastTargetPos = actualTargetPos;//Se intercambian valores
                return TaskStatus.Running;
            }


        }
    }
}
