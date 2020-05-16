using BehaviorDesigner.Runtime.Tasks;
using FrameSynthesis.VR;

namespace SocialPresenceVR
{
    [TaskDescription("Devuelve si el jugador está asintiendo con la cabeza")]
    [TaskCategory("SocialPresenceVR/Gestures/Head")]
    public class CheckNod : Conditional
    {
        HeadGestureState headGestureController;

        public override void OnAwake()
        {
            headGestureController = VRGestureRecognizer.Current.GetComponent<HeadGestureState>();
        }

        public override TaskStatus OnUpdate()
        {
            if (headGestureController.Nodding)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }

    }
}