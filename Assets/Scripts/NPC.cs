using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using BehaviorDesigner.Runtime;

namespace TFG
{
    [RequireComponent(typeof(Animator))]
    public class NPC : MonoBehaviour
    {
        public VRTK_SDKManager SDKManager;

        private Transform headTransform;
        private BehaviorTree behaviorTree;

    
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
            var controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath("Assets/Adri.controller");
            DestroyImmediate(controller,true);
            //animator.runtimeAnimatorController = controller;

            VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
        }

        // Start is called before the first frame update
        void Start()
        {
            headTransform = VRTK_DeviceFinder.DeviceTransform(VRTK_DeviceFinder.Devices.Headset);//Obtenemos el headset activo

            SharedGameObject headSharedGameObject = headTransform.gameObject;
            behaviorTree.SetVariable("Head", headSharedGameObject);
            behaviorTree.EnableBehavior();//Se activa el arbol de comportamiento


        }

        private void OnDestroy()
        {
            VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
        }
        // Update is called once per frame
        void Update()
        {
        }
    }
}
