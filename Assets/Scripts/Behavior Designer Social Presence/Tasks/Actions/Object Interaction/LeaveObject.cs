using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.XR.Interaction.Toolkit;

namespace SocialPresenceVR
{
    /// <summary>
    /// TODO: Borrar simulación
    /// </summary>
    [TaskDescription("El NPC Suelta el objeto interactuable que sostiene en la mano")]
    [TaskCategory("SocialPresenceVR/ObjectInteraction")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}Play.png")]
    public class LeaveObject : Action
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("Objeto que representa al jugador")]
        public SharedGameObject TargetObject;   //Camara

        [BehaviorDesigner.Runtime.Tasks.Tooltip("Objeto que representa el objeto interactuable a soltar")]
        public SharedGameObject InteractableObject;     //Ingrediente

        //Referencias
        private SP_NPC NPC;

        private XRBaseInteractable interactable;
        private bool picked;    //Booleana para comprobar que el jugador ha cogido el ingrediente (Simulación)
        private bool success;

        /// <summary>
        /// Obtiene referencias
        /// </summary>
        public override void OnAwake()
        {
            NPC = GetComponent<SP_NPC>();
        }

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
        /// Cuando el jugador agarra el objeto, se restablece el transform del objeto
        /// </summary>
        /// <param name="arg0"></param>
        private void OnPlayerGrabObject(XRBaseInteractor arg0)
        {
            picked = true;
            interactable.onSelectEnter.RemoveListener(OnPlayerGrabObject);

            // El ingrediente ya no está atado a la mano del NPC         
            interactable.transform.SetParent(NPC.GrabbedInteractable.Parent);

            //Rigidbody del objeto
            Rigidbody rb = interactable.GetComponent<Rigidbody>();   
            rb.isKinematic = NPC.GrabbedInteractable.IsKinematic;

            NPC.GrabbedInteractable.Interactable = null;
            NPC.GrabbedInteractable.Parent = null;
            NPC.GrabbedInteractable.IsKinematic = false;
        }

        //Método encargado de desligar el ingrediente que sostiene el NPC en la mano
        public void DetachIngredient()
        {
            ////*****TODO: ESTO ES PARA SIMULAR*****
            //// El ingrediente ya no está atado a la mano del NPC         
            //// ingredient.Value.transform.parent = null;

            ////Se une la posicion del objeto a la posicion de la cámara(jugador)        
            //Interactable.Value.transform.SetParent(TargetObject.Value.transform, true);

            ////Se resetean la posicion y la rotación del ingrediente para que estén delante del jugador
            //Interactable.Value.transform.localPosition = new Vector3(0.0f, 0.0f, 2.0f);
            //Interactable.Value.transform.localRotation = new Quaternion(0, 0, 0, 1);
            ////*****TODO: ESTO ES PARA SIMULAR*****
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

