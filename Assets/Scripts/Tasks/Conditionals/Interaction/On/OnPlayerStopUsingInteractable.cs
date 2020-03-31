using BehaviorDesigner.Runtime.Tasks;

namespace TFG
{
    [TaskCategory("TFG")]
    public class OnPlayerStopUsingInteractable : OnPlayerInteraction
    {
        protected override void AddListener()
        {
            XRGrabInteractable.Value.onDeactivate.AddListener(OnInteraction);
        }
    }
}