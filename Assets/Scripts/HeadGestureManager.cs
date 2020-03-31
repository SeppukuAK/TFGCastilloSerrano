using System.Collections;
using UnityEngine;
using FrameSynthesis.VR;

public class HeadGestureManager : MonoBehaviour
{
    public static HeadGestureManager Instance { get; private set; }

    /// <summary>
    /// Segundos desde que se detecta el cabeceo hasta que deja de considerarse cabeceo
    /// </summary>
    public float NodDuration;

    /// <summary>
    /// Segundos desde que se detecta la sacudida hasta que deja de considerarse sacudida
    /// </summary>
    public float HeadShakeDuration;

    public bool Nodding { get; private set; }
    public bool HeadShaking { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Nodding = false;
        HeadShaking = false;

        VRGestureRecognizer.Current.NodHandler += OnNod;
        VRGestureRecognizer.Current.HeadshakeHandler += OnHeadshake;
    }

    private void OnNod()
    {
        StartCoroutine(NodRoutine());
    }

    private void OnHeadshake()
    {
        StartCoroutine(HeadshakeRoutine());
    }

    IEnumerator NodRoutine()
    {
        Nodding = true;
        yield return new WaitForSeconds(NodDuration);
        Nodding = false;
    }

    IEnumerator HeadshakeRoutine()
    {
        HeadShaking = true;
        yield return new WaitForSeconds(NodDuration);
        HeadShaking = false;
    }

}
