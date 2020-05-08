using BehaviorDesigner.Runtime.Tasks;
using FrameSynthesis.VR;

namespace TFG
{
    [TaskDescription("Devuelve si el jugador está asintiendo con la cabeza")]
    [TaskCategory("TFG")]
    public class CheckNod : Conditional
    {
        private bool nodding = false;

        public override void OnStart()
        {
            base.OnStart();
        }

        public override TaskStatus OnUpdate()
        {
            if (HeadGestureManager.Instance.Nodding)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }

    }
}