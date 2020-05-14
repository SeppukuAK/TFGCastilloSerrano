using BehaviorDesigner.Runtime.Tasks;

namespace SocialPresenceVR
{
    [TaskDescription("Devuelve si el jugador está negando con la cabeza")]
    [TaskCategory("SocialPresenceVR/Gestures/Head")]
    public class CheckHeadShake : Conditional
    {
        public override TaskStatus OnUpdate()
        {
            if (HeadGestureManager.Instance.HeadShaking)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }

    }
}