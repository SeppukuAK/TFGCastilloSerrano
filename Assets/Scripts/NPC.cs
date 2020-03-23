using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using BehaviorDesigner.Runtime;
using UnityEditor;
using UnityEngine.Animations;
using UnityEditor.Animations;

namespace TFG
{
    [RequireComponent(typeof(Animator))]
    public class NPC : MonoBehaviour
    {
        public VRTK_SDKManager SDKManager;
        public AnimationClip DefaultAnimation;

        public AnimatorController AnimatorController { get; set; }

        private Transform headTransform;
        private BehaviorTree behaviorTree;

        public float StandardPlayerHeight { get; set; }



        private void Awake()
        {
            behaviorTree = GetComponent<BehaviorTree>();//Obtenemos el árbol de comportamiento

            Animator animator = GetComponent<Animator>();

            //https://docs.unity3d.com/ScriptReference/Animations.AnimatorController.html?_ga=2.239912643.1726044905.1583340668-154422748.1555576536
            if (animator.runtimeAnimatorController != null)
            {
                Debug.LogError("No debe haber un controlador asociado al animator");
                animator.runtimeAnimatorController = null;
            }

            // Creates the controller
            AnimatorController = AnimatorController.CreateAnimatorControllerAtPath("Assets/Adri.controller");
            animator.runtimeAnimatorController = AnimatorController;

            string animName = DefaultAnimation.name;

            //Creación de los parámetros y la transición
            var rootStateMachine = AnimatorController.layers[0].stateMachine;
            var newState = rootStateMachine.AddState("Default Animation");
            newState.motion = DefaultAnimation;

            VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);


        }

        // Start is called before the first frame update
        void Start()
        {
            headTransform = VRTK_DeviceFinder.DeviceTransform(VRTK_DeviceFinder.Devices.Headset);//Obtenemos el headset activo

            SharedGameObject headSharedGameObject = headTransform.gameObject;
            behaviorTree.SetVariable("Head", headSharedGameObject);
            behaviorTree.EnableBehavior();//Se activa el arbol de comportamiento

            //Se obtiene la altura inicial de la cabeza
            StandardPlayerHeight = headTransform.position.y;
            Debug.Log(StandardPlayerHeight);
        }

        private void OnDestroy()
        {
            VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
            AssetDatabase.DeleteAsset("Assets/Adri.controller");
        }

    }
}
