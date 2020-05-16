using BehaviorDesigner.Runtime.Tasks;
using FrameSynthesis.VR;

namespace SocialPresenceVR
{
    [TaskDescription("Devuelve si el jugador está negando con la cabeza")]
    [TaskCategory("SocialPresenceVR/Gestures/Head")]
    public class CheckHeadShake : Conditional
    {
        HeadGestureState headGestureController;

        public override void OnAwake()
        {
            headGestureController = VRGestureRecognizer.Current.GetComponent<HeadGestureState>();
        }

        public override TaskStatus OnUpdate()
        {
            if (headGestureController.HeadShaking)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }

    }
}