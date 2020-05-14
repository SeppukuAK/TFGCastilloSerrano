using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

namespace SocialPresenceVR
{
    /// <summary>
    /// Componente dedicado a controlar el estado de la mano
    /// </summary>
    [RequireComponent(typeof(XRController))]
    public class ControllerState : MonoBehaviour
    {
        /// <summary>
        /// Estado actual de la mano
        /// </summary>
        public HandState HandState { get; private set; }

        private XRController XRController;

        /// <summary>
        /// Obtiene referencias
        /// </summary>
        private void Awake()
        {
            XRController = GetComponent<XRController>();
        }

        /// <summary>
        /// Controla el estado actual de la mano
        /// </summary>
        private void Update()
        {
            //Input Grip
            bool pressed;
            XRController.inputDevice.IsPressed(XRController.selectUsage, out pressed, XRController.axisToPressThreshold);

            //Input axis
            Vector2 axisInput;
            XRController.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out axisInput);

            //Mano cerrada
            if (pressed)
                HandState = HandState.CLOSE;

            //Mano señalando
            else if (axisInput.y > 0.5f)
                HandState = HandState.POINTING;

            //Mano abierta
            else
                HandState = HandState.OPEN;

        }
    }
}
