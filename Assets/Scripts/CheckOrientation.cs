using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;


namespace TFG
{
    [TaskDescription("Comprueba la rotacion del NPC y del jugador")]
    [TaskCategory("TFG")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}SeekIcon.png")]
    public class CheckOrientation : Conditional
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The object that we are searching for")]
        public SharedGameObject targetObject;//Cámara

        [BehaviorDesigner.Runtime.Tasks.Tooltip("The angle where the player can see the NPC")]
        public SharedFloat ViewAngle = 60;//Ángulo de visión 

        private Transform targetTransform;//transform de la cámara

        public override void OnStart()
        {
            targetTransform = targetObject.Value.GetComponent<Transform>();//Se obtiene el componente Transform del jugador
        }

        public override TaskStatus OnUpdate()
        {
           float  targetOrientationY = targetTransform.rotation.eulerAngles.y;//Orientacion del jugador(eje Y)

            Vector3 distance = transform.position - targetTransform.position;//Distancia entre el jugador y el NPC
            distance.y = 0;//El cálculo de distancias solo aplica al eje XZ
            distance.Normalize();//Vector normalizado

            Quaternion rotation = Quaternion.LookRotation(distance, Vector3.forward);//Ángulo(En Quaternion) formado por la distancia entre el jugador y el NPC y el vector forward(0,0,1)
            float transformedAngle = rotation.eulerAngles.y;//Transformación a ángulos de euler

            //Se comprueba si la diferencia entre el ángulo distancia entre el jugador y el NPC y la orientación del jugador tiene un valor mayor o menor al  ángulo de visión del jugador

            //Si la diferencia es mayor, el jugador no puede ver al NPC
            if (Mathf.Abs(Mathf.DeltaAngle(transformedAngle, targetOrientationY)) > ViewAngle.Value)
            {
                //Debug.Log("El jugador puede ver al NPC");
                return TaskStatus.Failure;
            }
            //Si la diferencia es menor, el jugador puede ver al NPC
            else
            {
                //Debug.Log("El jugador NO puede ver al NPC");
                return TaskStatus.Success;
            }
        }
    }
}

