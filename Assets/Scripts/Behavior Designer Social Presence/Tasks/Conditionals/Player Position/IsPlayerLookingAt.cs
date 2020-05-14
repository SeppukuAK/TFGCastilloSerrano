using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace SocialPresenceVR
{
    /// <summary>
    /// TODO: Eliminar debug, Refinarlo para que vaya mejor, problemas de FPS?
    /// </summary>
    [TaskDescription("Devuelve si el jugador está mirando a un GameObject")]
    [TaskCategory("SocialPresenceVR/PlayerPosition")]
    public class IsPlayerLookingAt : Conditional
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("Objeto que representa al jugador")]
        public SharedGameObject targetObject;

        [BehaviorDesigner.Runtime.Tasks.Tooltip("Objetos que puede mirar el jugador y generan una respuesta en el NPC")]
        public SharedGameObjectList Objects;

        [BehaviorDesigner.Runtime.Tasks.Tooltip("Distancia máxima desde la que se detecta que está mirando al objeto")]
        public SharedFloat LookingAtDistance;

        private Transform targetTransform;//transform del jugador
        private Vector3 leftOffset, rightOffset;//Posiciones de los ojos

        //Variables para el contador
        private float nextCheck;//Siguiente momento en el que va a comprobarse si esta mirando
        public float checkRate;//Tiempo de duracion de comprobacion de si esta mirando

        public override void OnStart()
        {
            //Inicializacion de las posiciones de los ojos
            leftOffset = new Vector3(-0.1f, 0.1f, 0.1f);
            rightOffset = new Vector3(0.1f, 0.1f, 0.1f);

            //Se obtiene el transform del jugador
            targetTransform = targetObject.Value.transform;

            //Se inicializa el siguiente instante en el que termina la comprobacion de si el jugador está mirando a un objeto
            nextCheck = Time.time + checkRate;
        }

        /// <summary>
        /// Método que devuelve éxito si el jugador está mirando un objeto durante un tiempo
        /// </summary>
        /// <returns></returns>
        public override TaskStatus OnUpdate()
        {
            if (Time.time > nextCheck && RaycastCollideWithObject(targetTransform))
                return TaskStatus.Success;

            //Si el jugador deja de mirar a un objeto o ha acabado el tiempo y no ha estado mirando a un objeto, devuelve fallo
            else if (Time.time > nextCheck || !RaycastCollideWithObject(targetTransform))
                return TaskStatus.Failure;
            else
                return TaskStatus.Running;
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
                //Recorrido de cada uno de los objetos asociados
                foreach (GameObject item in Objects.Value)
                {
                    Collider[] colliders = item.GetComponentsInChildren<Collider>();

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
            }
            //Se dibujan los rayos tambien cuando no se está viendo un objeto(Rojo)
            Debug.DrawRay(player.position + leftOffset, player.TransformDirection(Vector3.forward) * LookingAtDistance.Value, Color.red);
            Debug.DrawRay(player.position + rightOffset, player.TransformDirection(Vector3.forward) * LookingAtDistance.Value, Color.red);

            return false;
        }
    }
}
