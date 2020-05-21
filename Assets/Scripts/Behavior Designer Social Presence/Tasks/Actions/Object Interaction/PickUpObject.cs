using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.XR.Interaction.Toolkit;

namespace SocialPresenceVR
{
    [TaskDescription("El NPC coge un objeto interactuable")]
    [TaskCategory("SocialPresenceVR/ObjectInteraction")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}Play.png")]
    public class PickUpObject : Action
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("Objeto interactuable a recoger")]
        public SharedGameObject Interactable;

        //Referencias
        private SP_NPC NPC;         // Controlador NPC
        private GameObject hand;    // Mano del NPC

        private bool success;       // Booleana que indica si puede recoger el objeto correctamente

        /// <summary>
        /// Obtiene referencias
        /// </summary>
        public override void OnAwake()
        {
            NPC = GetComponent<SP_NPC>();
            hand = NPC.Hand;
        }

        /// <summary>
        /// Liga el objeto a la mano del NPC.
        /// Detecta errores.
        /// </summary>
        public override void OnStart()
        {
            Rigidbody rb = Interactable.Value.GetComponent<Rigidbody>();
            XRBaseInteractable interactable = Interactable.Value.GetComponent<XRBaseInteractable>();

            if (!interactable || !rb)
            {
                Debug.LogError("Objeto a recoger no interactuable");
                success = false;
            }
            else
            {
                //Se guarda la información del Objeto
                InteractableInfo interactableInfo = new InteractableInfo
                {
                    Interactable = interactable,
                    Parent = interactable.transform.parent,
                    IsKinematic = rb.isKinematic
                };
                NPC.GrabbedInteractable = interactableInfo;

                //Se une la posicion del objeto a la posicion de la mano         
                Interactable.Value.transform.SetParent(hand.transform, true);

                //Se resetean la posicion y la rotación del ingrediente 
                Interactable.Value.transform.localPosition = new Vector3(0, 0, 0);
                Interactable.Value.transform.localRotation = new Quaternion(0, 0, 0, 1);

                //El objeto deja de responder a la física
                rb.isKinematic = true;

                //Objeto recogido correctamente
                success = true;
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (success)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }
    }
}
