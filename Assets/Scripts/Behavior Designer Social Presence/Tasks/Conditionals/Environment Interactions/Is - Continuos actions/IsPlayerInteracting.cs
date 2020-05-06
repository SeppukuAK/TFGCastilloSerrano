using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.XR.Interaction.Toolkit;

namespace TFG
{
    /// <summary>
    /// Clase base usada para escuchar los eventos de interacción con un objeto
    /// </summary>
    public abstract class IsPlayerInteracting : Conditional
    {
        [Tooltip("Objeto con el que se interactúa")]
        public SharedXRInteractable XRInteractable;

        private bool interaction;

        /// <summary>
        /// Método que tiene que ser suscrito.
        /// Añade el evento de empezar interacción correspndiente
        /// </summary>
        protected abstract void AddOnListener();

        /// <summary>
        /// Método que tiene que ser suscrito.
        /// Añade el evento de acabar interacción correspndiente
        /// </summary>
        protected abstract void AddOffListener();
        
        /// <summary>
        /// Añade los eventos correspondientes
        /// </summary>
        public override void OnAwake()
        {
            interaction = false;
            AddOnListener();
            AddOffListener();
        }

        /// <summary>
        /// Función llamada cuando se produce el evento de empezar interacción especificado por el hijo
        /// </summary>
        /// <param name="arg0"></param>
        protected void On(XRBaseInteractor arg0)
        {
            interaction = true;
        }

        /// <summary>
        /// Función llamada cuando se produce el evento de acabar interacción especificado por el hijo
        /// </summary>
        /// <param name="arg0"></param>
        protected void Off(XRBaseInteractor arg0)
        {
            interaction = false;
        }

        /// <summary>
        /// Devuelve si está realizando el tipo de interacción especificada por el hijo
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