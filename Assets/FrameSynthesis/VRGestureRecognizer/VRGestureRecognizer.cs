using UnityEngine;
using UnityEngine.XR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FrameSynthesis.VR
{
    public struct PoseSample
    {
        public float timestamp;
        public Quaternion orientation;
        public Vector3 eulerAngles;

        public PoseSample(float timestamp, Quaternion orientation)
        {
            this.timestamp = timestamp;
            this.orientation = orientation;

            eulerAngles = orientation.eulerAngles;
            eulerAngles.x = MyMath.WrapDegree(eulerAngles.x);
            eulerAngles.y = MyMath.WrapDegree(eulerAngles.y);
        }
    }

    /// <summary>
    /// Modificación propia de detección de gestos con la cabeza.
    /// Utilizada para modificar los valores de detección de los gestos.
    /// TODO: Se podría arreglar el warning
    /// </summary>
    public class VRGestureRecognizer : MonoBehaviour
    {
        public static VRGestureRecognizer Current { get; private set; }

        [SerializeField]
        float recognitionInterval = 0.5f;

        [Header("Nod")]
        [SerializeField] float maxNodValue = 10f;
        [SerializeField] float nodValue = 5f;

        [Header("Headshake")]
        [SerializeField] float maxHeadShakeValue = 10f;
        [SerializeField] float headShakeValue = 5f;

        public event Action NodHandler;
        public event Action HeadshakeHandler;

        public Queue<PoseSample> PoseSamples { get; } = new Queue<PoseSample>();

        float prevGestureTime;

        void Awake()
        {
            Current = this;
        }

        void Update()
        {
            var orientation = InputTracking.GetLocalRotation(XRNode.Head);

            // Record orientation
            PoseSamples.Enqueue(new PoseSample(Time.time, orientation));
            if (PoseSamples.Count >= 120)
            {
                PoseSamples.Dequeue();
            }

            // Recognize gestures
            RecognizeNod();
            RecognizeHeadshake();
        }

        IEnumerable<PoseSample> PoseSamplesWithin(float startTime, float endTime)
        {
            return PoseSamples.Where(sample => 
                sample.timestamp < Time.time - startTime && 
                sample.timestamp >= Time.time - endTime);
        }

        void RecognizeNod()
        {
            try
            {
                var averagePitch = PoseSamplesWithin(0.2f, 0.4f).Average(sample => sample.eulerAngles.x);
                var maxPitch = PoseSamplesWithin(0.01f, 0.2f).Max(sample => sample.eulerAngles.x);
                var pitch = PoseSamples.First().eulerAngles.x;

                if (maxPitch - averagePitch > maxNodValue &&
                    Mathf.Abs(pitch - averagePitch) < nodValue)
                {
                    if (prevGestureTime < Time.time - recognitionInterval)
                    {
                        prevGestureTime = Time.time;
                        NodHandler?.Invoke();
                    }
                }
            }
            catch (InvalidOperationException)
            {
                // PoseSamplesWithin contains no entry
            }
        }

        void RecognizeHeadshake()
        {
            try
            {
                var averageYaw = PoseSamplesWithin(0.2f, 0.4f).Average(sample => sample.eulerAngles.y);
                var maxYaw = PoseSamplesWithin(0.01f, 0.2f).Max(sample => sample.eulerAngles.y);
                var minYaw = PoseSamplesWithin(0.01f, 0.2f).Min(sample => sample.eulerAngles.y);
                var yaw = PoseSamples.First().eulerAngles.y;

                if ((maxYaw - averageYaw > maxHeadShakeValue || averageYaw - minYaw > maxHeadShakeValue) &&
                    Mathf.Abs(yaw - averageYaw) < headShakeValue)
                {
                    if (prevGestureTime < Time.time - recognitionInterval)
                    {
                        prevGestureTime = Time.time;
                        HeadshakeHandler?.Invoke();
                    }
                }
            }
            catch (InvalidOperationException)
            {
                // PoseSamplesWithin contains no entry
            }
        }
    }
}

