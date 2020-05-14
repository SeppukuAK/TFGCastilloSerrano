using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using BehaviorDesigner.Runtime;

namespace SocialPresenceVR
{
    /// <summary>
    /// Configura la escena para Debug. 
    /// Si está el casco conectado, se borra el simulator y se ejecuta como el juego en realidad
    /// Si no está conectado, se borra el XRRig y se configura la escena para que funcione el simulador
    /// </summary>
    public class DebugManager : MonoBehaviour
    {
        public XRRig XRRig;
        public PlayerMovement PlayerSimulatorPrefab;
        public SP_NPC NPC;

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
            //Camara del simulador
            SharedGameObject head = playerSimulator.GetComponentInChildren<Camera>().gameObject;
            NPC.GetComponent<BehaviorTree>().SetVariable("Head", head);

            //Se obtiene la altura inicial de la cabeza
            NPC.PlayerHeight = head.Value.transform.position.y;
            Destroy(this.gameObject);
        }


    }
}
