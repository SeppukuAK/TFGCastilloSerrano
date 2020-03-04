using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using BehaviorDesigner.Runtime;

namespace TFG
{
    public class NPC : MonoBehaviour
    {
        public VRTK_SDKManager SDKManager;

        private Transform headTransform;
        private BehaviorTree behaviorTree;


        private void Awake()
        {
            behaviorTree = GetComponent<BehaviorTree>();//Obtenemos el árbol de comportamiento

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
