using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace SocialPresenceVR
{
    /// <summary>
    /// TODO: No está hecho de la mejor manera, pero funciona
    /// </summary>
    [TaskDescription("Devuelve si la puerta del Escape Room ha sido abierta")]
    [TaskCategory("SocialPresenceVR/EscapeRoom")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}Play.png")]
    public class IsDoorOpen : Conditional
    {
        private PotionReceiver potionReceiver;

        private bool doorOpen;

        /// <summary>
        /// Obtiene referencia a puerta y se suscribe a cuando sea abierta
        /// </summary>
        public override void OnAwake()
        {
            doorOpen = false;

            GameObject potionReceiverObject = GameObject.Find("PotionReceiver");
            if (!potionReceiverObject)
                Debug.LogError("PotionReceiver no encontrado en la escena");

            potionReceiver = potionReceiverObject.GetComponent<PotionReceiver>();

            if (!potionReceiver)
                Debug.LogError("PotionReceiver no tiene el componente PotionReceiver");

            potionReceiver.OnPotionPoured.AddListener(OnDoorOpen);
        }

        /// <summary>
        /// Es llamado cuando la puerta ha sido abierta
        /// </summary>
        private void OnDoorOpen(string potionType)
        {
            doorOpen = true;
        }

        public override TaskStatus OnUpdate()
        {
            if (doorOpen)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }
    }
}