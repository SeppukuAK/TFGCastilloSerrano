using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace SocialPresenceVR
{
    /// <summary>
    /// TODO: Eliminar debug, Refinarlo para que vaya mejor (Mayor complejidad). 
    /// TODO: Es posible que se pueda unificar con mirar objeto.
    /// </summary>
    [TaskDescription("Devuelve si el jugador está señalando a un GameObject")]
    [TaskCategory("SocialPresenceVR/Gestures/Hand")]
    public class IsPlayerPointingTo : Conditional
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("Objeto que tiene que señalar el jugador")]
        public SharedGameObjectList Objects;

        [BehaviorDesigner.Runtime.Tasks.Tooltip("Distancia máxima desde la que se detecta que está señalando al objeto")]
        public SharedFloat PointingDistance;

        private ControllerState leftHand;
        private ControllerState rightHand;

        /// <summary>
        /// Obtiene referencias a variables
        /// </summary>
        public override void OnStart()
        {
            leftHand = MasterController.Instance.LeftDirectInteractor.GetComponent<ControllerState>();
            rightHand = MasterController.Instance.RightDirectInteractor.GetComponent<ControllerState>();
        }

        /// <summary>
        /// Comprueba si el jugador está señalando al objeto
        /// </summary>
        /// <returns></returns>
        public override TaskStatus OnUpdate()
        {
            if (leftHand.HandState == HandState.POINTING && RaycastCollideWithObject(leftHand.transform))
                return TaskStatus.Success;
            else if (rightHand.HandState == HandState.POINTING && RaycastCollideWithObject(rightHand.transform))
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }

        /// <summary>
        /// Devuelve si el rayo desde una mano colisiona con el objeto
        /// </summary>
        /// <param name="hand"></param>
        /// <returns></returns>
        private bool RaycastCollideWithObject(Transform hand)
        {
            RaycastHit hit;

            //Colisiona con algo
            if (Physics.Raycast(hand.position, hand.TransformDirection(Vector3.forward), out hit, PointingDistance.Value))
            {
                //Recorrido de cada uno de los objetos asociados
                foreach (GameObject item in Objects.Value)
                {
                    Collider[] colliders = item.GetComponentsInChildren<Collider>();

                    //Recorrido de todos los colliders del objeto
                    foreach (Collider collider in colliders)
                    {
                        if (hit.collider == collider)
                        {
                            Debug.DrawRay(hand.position, hand.TransformDirection(Vector3.forward) * hit.distance, Color.green);
                            return true;
                        }
                    }
                }
            }

            Debug.DrawRay(hand.position, hand.TransformDirection(Vector3.forward) * PointingDistance.Value, Color.red);
            return false;
        }

    }
}
