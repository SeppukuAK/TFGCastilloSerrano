using BehaviorDesigner.Runtime.Tasks;

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
