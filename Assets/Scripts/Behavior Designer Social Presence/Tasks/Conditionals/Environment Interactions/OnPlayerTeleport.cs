using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace SocialPresenceVR
{
    [TaskDescription("Devuelve si el jugador se ha teletransportado a alguno de los teletransportes")]
    [TaskCategory("SocialPresenceVR/EnviromentInteractions/EventActions")]
    public class OnPlayerTeleport : Conditional
    {
        private static bool teleported;

        /// <summary>
        /// Se suscribe a los eventos de teletransporte de todos los anchors en la escena
        /// </summary>
        public override void OnAwake()
        {
            teleported = false;
            TeleportationAnchor[] teleports = GameObject.FindObjectsOfType<TeleportationAnchor>();

            foreach (TeleportationAnchor teleport in teleports)
                teleport.onSelectExit.AddListener(OnTeleport);
        }

        private void OnTeleport(XRBaseInteractor arg0)
        {
            StartCoroutine(TeleportRoutine());
        }

        private IEnumerator TeleportRoutine()
        {
            teleported = true;
            yield return new WaitForEndOfFrame();
            teleported = false;
        }

        public override TaskStatus OnUpdate()
        {
            if (teleported)
            {
                teleported = false;
                return TaskStatus.Success;
            }
            else
                return TaskStatus.Failure;
        }
    }
}