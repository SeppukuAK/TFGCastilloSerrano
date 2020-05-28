using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace SocialPresenceVR
{
    /// <summary>
    /// Clase base usada para escuchar los eventos de interacción continua con un objeto
    /// </summary>
    public abstract class IsPlayerInteracting : Conditional
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("Objeto con el que se interactúa")]
        public SharedGameObject XRInteractable;

        private bool interaction;
        protected XRBaseInteractable interactable;    //Interactable al que se está suscrito a sus eventos actualmente

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
        /// Método que tiene que ser suscrito.
        /// Deja de escuchar al interactable actual
        /// </summary>
        protected abstract void RemoveListeners();

        public override void OnAwake()
        {
            interactable = null;
        }

        /// <summary>
        /// Añade los eventos correspondientes
        /// </summary>
        public override void OnStart()
        {
            //Caso en el que no tiene que detectar ningun interactable
            if (XRInteractable.Value == null)
            {
                interaction = false;

                //Detección de si está detectando alguno actualmente
                if (interactable != null)
                {
                    RemoveListeners();
                    interactable = null;
                }
            }
            else
            {
                XRBaseInteractable newInteractable = XRInteractable.Value.GetComponent<XRBaseInteractable>();

                //Deteción de si el objeto pasado no es un interactable
                if (!newInteractable)
                    Debug.LogError("El objeto pasado como parámetro no es un interactuable");

                //Caso en el que tiene que detectar algún interactable y no es el que se está detectando actualmente
                else if (interactable != newInteractable)
                {
                    interaction = false;

                    //Si se estaba detectando alguno anteriormente, se deja de escuchar a sus eventos
                    if (interactable != null)
                        RemoveListeners();

                    interactable = newInteractable;
                    AddOnListener();
                    AddOffListener();
                }
            }
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