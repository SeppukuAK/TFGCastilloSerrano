using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SocialPresenceVR
{
    public class PlayerQuietState : MonoBehaviour
    {
        /// <summary>
        /// Minima cantidad en la que se asume movimiento
        /// </summary>
        public float Threshold;

        /// <summary>
        /// Tiempo de duracion en el cual se quiere comprobar si el jugador ha estado quieto
        /// </summary>
        public float CheckRate;

        /// <summary>
        /// Devuelve si el jugador está actualmente quieto
        /// </summary>
        public bool Quiet { get; set; }

        /// <summary>
        /// Transform del jugador
        /// </summary>
        private Transform targetObject;

        private Vector3 lastTargetPos;      // Posicion anterior del jugador
        private Vector3 actualTargetPos;    // Posicion actual del jugador

        private bool positionChanged, rotationChanged;  // Variables para la detección de la posicion o rotacion modificadas

        // Variables de tipo offset para observar la diferencia de posiciones y rotaciones
        private Vector3 offsetPos, offsetRot;
        private float offsetSum;    // Acumulacion del offset

        private float nextCheck;    // Siguiente momento en el que va a comprobarse si esta quieto

        /// <summary>
        /// Obtiene referencias
        /// </summary>
        private void Awake()
        {
            targetObject = Camera.main.transform;
        }

        // Start is called before the first frame update
        void Start()
        {
            Restart();
        }


        private void Restart()
        {
            //Se inicializa el siguiente instante en el que termina la comprobacion de si el jugador está quieto
            nextCheck = Time.time + CheckRate;

            //Se obtiene la posicion anterior
            lastTargetPos = new Vector3(targetObject.position.x, targetObject.position.y, targetObject.position.z);
            positionChanged = false;
            offsetSum = 0.0f;

            Quiet = false;
        }

        /// <summary>
        /// Método que devuelve éxito si el jugador se ha mantenido quieto durante un tiempo 
        /// </summary>
        private void Update()
        {
            //Si ha consumido el Quiet, se resetea y se vuelve a comprobar si se queda quieto
            if (Quiet)
                Restart();

            //Si se ha acabado el tiempo y el jugador no se ha movido en CheckRate, devuelve success
            else if (Time.time > nextCheck && !positionChanged)
                Quiet = true;

            //Si el jugador se mueve en el momento o despues de un tiempo su posición ha cambiado, vuelve a comprobar si se queda quieto
            else if (Time.time > nextCheck || positionChanged)
                Restart();

            else
            {
                //Se obtiene la nueva posición del jugador
                actualTargetPos = new Vector3(targetObject.position.x, targetObject.position.y, targetObject.position.z);

                //Se halla la diferencia de movimiento entre la posición actual y la anterior
                offsetPos = actualTargetPos - lastTargetPos;

                //Acumulacion de la variacion de movimiento a lo largo del tiempo
                offsetSum += offsetPos.magnitude;

                //Se comprueba si la diferencia de movimiento supera el threshold de movimiento establecido
                positionChanged = offsetSum > Threshold;

                //Se intercambian valores. La posición anterior ahora es la actual
                lastTargetPos = new Vector3(actualTargetPos.x, actualTargetPos.y, actualTargetPos.z);
            }
        }

    }
}