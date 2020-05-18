using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine.XR.Interaction.Toolkit;

namespace SocialPresenceVR
{
    /// <summary>
    /// Controlador de un NPC con Presencia Social
    /// </summary>
    public class SP_NPC : MonoBehaviour
    {
        /// <summary>
        /// Animator controller asociado al NPC
        /// </summary>
        public AnimatorController AnimatorController { get; set; }

        /// <summary>
        /// Altura del jugador
        /// </summary>
        public float PlayerHeight { get; set; }

        /// <summary>
        /// Mano principal del NPC con la que agarra objetos
        /// </summary>
        public GameObject Hand { get; set; }

        /// <summary>
        /// Ruta donde se encuentra el animator controller creado para el NPC
        /// </summary>
        public string AnimatorControllerPath { get; set; }

        /// <summary>
        /// Información actual del objeto interactuable agarrado
        /// </summary>
        public InteractableInfo GrabbedInteractable;

        /// <summary>
        /// Destruye el asset del animator controller
        /// </summary>
        private void OnDestroy()
        {
            AssetDatabase.DeleteAsset(AnimatorControllerPath);
        }

    }
}
