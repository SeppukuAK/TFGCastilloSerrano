using BehaviorDesigner.Runtime.Tasks;

namespace SocialPresenceVR
{
    [TaskDescription("Devuelve si el jugador está asintiendo con la cabeza")]
    [TaskCategory("SocialPresenceVR/Gestures/Head")]
    public class CheckNod : Conditional
    {
        public override TaskStatus OnUpdate()
        {
            if (HeadGestureManager.Instance.Nodding)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }

    }
}