using BehaviorDesigner.Runtime.Tasks;

namespace TFG
{
    [TaskCategory("TFG")]
    public class OnPlayerUseInteractable : OnPlayerInteraction
    {
        protected override void AddListener()
        {
            XRGrabInteractable.Value.onActivate.AddListener(OnInteraction);
        }
    }
}