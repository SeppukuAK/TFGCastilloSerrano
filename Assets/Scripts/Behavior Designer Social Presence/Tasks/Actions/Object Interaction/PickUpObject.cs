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

        private GameObject hand;    // Mano del NPC
        private bool success;       // Booleana que indica si puede recoger el objeto correctamente

        /// <summary>
        /// Obtiene referencias
        /// </summary>
        public override void OnAwake()
        {
            hand = GetComponent<SP_NPC>().Hand;
        }

        public override void OnStart()
        {
            if (!Interactable.Value.GetComponent<XRBaseInteractable>() || !Interactable.Value.GetComponent<Rigidbody>())
            {
                Debug.LogError("Objeto a recoger no interactuable");
                success = false;
            }
            else
            {
                //Se une la posicion del objeto a la posicion de la mano         
                Interactable.Value.transform.SetParent(hand.transform, true);

                //Se resetean la posicion y la rotación del ingrediente 
                Interactable.Value.transform.localPosition = new Vector3(0, 0, 0);
                Interactable.Value.transform.localRotation = new Quaternion(0, 0, 0, 1);

                //Se obtiene el Rigidbody del objeto
                Rigidbody rb = Interactable.Value.GetComponent<Rigidbody>();

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
