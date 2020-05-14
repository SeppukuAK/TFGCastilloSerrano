using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace SocialPresenceVR
{
    [TaskDescription("Obtiene las 2 manos del jugador")]
    [TaskCategory("SocialPresenceVR/Init")]
    public class GetPlayerHands : Action
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("Mano izquierda")]
        public SharedGameObject LeftHand;

        [BehaviorDesigner.Runtime.Tasks.Tooltip("Mano derecha")]
        public SharedGameObject RightHand;

        public override void OnStart()
        {
            if (MasterController.Instance)
            {
                LeftHand.Value = MasterController.Instance.LeftDirectInteractor.gameObject;
                RightHand.Value = MasterController.Instance.RightDirectInteractor.gameObject;
            }
            else
                Debug.LogError("No hay manos");

        }

        public override TaskStatus OnUpdate()
        {
            if (LeftHand.Value != null && RightHand.Value != null)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }
    }
}