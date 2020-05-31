using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

namespace SocialPresenceVR
{
    /// <summary>
    /// Controlador de un NPC con Presencia Social
    /// </summary>
    public class SP_NPC : MonoBehaviour
    {
        /// <summary>
        /// Altura del jugador
        /// </summary>
        public float PlayerHeight { get; set; }

        /// <summary>
        /// Mano principal del NPC con la que agarra objetos
        /// </summary>
        public GameObject Hand { get; set; }

        /// <summary>
        /// Información actual del objeto interactuable agarrado
        /// </summary>
        public InteractableInfo GrabbedInteractable;

#if UNITY_EDITOR

        /// <summary>
        /// Animator controller asociado al NPC
        /// </summary>
        public AnimatorController AnimatorController { get; set; }

        public bool ResetAnimator { get; set; }
#endif
    }
}
