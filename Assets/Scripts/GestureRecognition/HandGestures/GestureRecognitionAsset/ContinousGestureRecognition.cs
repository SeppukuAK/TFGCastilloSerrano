using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace SocialPresenceVR
{
    /// <summary>
    /// Componente encargado de gestionar el reconocimiento de gestos con la mano derecha
    /// TODO: Debug en Texto de escena
    /// TODO: LoadGestureFile: Que se arrastre el archivo y no la ruta
    /// TODO: SerializeField?
    /// </summary>
    public class ContinousGestureRecognition : MonoBehaviour
    {
        private enum GestureType {WAVE, OK, NO, HAND_UP, POINT}
        private enum HandState {OPEN_HAND,CLOSED_HAND }

        /// <summary>
        /// Similitud entre en el gesto de la base de datos y el detectado para que sea detectado como gesto realizado
        /// </summary>
        [Range(0f, 1f)]
        public float GestureSimilarity;

        /// <summary>
        /// Intervalo en segundos en el que se realiza la identificación del gesto.
        /// </summary>
        [SerializeField] private float RecognitionInterval = 0.1f;

        /// <summary>
        /// Duración aproximada en segundos de los gestos a detectar.
        /// </summary>
        [SerializeField] private float GesturePeriod = 1.0f;

        /// <summary>
        /// Archivo del que se cargan los gestos.
        /// </summary>
        [SerializeField] private string LoadGesturesFile = "Sample_Continuous_Gestures.dat";

        /// <summary>
        /// Proporciona funciones para grabar, identificar y guardar gestos.
        /// </summary>
        private GestureRecognition gestureRecognition;

        /// <summary>
        /// Referencia a la mano derecha
        /// </summary>
        private GameObject rightHand;

        /// <summary>
        /// Último segundo en el que una detección de gesto se ha realizado
        /// </summary>
        private float lastRecognitionTime = 0.0f;

        private HandState handState;

        /// <summary>
        /// Obtiene referencias
        /// </summary>
        private void Awake()
        {
            //Documentación del asset lo hace así
            rightHand = GameObject.Find("RightHand Controller");
            if (rightHand == null)
            {
                Debug.LogError("Mano derecha no encontrada");
                return;
            }
        }

        /// <summary>
        /// Inicializa reconocimiento de gestos
        /// </summary>
        private void Start()
        {
            //Inicialización gestos de la mano.
            gestureRecognition = new GestureRecognition();

            //Carga los gestos de un archivo de base de datos de gestos
            string gesturesFilePath = "Assets/GestureRecognition";
            if (gestureRecognition.loadFromFile(gesturesFilePath + "/" + LoadGesturesFile) == false)
            {
                Debug.LogError("Error cargando gestos");
                return;
            }
            gestureRecognition.contdIdentificationSmoothing = 5;

            //Imprime los gestos disponibles
            for (int i = 0; i < gestureRecognition.numberOfGestures(); i++)
                Debug.Log("\n" + (i + 1) + " : " + gestureRecognition.getGestureName(i));

            //Empieza detección de gestos
            gestureRecognition.contdIdentificationPeriod = (int)(this.GesturePeriod * 1000.0f); // to milliseconds
            Vector3 headsetPos = Camera.main.gameObject.transform.position;
            Quaternion headsetRotation = Camera.main.gameObject.transform.rotation;
            gestureRecognition.startStroke(headsetPos, headsetRotation);
        }

        /// <summary>
        /// Detección de gestos continua
        /// </summary>
        private void Update()
        {
            //Instruciones mientras se realiza el reconocimiento de gesto
            Vector3 controllerPos = rightHand.transform.position;
            Quaternion controllerRotation = rightHand.transform.rotation;
            gestureRecognition.contdStrokeQ(controllerPos, controllerRotation);

            //Se detecta nuevo gesto cada 'RecognitionInterval' segundos
            if (Time.time - lastRecognitionTime > RecognitionInterval)
            {
                Vector3 headsetPos = Camera.main.gameObject.transform.position;
                Quaternion headsetRotation = Camera.main.gameObject.transform.rotation;

                //Información del tipo de gesto actual
                double similarity = -1.0; // [0, 1]
                int gestureID = gestureRecognition.contdIdentify(headsetPos, headsetRotation, ref similarity);

                //Gesto válido
                if (gestureID >= 0)
                {
                    string gestureName = gestureRecognition.getGestureName(gestureID);

                    //Procesamiento del gesto
                    GestureManagement(gestureID, similarity, gestureName);
                }
                lastRecognitionTime = Time.time;
            }

        }

        /// <summary>
        /// Procesamiento del gesto
        /// </summary>
        /// <param name="gestureID"></param>
        /// <param name="similarity"></param>
        /// <param name="gestureName"></param>
        private void GestureManagement(int gestureID, double similarity, string gestureName)
        {
            if (similarity > GestureSimilarity)
                Debug.Log("Gesto reconocido " + gestureName + ", Similitud: " + similarity + "%");
        }
    }
}