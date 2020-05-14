using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace SocialPresenceVR
{
    [TaskDescription("Obtiene la cabeza del jugador")]
    [TaskCategory("SocialPresenceVR/Init")]
    public class GetPlayerHead : Action
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("Cabeza del jugador")]
        public SharedGameObject Head;

        public override void OnStart()
        {
            if (Camera.main)
                Head.Value = Camera.main.gameObject;
            else
                Debug.LogError("No hay cámara");
        }

        public override TaskStatus OnUpdate()
        {
            if (Head.Value != null)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }
    }
}