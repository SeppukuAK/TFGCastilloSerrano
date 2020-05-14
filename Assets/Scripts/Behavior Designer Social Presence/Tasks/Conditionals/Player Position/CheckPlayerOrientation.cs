using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;


namespace SocialPresenceVR
{
    [TaskDescription("Comprueba si el jugador tiene en su campo de visión al NPC")]
    [TaskCategory("SocialPresenceVR/PlayerPosition")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}SeekIcon.png")]
    public class CheckPlayerOrientation : Conditional
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("Objeto que representa al jugador")]
        public SharedGameObject targetObject;

        [BehaviorDesigner.Runtime.Tasks.Tooltip("Ángulo en el que el jugador puede ver al NPC")]
        public SharedFloat ViewAngle = 60;//Ángulo de visión 

        private Transform targetTransform;//transform de la cámara

        public override void OnStart()
        {
            //Se obtiene el componente Transform del jugador
            targetTransform = targetObject.Value.GetComponent<Transform>();
        }

        /// <summary>
        /// Método que devuelve éxito si el jugador tiene en su campo de visión al NPC en un ángulo establecido
        /// </summary>
        /// <returns></returns>
        public override TaskStatus OnUpdate()
        {
            //Orientacion del jugador(eje Y)
            float targetOrientationY = targetTransform.rotation.eulerAngles.y;

            //Distancia entre el jugador y el NPC
            Vector3 distance = transform.position - targetTransform.position;

            //El cálculo de distancias solo aplica al eje XZ
            distance.y = 0;

            //Vector normalizado
            distance.Normalize();

            //Ángulo(En Quaternion) formado por la distancia entre el jugador y el NPC y el vector forward(0,0,1)
            Quaternion rotation = Quaternion.LookRotation(distance, Vector3.forward);

            //Transformación a ángulos de euler
            float transformedAngle = rotation.eulerAngles.y;

            //Se comprueba si la diferencia entre el ángulo distancia entre el jugador y el NPC 
            //y la orientación del jugador tiene un valor mayor o menor al ángulo de visión del jugador

            //Si la diferencia es mayor, el jugador no puede ver al NPC
            if (Mathf.Abs(Mathf.DeltaAngle(transformedAngle, targetOrientationY)) > ViewAngle.Value)            
                return TaskStatus.Failure;
            
            //Si la diferencia es menor, el jugador puede ver al NPC
            else           
                return TaskStatus.Success;           
        }
    }
}

