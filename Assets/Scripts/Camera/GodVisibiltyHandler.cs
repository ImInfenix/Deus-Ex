using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class GodVisibiltyHandler : MonoBehaviour
{
    static List<GodVisibiltyHandler> instances;

    Camera attachedCamera;

    public LayerMask godVisible;
    public LayerMask godInvisible;

    private void Awake()
    {
        if (instances == null)
            instances = new List<GodVisibiltyHandler>();

        instances.Add(this);

        attachedCamera = GetComponent<Camera>();
    }

    public static void Show(bool value)
    {
        foreach (var i in instances)
        {
            i.attachedCamera.cullingMask = value || GameManager.Instance.LocalPlayerType == GameManager.PlayerType.God ? i.godVisible : i.godInvisible;
        }
    }

    private void OnDestroy()
    {
        instances.Remove(this);
    }
}
