using BehaviorDesigner.Runtime;
using UnityEngine.XR.Interaction.Toolkit;

namespace SocialPresenceVR
{
    [System.Serializable]
    public class SharedXRInteractable : SharedVariable<XRBaseInteractable>
    {
        public static implicit operator SharedXRInteractable(XRBaseInteractable value) { return new SharedXRInteractable { Value = value }; }
    }
}
