using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace SocialPresenceVR
{
    [TaskDescription("Devuelve si el NPC ha sido golpeado por un objeto interactuable")]
    [TaskCategory("SocialPresenceVR/NPCInteraction")]
    public class HasBeenHitByInteractable : Conditional
    {
        private bool enteredCollision = false;

        public override TaskStatus OnUpdate()
        {
            return enteredCollision ? TaskStatus.Success : TaskStatus.Failure;
        }

        public override void OnEnd()
        {
            enteredCollision = false;
        }

        public override void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.GetComponent<XRBaseInteractable>())
                enteredCollision = true;
        }


    }
}
