using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace SocialPresenceVR
{
    /// <summary>
    /// TODO: Un poco raro que se suscriba al primer objeto que encuentre
    /// TODO: No te sorprendas si no va al poner muchas condiciones de estas en diferentes lados
    /// </summary>
    [TaskDescription("Devuelve si el jugador se ha teletransportado a alguno de los teletransportes")]
    [TaskCategory("SocialPresenceVR/EnviromentInteractions")]
    public class OnPlayerTeleport : Conditional
    {
        /// <summary>
        /// Booleana que contiene si se el jugador se ha teletransportado este frame
        /// </summary>
        private static bool teleported = false;

        /// <summary>
        /// Contiene si se ha inicializado ya la suscripción a los listeners de teletransporte
        /// </summary>
        private static bool initialized = false;

        /// <summary>
        /// Se suscribe a los eventos de teletransporte de todos los anchors en la escena
        /// </summary>
        public override void OnAwake()
        {
            if (!initialized)
            {
                initialized = true;
                TeleportationAnchor[] teleports = GameObject.FindObjectsOfType<TeleportationAnchor>();

                foreach (TeleportationAnchor teleport in teleports)
                    teleport.onSelectExit.AddListener(OnTeleport);
            }
        }

        private void OnTeleport(XRBaseInteractor arg0)
        {
            StartCoroutine(TeleportRoutine());
        }

        private static IEnumerator TeleportRoutine()
        {
            teleported = true;
            yield return new WaitForEndOfFrame();
            teleported = false;
        }

        public override TaskStatus OnUpdate()
        {
            if (teleported)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }
    }
}