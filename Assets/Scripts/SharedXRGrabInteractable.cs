using BehaviorDesigner.Runtime;
using UnityEngine.XR.Interaction.Toolkit;

namespace TFG
{
    [System.Serializable]
    public class SharedXRGrabInteractable : SharedVariable<XRGrabInteractable>
    {
        public static implicit operator SharedXRGrabInteractable(XRGrabInteractable value) { return new SharedXRGrabInteractable { Value = value }; }
    }
}
