using BehaviorDesigner.Runtime.Tasks;

namespace SocialPresenceVR
{
    [TaskDescription("Devuelve si el NPC tiene algún objeto agarrado")]
    [TaskCategory("SocialPresenceVR/NPCInteraction")]
    public class IsGrabbingObject : Conditional
    {
        private SP_NPC NPC;

        /// <summary>
        /// Obtiene referencias
        /// </summary>
        public override void OnAwake()
        {
            NPC = GetComponent<SP_NPC>();
        }
        public override TaskStatus OnUpdate()
        {
            if (NPC.GrabbedInteractable.Interactable != null)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }
    }
}
