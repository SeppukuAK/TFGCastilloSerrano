using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif
using System.IO;
using System;

namespace SocialPresenceVR
{
    /// <summary>
    /// TODO: No se si es necesario un default animation
    /// TODO: NO CREO QUE SE COJA BIEN LA ALTURA del jugador
    /// TODO: Parametrizar mucho mejor, todo lo que tenga que ver con animaciones
    /// </summary>
    [TaskDescription("Inicializa NPC con Presencia Social")]
    [TaskCategory("SocialPresenceVR/Init")]
    public class Init_SP_NPC : BehaviorDesigner.Runtime.Tasks.Action
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("Altura del jugador en metros")]
        public SharedFloat PlayerHeight;

        [BehaviorDesigner.Runtime.Tasks.Tooltip("Mano principal del NPC con la que agarra objetos")]
        public SharedGameObject Hand;

        public SharedBool ResetAnimator = true;

        private SP_NPC _SP_NPC;

        public float timePlayed;

        public override void OnAwake()
        {
            _SP_NPC = gameObject.AddComponent<SP_NPC>();

            _SP_NPC.PlayerHeight = PlayerHeight.Value;
            _SP_NPC.Hand = Hand.Value;

            Animator animator = GetComponent<Animator>();

#if UNITY_EDITOR
            _SP_NPC.ResetAnimator = ResetAnimator.Value;

            if (ResetAnimator.Value)
            {
                if (animator)
                    Debug.LogError("No debe haber un Animator asociado al NPC");

                else
                {
                    //https://docs.unity3d.com/ScriptReference/Animations.AnimatorController.html?_ga=2.239912643.1726044905.1583340668-154422748.1555576536
                    animator = gameObject.AddComponent<Animator>();

                    string animatorControllerPath = "Assets/SP_NPC_Controller.controller";

                    //Destruye el asset del animator controller
                    AssetDatabase.DeleteAsset(animatorControllerPath);

                    // Creates the controller
                    AnimatorController animatorController = AnimatorController.CreateAnimatorControllerAtPath(animatorControllerPath);
                    animator.runtimeAnimatorController = animatorController;

                    //Creación del estado inicial
                    var rootStateMachine = animatorController.layers[0].stateMachine;
                    var newState = rootStateMachine.AddState("Entry State");

                    _SP_NPC.AnimatorController = animatorController;
                }
            }
            else
            {
                if (!animator)
                    Debug.LogError("Debe haber un Animator asociado al NPC");

            }

#endif


        }

        public override TaskStatus OnUpdate()
        {
            if (_SP_NPC)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }
    
        public override void OnBehaviorComplete()
        {
            CreateText();
            Debug.Log("FIN");
        }

        void CreateText()
        {
            //Path of the file
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TiempoJugado.txt");

            float minutesPlayed = Time.time / 60;
            float secondsPlayed = Time.time % 60;
            //Content of the file
            string content = "\n" + "Tiempo jugado: " + (int)minutesPlayed + " minutos" + " y "+ (int)secondsPlayed + " segundos." + "\n";

            //Add some to text to it
            File.AppendAllText(path, content);
        }
    }

}