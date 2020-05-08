using BehaviorDesigner.Runtime.Tasks;
using FrameSynthesis.VR;

namespace TFG
{
    [TaskDescription("Devuelve si el jugador está negando con la cabeza")]
    [TaskCategory("TFG")]
    public class CheckHeadShake : Conditional
    {
        public override void OnStart()
        {
            base.OnStart();
        }

        public override TaskStatus OnUpdate()
        {
            if (HeadGestureManager.Instance.HeadShaking)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }

    }
}