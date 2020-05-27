using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;

namespace SocialPresenceVR
{
    /// <summary>
    /// TODO: Detectar bien el error
    /// </summary>
    [TaskDescription("Reproduce un efecto de sonido durante un tiempo")]
    [TaskCategory("SocialPresenceVR/EscapeRoom/Sound")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}Play.png")]
    public class PlaySoundEffect : Action
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("GameObject que contiene el audio source.")]
        public SharedGameObject targetGameObject;

        private AudioSource audioSource;
        private GameObject prevGameObject;
        private bool error;

        public override void OnStart()
        {
            error = false;

            var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
            if (currentGameObject != prevGameObject)
            {
                audioSource = currentGameObject.GetComponent<AudioSource>();
                prevGameObject = currentGameObject;
            }

            audioSource.Play();
        }

        //Cuando la tarea termina, se para el audio
        public override void OnEnd()
        {
            audioSource.Stop();
        }

        //TODO:MIRAR
        //public override void OnConditionalAbort()
        //{
        //    audioSource.Stop();
        //}


        public override TaskStatus OnUpdate()
        {
            //Caso de error donde no hay AudioSource
            if (error)
                return TaskStatus.Failure;

            //El audio se sigue ejecutando
            else
                return TaskStatus.Running;

        }

        public override void OnReset()
        {
            targetGameObject = null;
        }
    }
}
