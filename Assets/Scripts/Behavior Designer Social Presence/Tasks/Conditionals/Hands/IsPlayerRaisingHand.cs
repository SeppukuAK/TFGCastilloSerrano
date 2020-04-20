using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace TFG
{
    /// <summary>
    /// TODO: Añadir complejidad: Mano cerrada, abierta o señalar
    /// </summary>
    [TaskDescription("Devuelve si el jugador está levantando una mano.")]
    [TaskCategory("TFG")]
    public class IsPlayerRaisingHand : Conditional
    {
        //[BehaviorDesigner.Runtime.Tasks.Tooltip("¿Tiene que tener ambas manos levantadas?")]
        //public SharedBool Both;

        [BehaviorDesigner.Runtime.Tasks.Tooltip("Diferencia en la coordenada Y entre mano y cabeza para que el gesto sea detectado")]
        public SharedFloat Diff;

        private Transform head;
        private Transform leftHand;
        private Transform rightHand;

        /// <summary>
        /// Obtiene referencias a variables
        /// </summary>
        public override void OnStart()
        {
            head = Camera.main.transform;
            leftHand = MasterController.Instance.LeftDirectInteractor.transform;
            rightHand = MasterController.Instance.RightDirectInteractor.transform;
        }

        /// <summary>
        /// Comprueba si el jugador está levantando la mano
        /// </summary>
        /// <returns></returns>
        public override TaskStatus OnUpdate()
        {
            bool leftUp = IsHandUp(leftHand);
            bool rightUp = IsHandUp(rightHand);

            //Solo la mano izquierda levantada
            if (leftUp && !rightUp)
            {
                HandState handState = MyMasterController.MyInstance.LeftHandState;
                if (handState == HandState.OPEN || handState == HandState.POINTING)
                    return TaskStatus.Success;
            }

            //Solo la mano derecha levantada
            else if (!leftUp && rightUp)
            {
                HandState handState = MyMasterController.MyInstance.RightHandState;
                if (handState == HandState.OPEN || handState == HandState.POINTING)
                    return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }

        /// <summary>
        /// Devuelve si la mano está levantada
        /// </summary>
        /// <param name="hand"></param>
        /// <returns></returns>
        private bool IsHandUp(Transform hand)
        {
            return ((hand.position.y - head.position.y) >= Diff.Value);
        }
    }
}
