using BehaviorDesigner.Runtime.Tasks;

namespace TFG
{
    [TaskDescription("Devuelve si el jugador está asintiendo con la cabeza")]
    [TaskCategory("TFG")]
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