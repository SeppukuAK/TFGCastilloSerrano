﻿using System.Collections;
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
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}Play.png")]
    public class PlayAnimation : Action
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The animation we want to play")]
        public SharedAnimationClip AnimationClip;

        [BehaviorDesigner.Runtime.Tasks.Tooltip("The duration of the transition between the previous animation and this animation")]
        public SharedFloat TransitionDuration;

        [BehaviorDesigner.Runtime.Tasks.Tooltip("The duration of the animation")]
        public SharedFloat AnimDuration;

        private Animator animator;
        private string triggerName;//Name of the trigger

        bool ended;

        public override void OnAwake()
        {
            animator = GetComponent<Animator>();
            triggerName = AnimationClip.Value.name + "Trigger";

            //Creación de los parámetros y la transición
            AnimatorController controller = GetComponent<NPC>().AnimatorController;

            controller.AddParameter(triggerName, AnimatorControllerParameterType.Trigger);

            var rootStateMachine = controller.layers[0].stateMachine;
            var newState = rootStateMachine.AddState(AnimationClip.Value.name);

            newState.motion = AnimationClip.Value;

            //transicion de cualquier estado a cualquier estado si se activa el trigger
            var resetTransition = rootStateMachine.AddAnyStateTransition(newState);
            resetTransition.AddCondition(AnimatorConditionMode.If, 0, triggerName);
            resetTransition.duration = TransitionDuration.Value;

            AnimDuration.Value = AnimationClip.Value.length;
        }

        public override void OnStart()
        {
            ended = false;
            GetComponent<Animator>().SetTrigger(triggerName);
            StartCoroutine(WaitForAnimation());
        }

        public override TaskStatus OnUpdate()
        {

            //Se comprueba si la animación ha terminado de reproducirse
            if (ended)
                return TaskStatus.Success;
            
            else
                return TaskStatus.Running;
        }

        //Corrutina de espera hasta el fin de la animación
        IEnumerator WaitForAnimation()
        {
            yield return new WaitForSeconds(AnimDuration.Value);
            ended = true;
        }


    }
}
