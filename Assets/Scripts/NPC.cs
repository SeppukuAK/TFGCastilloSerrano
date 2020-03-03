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
            behaviorTree = GetComponent<BehaviorTree>();
            VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
        }

        // Start is called before the first frame update
        void Start()
        {
            headTransform = VRTK_DeviceFinder.DeviceTransform(VRTK_DeviceFinder.Devices.Headset);

            SharedGameObject headSharedGameObject = headTransform.gameObject;
            behaviorTree.SetVariable("Head", headSharedGameObject);
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}
