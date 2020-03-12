using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;


namespace TFG
{
    [TaskDescription("Comprueba la rotacion del NPC y del jugador")]
    [TaskCategory("TFG")]
    public class CheckOrientation : Conditional
    {
        private Transform NPCTransform;//transform del NPC
        private Transform targetTransform;//transform de la cámara

        [BehaviorDesigner.Runtime.Tasks.Tooltip("The object that we are searching for")]
        public SharedGameObject targetObject;//transform de la cámara

        public SharedFloat ViewAngle = 60;
        private float targetOrientationY;
        public override void OnAwake()
        {
            NPCTransform = GetComponent<Transform>();
        }

        public override void OnStart()
        {
            targetTransform = targetObject.Value.GetComponent<Transform>();




        }
        public override TaskStatus OnUpdate()
        {
            targetOrientationY = targetTransform.rotation.eulerAngles.y;//Orientacion del jugador

            Vector3 distance = targetTransform.position - transform.position;
            distance.Normalize();//vector normalizada

            float angle = Vector3.Angle(new Vector3(targetTransform.position.x, 0, targetTransform.position.z), new Vector3(transform.position.x, 0, transform.position.z));

            //TODO: Usar https://docs.unity3d.com/ScriptReference/Mathf.DeltaAngle.html
            Debug.Log("targetOrientation" + targetOrientationY);

            if (targetOrientationY - angle < 120)
                Debug.Log("No me está mirando");

            return TaskStatus.Success;


        }
    }
}

