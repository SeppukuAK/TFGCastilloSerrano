using System.Collections;
using UnityEngine;
using FrameSynthesis.VR;

/// <summary>
/// Componente que guarda el estado actual de gestos con la cabeza
/// </summary>
public class HeadGestureState : MonoBehaviour
{
    /// <summary>
    /// Segundos desde que se detecta el cabeceo hasta que deja de considerarse cabeceo.
    /// </summary>
    public float NodDuration;

    /// <summary>
    /// Segundos desde que se detecta la sacudida hasta que deja de considerarse sacudida.
    /// </summary>
    public float HeadShakeDuration;

    /// <summary>
    /// Devuelve si está actualmente cabeceando. (SI)
    /// </summary>
    public bool Nodding { get; private set; }

    /// <summary>
    /// Devuelve si está actualmente sacudiendo la cabeza. (NO)
    /// </summary>
    public bool HeadShaking { get; private set; }

    /// <summary>
    /// Inicializa variables.
    /// Suscripción a eventos.
    /// </summary>
    private void Start()
    {
        Nodding = false;
        HeadShaking = false;

        VRGestureRecognizer.Current.NodHandler += OnNod;
        VRGestureRecognizer.Current.HeadshakeHandler += OnHeadshake;
    }

    /// <summary>
    /// Evento producido cuando se detecta un (SI)
    /// </summary>
    private void OnNod()
    {
        StartCoroutine(NodRoutine());
    }

    /// <summary>
    /// Rutina que maneja la detección del (SI)
    /// </summary>
    /// <returns></returns>
    private IEnumerator NodRoutine()
    {
        Nodding = true;
        yield return new WaitForSeconds(NodDuration);
        Nodding = false;
    }

    /// <summary>
    /// Evento producido cuando se detecta un (NO)
    /// </summary>
    private void OnHeadshake()
    {
        StartCoroutine(HeadshakeRoutine());
    }

    /// <summary>
    /// Rutina que maneja la detección del (NO)
    /// </summary>
    /// <returns></returns>
    private IEnumerator HeadshakeRoutine()
    {
        HeadShaking = true;
        yield return new WaitForSeconds(NodDuration);
        HeadShaking = false;
    }

}
