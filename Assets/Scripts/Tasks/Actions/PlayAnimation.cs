using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEditor;
using UnityEditor.Animations;

namespace TFG
{
    [TaskDescription("Reproduce la animación")]
    [TaskCategory("TFG")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}SeekIcon.png")]
    public class PlayAnimation : Action
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The GameObject that the agent is seeking")]
        public SharedAnimationClip AnimationClip;

        [BehaviorDesigner.Runtime.Tasks.Tooltip("The GameObject that the agent is seeking")]
        public SharedFloat TransitionDuration;

        [BehaviorDesigner.Runtime.Tasks.Tooltip("The GameObject that the agent is seeking")]
        public SharedFloat AnimDuration;

        private Animator animator;
        private string animName;

        public override void OnAwake()
        {
            Animator animator = GetComponent<Animator>();
            animName = AnimationClip.Value.name + "Trigger";

            //Creación de los parámetros y la transición
            AnimatorController controller = GetComponent<NPC>().AnimatorController;

            controller.AddParameter(animName, AnimatorControllerParameterType.Trigger);

            var rootStateMachine = controller.layers[0].stateMachine;
            var newState = rootStateMachine.AddState(AnimationClip.Value.name);

            newState.motion = AnimationClip.Value;

            var resetTransition = rootStateMachine.AddAnyStateTransition(newState);
            resetTransition.AddCondition(AnimatorConditionMode.If, 0, animName);
            resetTransition.duration = TransitionDuration.Value;

            var defaultTransition = rootStateMachine.AddAnyStateTransition(rootStateMachine.defaultState);
            defaultTransition.hasExitTime = true;
            defaultTransition.duration = TransitionDuration.Value;

            AnimDuration.Value = AnimationClip.Value.length;
        }

        public override void OnStart()
        {
            GetComponent<Animator>().SetTrigger(animName);
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }
    }
}
