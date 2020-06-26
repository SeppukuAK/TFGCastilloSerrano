using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace SocialPresenceVR
{
    [TaskDescription("El jugador esta quieto")]
    [TaskCategory("SocialPresenceVR/PlayerPosition")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}Play.png")]
    public class IsPlayerStill : Conditional
    {
        /// <summary>
        /// Referencia al estado de quieto del jugador
        /// </summary>
        private PlayerQuietState playerQuietState;

        /// <summary>
        /// Obtiene referencias
        /// </summary>
        public override void OnAwake()
        {
            playerQuietState = Camera.main.GetComponentInParent<PlayerQuietState>();
        }

        public override TaskStatus OnUpdate()
        {
            if (playerQuietState.Quiet)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }
    }
}
