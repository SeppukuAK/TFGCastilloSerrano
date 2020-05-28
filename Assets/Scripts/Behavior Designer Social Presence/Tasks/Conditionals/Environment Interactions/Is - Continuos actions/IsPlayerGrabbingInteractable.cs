using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.XR.Interaction.Toolkit;

namespace SocialPresenceVR
{
    [TaskDescription("Devuelve si el jugador está agarrando el objeto establecido")]
    [TaskCategory("SocialPresenceVR/EnviromentInteractions/ContinuousActions")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}Play.png")]
    public class IsPlayerGrabbingInteractable : Conditional
    {
        [Tooltip("Objeto a detectar si está siendo agarrado")]
        public SharedGameObject XRInteractable;

        public override TaskStatus OnUpdate()
        {
            if (XRInteractable.Value)
            {
                XRBaseInteractable interactable = XRInteractable.Value.GetComponent<XRBaseInteractable>();
                if (interactable && interactable.isSelected)
                    return TaskStatus.Success;

            }
            return TaskStatus.Failure;
        }
    }
}