using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace SocialPresenceVR
{
    [TaskDescription("Cambia el estado de los ojos del NPC")]
    [TaskCategory("SocialPresenceVR/EscapeRoom")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}Play.png")]
    public class ChangeJammoEyes : Action
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("Nuevo estado de los ojos. [0,normal], [1, angry], [2, happy], [3, dead]")]
        public SharedInt NewEyesState;

        private JammoEyesController jammoEyesController;

        public override void OnAwake()
        {
            jammoEyesController = GetComponent<JammoEyesController>();
            if (!jammoEyesController)
                Debug.LogError("JammoEyesController Component no asociado al NPC");
        }

        public override void OnStart()
        {
            jammoEyesController.ChangeEyes(NewEyesState.Value);
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }

    }
}
