using UnityEngine;

namespace SocialPresenceVR
{
    /// <summary>
    /// Componente encargado de manejar el estado de los ojos del NPC
    /// </summary>
    public class JammoEyesController : MonoBehaviour
    {
        public enum EyePosition { normal, angry, happy, dead }

        public Material[] EyeMaterials;

        private Renderer eyesRenderer;      //Referencia al renderer de los ojos

        /// <summary>
        /// Obtiene referencia al renderer de los ojos
        /// </summary>
        private void Awake()
        {
            eyesRenderer = null;
            Renderer[] characterMaterials = GetComponentsInChildren<Renderer>();

            int i = 0;
            while (eyesRenderer == null)
            {
                if (characterMaterials[i].transform.CompareTag("PlayerEyes"))
                    eyesRenderer = characterMaterials[i];

                i++;
            }

            if (!eyesRenderer)
                Debug.LogError("No se encuentran los ojos del NPC");
        }

        /// <summary>
        /// Cambia el material, lo que hace que cambie la forma de los ojos y su color
        /// </summary>
        /// <param name="eyesState"></param>
        public void ChangeEyes(int eyesState)
        {
            eyesRenderer.material = EyeMaterials[eyesState];
        }
    }
}
