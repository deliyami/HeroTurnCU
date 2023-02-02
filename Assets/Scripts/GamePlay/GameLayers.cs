using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    [SerializeField] LayerMask solidObjectsLayer; //solidObjectsLayer
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] LayerMask grassLayer; //grassLayer
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask fovLayer;
    [SerializeField] LayerMask portalLayer;
    [SerializeField] LayerMask triggerLayer;
    [SerializeField] LayerMask ledgeLayer;

    public static GameLayers i { get; set; }

    private void Awake() {
        i = this;
    }

    public LayerMask SolidLayer {
        get => solidObjectsLayer;
    }
    public LayerMask InteractableLayer {
        get => interactableLayer;
    }
    public LayerMask GrassLayer {
        get => grassLayer;
    }
    public LayerMask PlayerLayer {
        get => playerLayer;
    }
    public LayerMask FovLayer {
        get => fovLayer;
    }
    public LayerMask PortalLayer {
        get => portalLayer;
    }
    public LayerMask TriggerLayer => triggerLayer;
    public LayerMask LedgeLayer => ledgeLayer;
    public LayerMask TriggerableLayer {
        get => grassLayer | fovLayer | portalLayer | triggerLayer;
    }
}
