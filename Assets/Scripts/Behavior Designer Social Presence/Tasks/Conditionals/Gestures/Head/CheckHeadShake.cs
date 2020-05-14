using BehaviorDesigner.Runtime.Tasks;

namespace TFG
{
    [TaskDescription("Devuelve si el jugador está negando con la cabeza")]
    [TaskCategory("TFG")]
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