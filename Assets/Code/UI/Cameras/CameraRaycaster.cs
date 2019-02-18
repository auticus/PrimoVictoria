using System;
using System.Linq;
using UnityEngine;

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
            if (_currentLayer == value)
                return;

            _currentLayer = value;
            Debug.Log($"Layer changed to {value}");
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
        //loop through each potential layer.  When you find the first one, set it and bounce out.  Right now we don't want to know every layer, just the top most layer that was hit.
        foreach(var layer in _layers.OrderBy(p=>p.Priority))
        {
            var layer_mask = LayerMask.GetMask(layer.Name);
            var hit = RaycastForLayer(layer_mask);
            if (hit.HasValue)
            {
                CurrentLayer = layer;
                return;
            }
        }
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
