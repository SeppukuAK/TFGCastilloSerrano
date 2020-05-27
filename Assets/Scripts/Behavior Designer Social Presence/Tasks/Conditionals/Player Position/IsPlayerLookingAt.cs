using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using System.Collections;


namespace SocialPresenceVR
{
    /// <summary>
    /// TODO: Eliminar debug, Refinarlo para que vaya mejor (Mayor complejidad). 
    /// TODO: Es posible que se pueda unificar con mirar objeto.
    /// </summary>
    [TaskDescription("Devuelve si el jugador está mirando a un GameObject durante 'CheckRate' segundos")]
    [TaskCategory("SocialPresenceVR/PlayerPosition")]
    public class IsPlayerLookingAt : Conditional
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("Objeto que representa al jugador")]
        public SharedGameObject Player;

        [BehaviorDesigner.Runtime.Tasks.Tooltip("Objetos que puede mirar el jugador y generan una respuesta en el NPC")]
        public SharedGameObject Object;

        [BehaviorDesigner.Runtime.Tasks.Tooltip("Distancia máxima desde la que se detecta que está mirando al objeto")]
        public SharedFloat LookingAtDistance;

        [BehaviorDesigner.Runtime.Tasks.Tooltip("Tiempo de duracion de comprobacion de si esta mirando")]
        public float CheckRate;

        private Vector3 leftOffset, rightOffset;    //Posiciones de los ojos

        private float nextCheck;    //Instante de tiempo al que si llega, se considera que el jugador ha estado mirando al objeto

        //Variables de control de la rutina
        private bool isLooking;
        private bool isChecking;

        /// <summary>
        /// Inicialización variables de control y posiciones de los ojos
        /// </summary>
        public override void OnAwake()
        {
            isChecking = false;
            isLooking = false;

            //Inicializacion de las posiciones de los ojos
            leftOffset = new Vector3(-0.1f, 0.1f, 0.1f);
            rightOffset = new Vector3(0.1f, 0.1f, 0.1f);
        }


        private IEnumerator LookingRoutine()
        {
            isChecking = true;

            //Se inicializa el siguiente instante en el que termina la comprobacion de si el jugador está mirando a un objeto
            nextCheck = Time.time + CheckRate;

            //Mientras siga mirando durante CheckRate Seconds
            while (Time.time <= nextCheck && RaycastCollideWithObject(Player.Value.transform))
                yield return new WaitForFixedUpdate();

            //Está mirando
            isLooking = Time.time >= nextCheck;

            isChecking = false;

            yield return null;
        }

        public override void OnStart()
        {
            nextCheck = Time.time + CheckRate;
        }

        public override void OnEnd()
        {
            isLooking = false;
        }

        /// <summary>
        /// Método que devuelve éxito si el jugador está mirando un objeto durante un tiempo
        /// </summary>
        /// <returns></returns>
        public override TaskStatus OnUpdate()
        {
            if (isLooking)
                return TaskStatus.Success;

            else
            {
                //No se está comprobando, empieza comprobación de si está mirando
                if (!isChecking && RaycastCollideWithObject(Player.Value.transform))
                    StartCoroutine(LookingRoutine());

                return TaskStatus.Failure;
            }

        }

        /// <summary>
        /// Método que comprueba si los rayos situados en los ojos del jugador colisionan con un objeto 
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private bool RaycastCollideWithObject(Transform player)
        {
            RaycastHit hit;

            //Se comprueba si hay colisión
            if (Physics.Raycast(player.position + leftOffset, player.TransformDirection(Vector3.forward), out hit, LookingAtDistance.Value) || Physics.Raycast(player.position + rightOffset, player.TransformDirection(Vector3.forward), out hit, LookingAtDistance.Value))
            {
                Collider[] colliders = Object.Value.GetComponentsInChildren<Collider>();

                //Recorrido de todos los posibles colliders del objeto
                foreach (Collider collider in colliders)
                {
                    if (hit.collider == collider)
                    {
                        //Se dibujan los rayos cuando se está viendo un objeto(Verde)
                        Debug.DrawRay(player.position + leftOffset, player.TransformDirection(Vector3.forward) * hit.distance, Color.green);
                        Debug.DrawRay(player.position + rightOffset, player.TransformDirection(Vector3.forward) * hit.distance, Color.green);

                        return true;
                    }
                }
            }
            //Se dibujan los rayos tambien cuando no se está viendo un objeto(Rojo)
            Debug.DrawRay(player.position + leftOffset, player.TransformDirection(Vector3.forward) * LookingAtDistance.Value, Color.red);
            Debug.DrawRay(player.position + rightOffset, player.TransformDirection(Vector3.forward) * LookingAtDistance.Value, Color.red);

            return false;
        }
    }
}
