using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraShake : MonoBehaviour
{
    private Transform _camTransform;
    private Vector3 _originPosition;

    private float _shakeDur = 1f, _shakeAmount = 0.04f, _decreaseFactor = 1.5f;
    
    private void Start()
    {
        _camTransform = transform;
        _originPosition = transform.localPosition;

    }

    private void Update()
    {
        if (_shakeDur > 0)
        {
            _camTransform.localPosition = _originPosition + Random.insideUnitSphere * _shakeAmount;
            _shakeDur -= Time.deltaTime * _decreaseFactor;
        }
        else
        {
            _shakeDur = 0;
            _camTransform.localPosition = _originPosition;
        }
    }
}
