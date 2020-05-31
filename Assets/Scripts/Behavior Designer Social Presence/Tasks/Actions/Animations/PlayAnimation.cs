using System.Collections;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

namespace SocialPresenceVR
{
    [TaskDescription("Reproduce la animación")]
    [TaskCategory("SocialPresenceVR/Animations")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}Play.png")]
    public class PlayAnimation : Action
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("Animación que se quiere reproducir")]
        public SharedAnimationClip AnimationClip;

        [BehaviorDesigner.Runtime.Tasks.Tooltip("Duración de la transición entre la animación anterior y la actual")]
        public SharedFloat TransitionDuration;

        /// <summary>
        /// Duración total de la animación
        /// </summary>
        private float animDuration;

        /// <summary>
        /// Referencia al animator del NPC
        /// </summary>
        private Animator animator;

        /// <summary>
        /// Nombre del trigger de esta transición
        /// </summary>
        private string triggerName;

        /// <summary>
        /// Booleana que indica si la animación ha acabado
        /// </summary>
        bool ended;

        /// <summary>
        /// Lista con la lista de animaciones creadas
        /// </summary>
        private static List<string> transtionsCreated = new List<string>();

        /// <summary>
        /// Referencia a la rutina en ejecución de espera para la animación
        /// </summary>
        private IEnumerator animationRoutine;

        /// <summary>
        /// Crea la transición
        /// </summary>
        public override void OnAwake()
        {
            animationRoutine = null;
            animator = GetComponent<Animator>();
            triggerName = AnimationClip.Value.name + "Trigger";
            animDuration = AnimationClip.Value.length;
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
        /// Si la transición no está creada, la crea
        /// </summary>
        public override void OnStart()
        {
            ended = false;
            animator.SetTrigger(triggerName); //Play

            animationRoutine = WaitForAnimation();
            StartCoroutine(animationRoutine);
        }


        public override void OnEnd()
        {
            animator.ResetTrigger(triggerName);
            StopCoroutine(animationRoutine);
            animationRoutine = null;
        }

        /// <summary>
        /// Comprueba si la animación ha acabado de reproducirse.
        /// Devuelve success cuando ha acabado y running si está en proceso
        /// </summary>
        /// <returns></returns>
        public override TaskStatus OnUpdate()
        {
            if (ended)
                return TaskStatus.Success;
            else
                return TaskStatus.Running;
        }

        /// <summary>
        /// Corrutina de espera hasta el fin de la animación
        /// </summary>
        /// <returns></returns>
        IEnumerator WaitForAnimation()
        {
            yield return new WaitForSeconds(animDuration);
            ended = true;
        }
    }
}
