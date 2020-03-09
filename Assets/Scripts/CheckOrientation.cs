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
        private Vector3 NPCOrientation;
        private Vector3 targetOrientation;
        public override void OnAwake()
        {
            NPCTransform = GetComponent<Transform>();
        }

        public override void OnStart()
        {
            targetTransform = targetObject.Value.GetComponent<Transform>();

            NPCOrientation = NPCTransform.rotation.eulerAngles;
            targetOrientation = targetTransform.rotation.eulerAngles;
        }
        public override TaskStatus OnUpdate()
        {
            Debug.Log(targetOrientation);
            float idealOrientation = NPCOrientation.y + 180;
            if (targetOrientation.y >= idealOrientation - ViewAngle.Value && targetOrientation.y <= idealOrientation + ViewAngle.Value)
            {
                Debug.Log("Nos estamos viendo");
                return TaskStatus.Success;
            }

            else
            {
                Debug.Log("No nos estamos viendo");

                return TaskStatus.Failure;
            }
        }
    }
}

