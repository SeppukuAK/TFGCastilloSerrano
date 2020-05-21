using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Floating : MonoBehaviour
{
    public PathType pathSystem = PathType.Linear;
    public Vector3[] pathval = new Vector3[2];

    // Start is called before the first frame update
    void Start()
    {
        transform.DOLocalPath(pathval, 6, pathSystem).SetRelative().SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }
}
