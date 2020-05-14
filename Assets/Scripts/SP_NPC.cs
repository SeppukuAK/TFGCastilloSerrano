using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

namespace TFG
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
        /// Destruye el asset del animator controller
        /// </summary>
        private void OnDestroy()
        {
            AssetDatabase.DeleteAsset(AnimatorControllerPath);
        }

    }
}
