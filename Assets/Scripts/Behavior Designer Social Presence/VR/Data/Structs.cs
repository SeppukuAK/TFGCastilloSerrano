using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace SocialPresenceVR
{
    /// <summary>
    /// Guarda información del estado de un objeto interactuable
    /// </summary>
    public struct InteractableInfo
    {
        public XRBaseInteractable Interactable;
        public Transform Parent;
        public bool IsKinematic;
    }
}