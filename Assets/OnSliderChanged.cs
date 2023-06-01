using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnSliderChanged : MonoBehaviour
{
    [SerializeField] Vector3 _originalSize;
    [SerializeField] SpriteRenderer _spriteRenderer;
    // Start is called before the first frame update
    private void Awake()
    {
        _originalSize = transform.localScale;
    }
    
    public void OnValueChangedForSize(float value)
    {
        Debug.Log($"Value: {value}");
        Debug.Log($"LocalScale of the original size: {_originalSize}");
        if (_originalSize != null)
        {
            transform.localScale = value*_originalSize;
        }
    }

    public void OnValueChangedForMaterial(float value)
    {
        Debug.Log($"Value: {value}");
        _spriteRenderer.material.SetFloat("_Density", value);
    }

}
