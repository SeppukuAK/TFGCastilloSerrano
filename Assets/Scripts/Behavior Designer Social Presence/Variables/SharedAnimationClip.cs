using UnityEngine;
using BehaviorDesigner.Runtime;

namespace SocialPresenceVR
{
    [System.Serializable]
    public class SharedAnimationClip : SharedVariable<AnimationClip>
    {
        public static implicit operator SharedAnimationClip(AnimationClip value) { return new SharedAnimationClip { Value = value }; }
    }
}