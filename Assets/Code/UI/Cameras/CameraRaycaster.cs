using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Camera))]
public class CameraRaycaster : MonoBehaviour
{
    [SerializeField] float DistanceToBackground = 100f;
    private Camera view;
    private Layer[] _layers;

    private Layer _currentLayer;
    private Layer CurrentLayer
    {
        set
        {
            if (_currentLayer != null && _currentLayer.Name == value.Name)
                return;

            _currentLayer = value;
            OnLayerChanged?.Invoke(this, _currentLayer);
        }
    }

    public EventHandler<Layer> OnLayerChanged;

    // Start is called before the first frame update
    private void Start()
    {
        view = Camera.main;
        _layers = Layer.GetLayers();
    }

    // Update is called once per frame
    private void Update()
    {
        //if we're over a UI object, the only thing we're sending back is the UI element
        if (EventSystem.current.IsPointerOverGameObject())
        {
            CurrentLayer = _layers.First(p => p.Name == Layer.UI);
            return;
        }

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hits = Physics.RaycastAll(ray, DistanceToBackground);
        var priorityHit = FindTopLayerHit(hits);
        
        if (!priorityHit.HasValue)
        {
            CurrentLayer = _layers.First(p => p.Name == Layer.DEFAULT);
            return;
        }

        var layerHit = _layers.First(p => p.Priority == priorityHit.Value.collider.gameObject.layer);
        CurrentLayer = layerHit;

        //todo:  check for mouse click events and broadcast those as well
        if (Input.GetMouseButton(0))
        {
            //todo: left button clicked
        }
        if (Input.GetMouseButton(1))
        {
            //todo: right button clicked
        }
        if (Input.GetMouseButton(2))
        {
            //todo: middle button clicked
        }
    }

    /// <summary>
    /// get the raycast that is associated with the top level layer in order of priority
    /// </summary>
    /// <param name="hits"></param>
    /// <returns></returns>
    private RaycastHit? FindTopLayerHit(RaycastHit[] hits)
    {
        foreach (var layer in _layers)
        {
            foreach (var hit in hits)
            {
                if (hit.collider.gameObject.layer == layer.Priority) //layer int number is the same as the layer.Priority set up
                    return hit;
            }
        }
        return null;
    }

    private RaycastHit? RaycastForLayer(int layer)
    {
        var ray = view.ScreenPointToRay(Input.mousePosition);
        var hasHit = Physics.Raycast(ray, out RaycastHit hit, DistanceToBackground, layer);
        if (hasHit)
            return hit;

        return null;
    }
}
