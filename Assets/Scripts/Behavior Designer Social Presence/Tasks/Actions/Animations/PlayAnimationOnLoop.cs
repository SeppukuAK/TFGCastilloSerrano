using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEditor.Animations;
using System.Collections.Generic;

namespace SocialPresenceVR
{
    /// <summary>
    /// TODO: Puede estar más comentado el método de CreateAnimatorTransition(), tambien de no Loop
    /// TODO: Clase padre de ambos PlayAnimation
    /// </summary>
    [TaskDescription("Reproduce la animación en bucle")]
    [TaskCategory("SocialPresenceVR/Animations")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}Play.png")]
    public class PlayAnimationOnLoop : Action
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The animation we want to play")]
        public SharedAnimationClip AnimationClip;

        [BehaviorDesigner.Runtime.Tasks.Tooltip("The duration of the transition between the previous animation and this animation")]
        public SharedFloat TransitionDuration;

        /// <summary>
        /// Referencia al animator del NPC
        /// </summary>
        private Animator animator;

        /// <summary>
        /// Nombre del trigger de esta transición
        /// </summary>
        private string triggerName;

        /// <summary>
        /// Lista con la lista de animaciones creadas
        /// </summary>
        private static List<string> transtionsCreated = new List<string>();

        /// <summary>
        /// Crea la transición
        /// </summary>
        public override void OnAwake()
        {
            animator = GetComponent<Animator>();
            CreateAnimatorTransition();
        }
 
        /// <summary>
        /// Crea el estado en la máquina de estados y su transición
        /// </summary>
        private void CreateAnimatorTransition()
        {
            if (!AnimationClip.Value.isLooping)
                Debug.LogError("Animación asociada a un playAnimationOnLoop sin LoopTime marcado: " + AnimationClip.Value.name);

            triggerName = AnimationClip.Value.name + "Trigger";

            if (!transtionsCreated.Contains(triggerName))
            {
                transtionsCreated.Add(triggerName);

                //Creación de los parámetros y la transición
                AnimatorController controller = GetComponent<SP_NPC>().AnimatorController;

                controller.AddParameter(triggerName, AnimatorControllerParameterType.Trigger);

                //Maquina de estados
                var rootStateMachine = controller.layers[0].stateMachine;

                var newState = rootStateMachine.AddState(AnimationClip.Value.name);

                newState.motion = AnimationClip.Value;

                var resetTransition = rootStateMachine.AddAnyStateTransition(newState);
                resetTransition.AddCondition(AnimatorConditionMode.If, 0, triggerName);
                resetTransition.duration = TransitionDuration.Value;

            }
        }

        /// <summary>
        /// Reproduce la animación.
        /// </summary>
        public override void OnStart()
        {
            animator.SetTrigger(triggerName);   //play
        }

        /// <summary>
        /// Devuelve running continuamente. Esta tarea tiene que ser interrumpida para acabar
        /// </summary>
        /// <returns></returns>
        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Running;
        }
    }
}
