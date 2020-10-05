using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    [SerializeField]
    private Material _materialGood = null;
    [SerializeField]
    private Material _materialBad = null;

    private MeshRenderer _renderer = null;
    private List<int> _colliders = new List<int>();

    private bool _tooFar = false;
    public bool tooFar
    {
        set 
        { 
            _tooFar = value;
            if (value)
                _renderer.material = _materialBad;
            else
                _renderer.material = (isGood) ? _materialGood : _materialBad;
        }
    }

    public bool isGood
    {
        get => _colliders.Count == 0 && !_tooFar;
    }

    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        _renderer.material = _materialGood;
    }

    private void OnTriggerEnter(Collider other)
    {
        _renderer.material = _materialBad;
        if (!_colliders.Contains(other.GetHashCode()))
            _colliders.Add(other.GetHashCode());
    }

    private void OnTriggerExit(Collider other)
    {
        if (_colliders.Contains(other.GetHashCode()))
            _colliders.Remove(other.GetHashCode());
        if (_colliders.Count == 0 && !_tooFar)
            _renderer.material = _materialGood;
    }

}
