using System.Collections;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEditor.Animations;
using System.Collections.Generic;

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

                animDuration = AnimationClip.Value.length;
            }
        }

        /// <summary>
        /// Reproduce la animación.
        /// Si la transición no está creada, la crea
        /// </summary>
        public override void OnStart()
        {
            ended = false;
            animator.SetTrigger(triggerName); //Play
            StartCoroutine(WaitForAnimation());
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
