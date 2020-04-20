using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks.Movement;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace TFG
{
    [TaskDescription("Reproduce la animación")]
    [TaskCategory("TFG")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}RotateTowardsIcon.png")]
    public class RotateTowardsTrigger : RotateTowards
    {
        public SharedBool IsGivingTheBack;

        //public override void OnStart()
        //{
        //    IsGivingTheBack.Value = true;
        //}

        ////Establece a false la booleana de dar la espalda
        //public override void OnEnd()
        //{
        //    IsGivingTheBack.Value = false;
        //}
    }
}