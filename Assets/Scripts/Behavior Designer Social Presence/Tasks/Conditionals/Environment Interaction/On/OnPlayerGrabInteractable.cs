using BehaviorDesigner.Runtime.Tasks;

namespace TFG
{
    [TaskCategory("TFG")]
    public class OnPlayerGrabInteractable : OnPlayerInteraction
    {
        protected override void AddListener()
        {
            XRGrabInteractable.Value.onSelectEnter.AddListener(OnInteraction);
        }
    }
}