using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace SocialPresenceVR
{
    /// <summary>
    /// TODO: No está hecho de la mejor manera, pero funciona
    /// </summary>
    [TaskDescription("Devuelve si la radio está encendida")]
    [TaskCategory("SocialPresenceVR/EscapeRoom")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}Play.png")]
    public class IsRadioOn : Conditional
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("Volumen en el que se considera que la radio está encendida [ 0 , 1 ]")]
        public SharedFloat Volume;

        private AudioSource radioSource;

        /// <summary>
        /// Obtiene referencia a la radio
        /// </summary>
        public override void OnAwake()
        {
            Radio radio = GameObject.FindObjectOfType<Radio>();
            if (!radio)
                Debug.LogError("Radio no encontrada en la escena");

            radioSource = radio.MusicSource;
        }

        public override TaskStatus OnUpdate()
        {
            if (radioSource.isPlaying && radioSource.volume > Volume.Value)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }
    }
}