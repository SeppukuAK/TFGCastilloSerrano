using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;
using UnityEngine;

namespace SocialPresenceVR
{
    /// <summary>
    /// Clase base usada para escuchar los eventos de interacción con un objeto
    /// TODO: Puede que haya problemas en el futuro con interacion= true, no lo tengo claro
    /// </summary>
    public abstract class OnPlayerInteraction : Conditional
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("Objeto con el que se interactúa")]
        public SharedXRInteractable XRInteractable;

        private bool interaction;

        /// <summary>
        /// Método que tiene que ser suscrito.
        /// Añade el evento correspndiente
        /// </summary>
        protected abstract void AddListener();

        /// <summary>
        /// Añade el evento correspondiente
        /// </summary>
        public override void OnAwake()
        {
            interaction = false;
            AddListener();
        }

        /// <summary>
        /// Función llamada cuando se produce el evento especificado por el hijo
        /// </summary>
        /// <param name="arg0"></param>
        protected void OnInteraction(XRBaseInteractor arg0)
        {
            StartCoroutine(InteractionRoutine());
        }

        /// <summary>
        /// Establece la interacción con el objeto solo en este frame
        /// </summary>
        /// <returns></returns>
        private IEnumerator InteractionRoutine()
        {
            interaction = true;
            yield return new WaitForEndOfFrame();
            interaction = false;
        }

        /// <summary>
        /// Cuando ha devuelto success, establece interaction a false
        /// </summary>
        public override void OnEnd()
        {
            interaction = false;
        }

        /// <summary>
        /// Devuelve si ha realizado el tipo de interacción especificada por el hijo
        /// </summary>
        /// <returns></returns>
        public override TaskStatus OnUpdate()
        {
            if (interaction)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }

    }
}