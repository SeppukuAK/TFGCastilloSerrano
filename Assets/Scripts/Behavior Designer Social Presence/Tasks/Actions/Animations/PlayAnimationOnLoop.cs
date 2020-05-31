using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

namespace SocialPresenceVR
{
    /// <summary>
    /// TODO: Puede estar más comentado el método de CreateAnimatorTransition(), tambien de no Loop
    /// TODO: Clase padre de ambos PlayAnimation
    /// TODO: Update un poco cutre y puede haber probemas con playAnimation normal
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

            if (!AnimationClip.Value.isLooping)
                Debug.LogError("Animación asociada a un playAnimationOnLoop sin LoopTime marcado: " + AnimationClip.Value.name);

            triggerName = AnimationClip.Value.name + "Trigger";

#if UNITY_EDITOR
            if (GetComponent<SP_NPC>().ResetAnimator && !transtionsCreated.Contains(triggerName))
                CreateAnimatorTransition();
#endif
        }

#if UNITY_EDITOR
        /// <summary>
        /// Crea el estado en la máquina de estados y su transición
        /// </summary>
        private void CreateAnimatorTransition()
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
#endif

        /// <summary>
        /// Reproduce la animación.
        /// </summary>
        public override void OnStart()
        {
            animator.SetTrigger(triggerName);   //play
        }

        public override void OnEnd()
        {
            animator.ResetTrigger(triggerName);
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
