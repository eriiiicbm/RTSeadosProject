using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializedTransform
{
    public float[] _position = new float[3];
    public float[] _rotation = new float[4];
    public float[] _scale = new float[3];
 
 
    public SerializedTransform(Transform transform, bool worldSpace = false)
    {
        _position[0] = transform.localPosition.x;
        _position[1] = transform.localPosition.y;
        _position[2] = transform.localPosition.z;
 
        _rotation[0] = transform.localRotation.w;
        _rotation[1] = transform.localRotation.x;
        _rotation[2] = transform.localRotation.y;
        _rotation[3] = transform.localRotation.z;
 
        _scale[0] = transform.localScale.x;
        _scale[1] = transform.localScale.y;
        _scale[2] = transform.localScale.z;
 
    }
    
    
}
public static class SerializedTransformExtention
{
    public static void DeserialTransform(this SerializedTransform _serializedTransform, Transform _transform)
    { Debug.Log(_serializedTransform._position.ToString() +" y la posicion antigua es " + _transform.position);
        _transform.localPosition = new Vector3(_serializedTransform._position[0], _serializedTransform._position[1], _serializedTransform._position[2]);
        _transform.localRotation = new Quaternion(_serializedTransform._rotation[1], _serializedTransform._rotation[2], _serializedTransform._rotation[3], _serializedTransform._rotation[0]);
        _transform.localScale = new Vector3(_serializedTransform._scale[0], _serializedTransform._scale[1], _serializedTransform._scale[2]);
    }
    
}
  