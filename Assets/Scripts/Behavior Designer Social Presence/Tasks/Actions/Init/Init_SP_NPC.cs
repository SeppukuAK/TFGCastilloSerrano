using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEditor.Animations;

namespace TFG
{
    /// <summary>
    /// TODO: No se si es necesario un default animation
    /// TODO: NO CREO QUE SE COJA BIEN LA ALTURA del jugador
    /// TODO: Parametrizar mucho mejor, todo lo que tenga que ver con animaciones
    /// </summary>
    [TaskDescription("Inicializa NPC con Presencia Social")]
    [TaskCategory("TFG")]
    public class Init_SP_NPC : Action
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("Mano principal del NPC con la que agarra objetos")]
        public SharedGameObject Hand;

        private SP_NPC _SP_NPC;

        public override void OnAwake()
        {
            _SP_NPC = gameObject.AddComponent<SP_NPC>();

            Animator animator = GetComponent<Animator>();

            if (animator)
                Debug.LogError("No debe haber un Animator asociado al NPC");

            else
            {
                //https://docs.unity3d.com/ScriptReference/Animations.AnimatorController.html?_ga=2.239912643.1726044905.1583340668-154422748.1555576536
                animator = gameObject.AddComponent<Animator>();

                string animatorControllerPath = "Assets/SP_NPC_Controller.controller";

                // Creates the controller
                AnimatorController animatorController = AnimatorController.CreateAnimatorControllerAtPath(animatorControllerPath);
                animator.runtimeAnimatorController = animatorController;

                //Creación del estado inicial
                var rootStateMachine = animatorController.layers[0].stateMachine;
                var newState = rootStateMachine.AddState("Entry State");

                _SP_NPC.AnimatorController = animatorController;
                _SP_NPC.PlayerHeight = Camera.main.transform.position.y;
                _SP_NPC.Hand = Hand.Value;
                _SP_NPC.AnimatorControllerPath = animatorControllerPath;
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (_SP_NPC)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }

    }
}