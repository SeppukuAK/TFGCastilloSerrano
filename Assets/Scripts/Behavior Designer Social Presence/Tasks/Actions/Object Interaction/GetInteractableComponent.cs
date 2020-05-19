using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.XR.Interaction.Toolkit;

namespace SocialPresenceVR
{
    [TaskDescription("Obtiene el componente XRBaseInteractable del GameObject")]
    [TaskCategory("SocialPresenceVR/ObjectInteraction")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}Play.png")]
    public class GetInteractableComponent : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;

        [Tooltip("The component")]
        [RequiredField]
        public SharedXRInteractable storeValue;

        public override TaskStatus OnUpdate()
        {
            storeValue.Value = targetGameObject.Value.GetComponent<XRBaseInteractable>();
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            storeValue.Value = null;
        }
    }
}