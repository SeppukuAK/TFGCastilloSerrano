using BehaviorDesigner.Runtime.Tasks;

namespace TFG
{
    [TaskCategory("TFG")]
    public class OnPlayerDropInteractable : OnPlayerInteraction
    {
        protected override void AddListener()
        {
            XRGrabInteractable.Value.onSelectExit.AddListener(OnInteraction);
        }
    }
}