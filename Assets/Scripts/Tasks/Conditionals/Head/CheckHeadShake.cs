using BehaviorDesigner.Runtime.Tasks;

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
