using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace TFG
{
    /// <summary>
    /// Manejador del input de los controladores.
    /// Controla el estado y las animaciones de las manos.
    /// </summary>
    public class MyMasterController : MasterController
    {
        public static MyMasterController MyInstance;

        /// <summary>
        /// Tipo para guardar el estado actual de una interacción
        /// </summary>
        internal struct InteractionState
        {
            /// <summary>This field is true if it is is currently on.</summary>
            public bool active;
            /// <summary>This field is true if the interaction state was activated this frame.</summary>
            public bool activatedThisFrame;
            /// <summary>This field is true if the interaction state was de-activated this frame.</summary>
            public bool deActivatedThisFrame;
        }

        [Header("Model")]

        [SerializeField]
        [Tooltip("Gets or sets the animation transition to enable when selecting.")]
        private string modelSelectTransition;

        [SerializeField]
        [Tooltip("Gets or sets the animation transition to enable when de-selecting.")]
        private string modelDeSelectTransition;

        [SerializeField]
        [Tooltip("Gets or sets the animation transition to enable when de-selecting.")]
        private string modelPointingTransition;


        //References

        private XRController leftXRController;
        private XRController rightXRController;


        //Properties

        public HandState LeftHandState { get { return leftHandState; } private set { leftHandState = value; } }
        private HandState leftHandState;

        public HandState RightHandState { get { return rightHandState; } private set { rightHandState = value; } }
        private HandState rightHandState;

        //Atributtes

        //Input de abierta / cerrada de cada mano
        private InteractionState leftHandInteractionState;
        private InteractionState rightHandInteractionState;

        private bool leftHandIsPointing;
        private bool rightHandIsPointing;


        /// <summary>
        /// Obtiene referencias.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            MyInstance = this;
            leftXRController = LeftDirectInteractor.GetComponent<XRController>();
            rightXRController = RightDirectInteractor.GetComponent<XRController>();
        }

        /// <summary>
        /// Inicializa variables
        /// </summary>
        protected override void Start()
        {
            base.Start();
        }

        /// <summary>
        /// Actualiza los controladores
        /// </summary>
        protected override void Update()    //Se hace override para evitar input de teletransporte y animación
        {
            // Actualiza el input
            UpdateInput(ref leftHandInteractionState, leftXRController, out leftHandIsPointing);
            UpdateInput(ref rightHandInteractionState, rightXRController, out rightHandIsPointing);

            // Actualiza la animación de las manos.
            UpdateControllerModelAnimation(ref m_RightHandPrefab, LeftDirectInteractor, leftHandInteractionState, leftHandIsPointing);
            UpdateControllerModelAnimation(ref m_LeftHandPrefab, RightDirectInteractor, rightHandInteractionState, rightHandIsPointing);

            //Actualiza el estado lógico de la manos
            UpdateHandState(ref leftHandState, leftHandInteractionState, leftHandIsPointing);
            UpdateHandState(ref rightHandState, rightHandInteractionState, rightHandIsPointing);
        }

        /// <summary>
        /// Actualiza el input de la mano
        /// </summary>
        private void UpdateInput(ref InteractionState interactionState, XRController XRController, out bool isPointing)
        {
            //Limpia el estado en este frame de seleccionar y señalar
            interactionState.activatedThisFrame = interactionState.deActivatedThisFrame = false;

            //Detecta el input de abrir/cerrar la mano
            HandleInteraction(XRController, ref interactionState);

            //Detecta el input de señalar
            XRController.inputDevice.IsPressed(InputHelpers.Button.Grip, out isPointing, XRController.axisToPressThreshold);
        }

        /// <summary>
        /// Detecta el input de abrir/cerrar la mano
        /// </summary>
        /// <param name="XRController"></param>
        /// <param name="interactionState"></param>
        private void HandleInteraction(XRController XRController, ref InteractionState interactionState)
        {
            bool gripPressed = false;
            XRController.inputDevice.IsPressed(InputHelpers.Button.Grip, out gripPressed, XRController.axisToPressThreshold);

            bool triggerPressed = false;
            XRController.inputDevice.IsPressed(InputHelpers.Button.Trigger, out triggerPressed, XRController.axisToPressThreshold);

            if (gripPressed && triggerPressed)
            {
                if (!interactionState.active)
                {
                    interactionState.activatedThisFrame = true;
                    interactionState.active = true;
                }
            }
            else
            {
                if (interactionState.active)
                {
                    interactionState.deActivatedThisFrame = true;
                    interactionState.active = false;
                }
            }
        }

        /// <summary>
        /// Actualiza el estado de animación de la mano
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="directInteractor"></param>
        /// <param name="gripState"></param>
        /// <param name="pointingState"></param>
        private void UpdateControllerModelAnimation(ref HandPrefab myHandPrefab, XRDirectInteractor directInteractor, InteractionState gripState, bool pointing)
        {
            //Obtiene el animator si no se ha obtenido todavia
            if (!myHandPrefab)
                myHandPrefab = directInteractor.GetComponentInChildren<HandPrefab>();

            if (gripState.activatedThisFrame)
                myHandPrefab.Animator.SetTrigger(modelSelectTransition);

            else if (gripState.deActivatedThisFrame)
                myHandPrefab.Animator.SetTrigger(modelDeSelectTransition);

            myHandPrefab.Animator.SetBool(modelPointingTransition, pointing);
        }

        /// <summary>
        /// Actualiza el estado lógico de la mano
        /// </summary>
        /// <param name="handState"></param>
        /// <param name="interactionState"></param>
        /// <param name="isPointing"></param>
        private void UpdateHandState(ref HandState handState, InteractionState interactionState, bool isPointing)
        {
            if (interactionState.active)
                handState = HandState.CLOSE;

            else if (isPointing)
                handState = HandState.POINTING;

            else
                handState = HandState.OPEN;
        }
    }
}
