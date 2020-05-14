using BehaviorDesigner.Runtime.Tasks.Movement;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using UnityEngine;

namespace SocialPresenceVR
{
    [TaskDescription("Versión arreglada del CanSeeObject")]
    [TaskCategory("SocialPresenceVR")]
    public class MyCanSeeObject : Conditional
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The object that we are searching for")]
        public SharedGameObject targetObject;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The field of view angle of the agent (in degrees)")]
        public SharedFloat fieldOfViewAngle = 90;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The distance that the agent can see")]
        public SharedFloat viewDistance = 1000;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The raycast offset relative to the pivot position")]
        public SharedVector3 offset;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The target raycast offset relative to the pivot position")]
        public SharedVector3 targetOffset;


        public override TaskStatus OnUpdate()
        {
            bool canSee = false;

            // If the target is not null then determine if that object is within sight
            if (targetObject.Value != null)
                canSee = WithinSight(transform, offset.Value, fieldOfViewAngle.Value, viewDistance.Value, targetObject.Value, targetOffset.Value);

            if (canSee)
                return TaskStatus.Success;

            // An object is not within sight so return failure
            return TaskStatus.Failure;
        }

        // Reset the public variables
        public override void OnReset()
        {
            fieldOfViewAngle = 90;
            viewDistance = 1000;
            offset = Vector3.zero;
            targetOffset = Vector3.zero;
        }

        // Draw the line of sight representation within the scene window
        public override void OnDrawGizmos()
        {
            MovementUtility.DrawLineOfSight(Owner.transform, offset.Value, fieldOfViewAngle.Value, 0, viewDistance.Value, false);
        }

        public override void OnBehaviorComplete()
        {
            MovementUtility.ClearCache();
        }

        // Public helper function that will automatically create an angle variable that is not used. This function is useful if the calling object doesn't
        // care about the angle between transform and targetObject
        public static bool WithinSight(Transform transform, Vector3 positionOffset, float fieldOfViewAngle, float viewDistance, GameObject targetObject, Vector3 targetOffset)
        {
            float angle;
            return WithinSight(transform, positionOffset, fieldOfViewAngle, viewDistance, targetObject, targetOffset, out angle);
        }

        // Determines if the targetObject is within sight of the transform. It will set the angle regardless of whether or not the object is within sight
        public static bool WithinSight(Transform transform, Vector3 positionOffset, float fieldOfViewAngle, float viewDistance, GameObject targetObject, Vector3 targetOffset, out float angle)
        {
            if (targetObject == null)
            {
                angle = 0;
                return false;
            }

            // The target object needs to be within the field of view of the current object
            Vector3 targetPos = targetObject.transform.TransformPoint(targetOffset);
            Vector3 pos = transform.TransformPoint(positionOffset);

            targetPos.y = pos.y = 0;

            var direction = targetPos - pos;

            angle = Vector3.Angle(direction, transform.forward);
            direction.y = 0;

            if (direction.magnitude < viewDistance && angle < fieldOfViewAngle * 0.5f)
                return true; // return the target object meaning it is within sight

            // return null if the target object is not within sight
            return false;
        }
    }
}
