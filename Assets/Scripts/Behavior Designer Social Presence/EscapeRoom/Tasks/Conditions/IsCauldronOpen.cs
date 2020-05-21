using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace SocialPresenceVR
{
    /// <summary>
    /// TODO: No está hecho de la mejor manera, pero funciona
    /// </summary>
    [TaskDescription("Devuelve si se ha abierto el caldero")]
    [TaskCategory("SocialPresenceVR/EscapeRoom")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}Play.png")]
    public class IsCauldronOpen : Conditional
    {
        private MagicReceiver cauldron;    //Caldero
        private bool cauldronOpen;

        /// <summary>
        /// Obtiene referencia al caldero y se suscribe a cuando sea abierto
        /// </summary>
        public override void OnAwake()
        {
            cauldron = null;
            cauldronOpen = false;

            GameObject cauldronObject = GameObject.Find("Cauldron");

            if (!cauldronObject)
                Debug.LogError("Caldero no encontrado en la escena");

            else
            {
                cauldron = cauldronObject.GetComponentInChildren<MagicReceiver>();
                if (!cauldron)
                    Debug.LogError("Componente MagicReceiver en caldero no encontrado");

                else
                    cauldron.OnMagicCollision.AddListener(OnChauldronOpen);

            }
        }

        /// <summary>
        /// Es llamado cuando el caldero ha sido abierto por la magia
        /// </summary>
        private void OnChauldronOpen()
        {
            cauldronOpen = true;
        }

        public override TaskStatus OnUpdate()
        {
            if (cauldronOpen)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }
    }
}