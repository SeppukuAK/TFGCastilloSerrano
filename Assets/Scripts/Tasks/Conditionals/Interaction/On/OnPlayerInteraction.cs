using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.XR.Interaction.Toolkit;

namespace TFG
{
    public abstract class OnPlayerInteraction : Conditional
    {
        public SharedXRGrabInteractable XRGrabInteractable;

        private bool interaction;

        protected abstract void AddListener();

        public override void OnAwake()
        {
            interaction = false;
            AddListener();
        }

        protected void OnInteraction(XRBaseInteractor arg0)
        {
            interaction = true;
        }

        public override void OnEnd()
        {
            interaction = false;
        }

        public override TaskStatus OnUpdate()
        {
            if (interaction)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }

    }
}