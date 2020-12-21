using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 指定した位置と位置を回転しながら往復する
/// </summary>
public class RotatePingpong : MonoBehaviour
{
    [SerializeField] private float _length = 3f;
    [SerializeField] private float _pingPongSpeed = 1f;
    [SerializeField] private Vector3 _eulers = new Vector3(0f, 0f, 1f);

    private Vector3 _position = Vector3.zero;

    void Update()
    {
        _position = transform.position;
        _position.z = Mathf.PingPong(Time.time * _pingPongSpeed, _length);
        transform.position = _position;

        transform.Rotate(_eulers);
    }
}
