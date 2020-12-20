﻿using System;
using UnityEngine;

/// <summary>
/// 残像用オブジェクトにアタッチするコンポーネント
/// </summary>
public class AfterImage : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer = null;
    [SerializeField] private Shader _shader = null;
    [SerializeField, Range(0f, 1f)] private float _rate = 0f;
    [SerializeField] private Gradient _gradient = null;
    [SerializeField, ReadOnly] private Material _material = null;

    private int _baseColorID = -1;

    private void Awake()
    {
        if (_meshRenderer == null)
        {
            throw new Exception("Mesh Renderer null");
        }

        // IDと進行値を設定
        _rate = 0f;
        _baseColorID = Shader.PropertyToID("_BaseColor");
        // シェーダーを元にマテリアルを作成
        _material = new Material(_shader);
        // マテリアルを設定して初期化
        _meshRenderer.material = _material;
        UpdateColor();
    }

    private void Update()
    {
        UpdateColor();
    }

    private void UpdateColor()
    {
        _material.SetColor(_baseColorID, _gradient.Evaluate(_rate));
    }
}
