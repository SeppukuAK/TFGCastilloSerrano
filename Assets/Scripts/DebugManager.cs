using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using BehaviorDesigner.Runtime;

namespace TFG
{
    public class DebugManager : MonoBehaviour
    {
        public XRRig XRRig;
        public PlayerMovement PlayerSimulatorPrefab;
        public NPC NPC;

        private Vector3 XRRigPosition;
        private Quaternion XRRigRotation;
        private PlayerMovement playerSimulator;

        private void Awake()
        {
            if (XRDevice.isPresent)
            {
                Debug.Log("Dispositivo XR presente");
                Destroy(this.gameObject);
            }
            else
            {
                Debug.Log("No hay ningún dispositivo XR presente");
                XRRigPosition = XRRig.transform.position;
                XRRigRotation = XRRig.transform.rotation;
                Destroy(XRRig.gameObject);

                playerSimulator = Instantiate(PlayerSimulatorPrefab);
                playerSimulator.transform.position = new Vector3(XRRigPosition.x, 1.0f, XRRigPosition.z);
                playerSimulator.transform.rotation = XRRigRotation;
            }

        }

        private void Start()
        {
            SharedGameObject head = playerSimulator.GetComponentInChildren<Camera>().gameObject;
            NPC.GetComponent<BehaviorTree>().SetVariable("Head", head);
            Destroy(this.gameObject);
        }


    }
}
