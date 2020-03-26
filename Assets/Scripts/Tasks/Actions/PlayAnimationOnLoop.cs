using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEditor;
using UnityEditor.Animations;

namespace TFG
{
    [TaskDescription("Reproduce la animación en bucle")]
    [TaskCategory("TFG")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}Play.png")]
    public class PlayAnimationOnLoop : Action
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The animation we want to play")]
        public SharedAnimationClip AnimationClip;

        [BehaviorDesigner.Runtime.Tasks.Tooltip("The duration of the transition between the previous animation and this animation")]
        public SharedFloat TransitionDuration;

        private Animator animator;
        private string triggerName;//Name of the trigger

        public override void OnAwake()
        {
            animator = GetComponent<Animator>();
            triggerName = AnimationClip.Value.name + "Trigger";

            //Creación de los parámetros y la transición
            AnimatorController controller = GetComponent<NPC>().AnimatorController;
           
            controller.AddParameter(triggerName, AnimatorControllerParameterType.Trigger);

            //maquina de estados
            var rootStateMachine = controller.layers[0].stateMachine;
         
            var newState = rootStateMachine.AddState(AnimationClip.Value.name);

            newState.motion = AnimationClip.Value;

            var resetTransition = rootStateMachine.AddAnyStateTransition(newState);
            resetTransition.AddCondition(AnimatorConditionMode.If, 0, triggerName);
            resetTransition.duration = TransitionDuration.Value;
        }

        public override void OnStart()
        {
            GetComponent<Animator>().SetTrigger(triggerName);//play
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Running;
        }
    }
}
