using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.XR.Interaction.Toolkit;

namespace SocialPresenceVR
{
    /// <summary>
    /// TODO: No está hecho de la mejor manera, pero funciona
    /// </summary>
    [TaskDescription("Devuelve si el libro ha sido colocado y abierto")]
    [TaskCategory("SocialPresenceVR/EscapeRoom")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}Play.png")]
    public class IsBookOpen : Conditional
    {
        private XRExclusiveSocketInteractor lectern;    //Atril
        private bool bookOpen;

        /// <summary>
        /// Obtiene referencia al atril y se suscribe a cuando sea abierta
        /// </summary>
        public override void OnAwake()
        {
            lectern = null;
            bookOpen = false;

            Object[] objects = GameObject.FindObjectsOfType(typeof(XRExclusiveSocketInteractor));

            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i].name == "BookHolder")
                    lectern = (XRExclusiveSocketInteractor)objects[i];
            }

            if (!lectern)
                Debug.LogError("lectern no encontrado en la escena");

            lectern.onSelectEnter.AddListener(OnBookOpen);
        }

        /// <summary>
        /// Es llamado cuando el libro ha sido colocado en el atril
        /// </summary>
        private void OnBookOpen(XRBaseInteractable arg0)
        {
            bookOpen = true;
        }

        public override TaskStatus OnUpdate()
        {
            if (bookOpen)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }
    }
}