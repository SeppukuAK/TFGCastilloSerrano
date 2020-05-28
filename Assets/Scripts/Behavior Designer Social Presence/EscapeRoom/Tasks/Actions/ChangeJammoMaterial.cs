using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

namespace SocialPresenceVR
{
    /// <summary>
    /// TODO: Si hay 2 tareas iguales puede no cambiarse bien el color, porque no comparten indice ni albedoList
    /// TODO: Detección de errores
    /// </summary>
    [TaskDescription("Cambia el color del material del NPC")]
    [TaskCategory("SocialPresenceVR/EscapeRoom")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}Play.png")]
    public class ChangeJammoMaterial : Action
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("Materiales que se establecen al NPC")]
        public SharedMaterial[] Colors;

        private Renderer[] characterMaterials;
        private int index;

        public override void OnAwake()
        {
            index = 0;
            characterMaterials = gameObject.GetComponentsInChildren<Renderer>();

            if (characterMaterials == null)
                Debug.LogError("Renderers Component no asociado al NPC");
        }

        public override void OnStart()
        {
            for (int i = 0; i < characterMaterials.Length; i++)
                if (!characterMaterials[i].transform.CompareTag("PlayerEyes"))
                    characterMaterials[i].material = Colors[index].Value;

            index = (index + 1) % Colors.Length;
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }

    }
}
