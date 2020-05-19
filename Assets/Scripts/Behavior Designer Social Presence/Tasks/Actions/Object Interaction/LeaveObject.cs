using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.XR.Interaction.Toolkit;

namespace SocialPresenceVR
{
    [TaskDescription("El NPC Suelta el objeto interactuable que sostiene en la mano")]
    [TaskCategory("SocialPresenceVR/ObjectInteraction")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}Play.png")]
    public class LeaveObject : Action
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("Objeto que representa al jugador")]
        public SharedGameObject TargetObject;           // Camara

        [BehaviorDesigner.Runtime.Tasks.Tooltip("Objeto que representa el objeto interactuable a soltar")]
        public SharedGameObject InteractableObject;     // Objecto interactuable

        private SP_NPC NPC;     // Controlador NPC

        private XRBaseInteractable interactable;        // Objecto interactuable
        private bool picked;                            // Booleana para comprobar que el jugador ha cogido el ingrediente (Simulación)
        private bool success;                           // Booleana que indica si puede recoger el objeto correctamente

        /// <summary>
        /// Obtiene referencias
        /// </summary>
        public override void OnAwake()
        {
            NPC = GetComponent<SP_NPC>();
        }

        /// <summary>
        /// Escucha cuando el jugador agarra el jugador que el NPC tiene en la mano
        /// Detecta errores
        /// </summary>
        public override void OnStart()
        {
            interactable = InteractableObject.Value.GetComponent<XRBaseInteractable>();

            if (!InteractableObject.Value || NPC.GrabbedInteractable.Interactable != interactable)
            {
                Debug.LogError("El objeto a entregar al jugador no está agarrado por el NPC");
                success = false;
            }
            else
            {
                success = true;
                picked = false;

                interactable.onSelectEnter.AddListener(OnPlayerGrabObject);
            }
        }

        /// <summary>
        /// Cuando el jugador agarra el objeto, se restablece el transform del objeto.
        /// Desliga el ingrediente que sostiene el NPC en la mano
        /// </summary>
        /// <param name="arg0"></param>
        private void OnPlayerGrabObject(XRBaseInteractor arg0)
        {
            picked = true;

            //Desuscripción a escuchar eventos de agarrar el objeto
            interactable.onSelectEnter.RemoveListener(OnPlayerGrabObject);

            // El ingrediente ya no está atado a la mano del NPC         
            interactable.transform.SetParent(NPC.GrabbedInteractable.Parent);

            //Rigidbody del objeto se restablece a sus propiedades anteriores
            Rigidbody rb = interactable.GetComponent<Rigidbody>();
            rb.isKinematic = NPC.GrabbedInteractable.IsKinematic;

            //Reset del objeto agarrado por NPC
            NPC.GrabbedInteractable.Interactable = null;
            NPC.GrabbedInteractable.Parent = null;
            NPC.GrabbedInteractable.IsKinematic = false;
        }


        public override TaskStatus OnUpdate()
        {
            if (!success)
                return TaskStatus.Failure;
            else if (picked)
                return TaskStatus.Success;
            else
                return TaskStatus.Running;
        }
    }
}

