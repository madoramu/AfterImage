using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

/// <summary>
/// SkinnedMeshと複数のボーンからなる残像用オブジェクトにアタッチするコンポーネント
/// </summary>
public class AfterImageSkinnedMesh : MonoBehaviour
{
    [SerializeField] private Shader _shader = null;
    [SerializeField, RangeReactiveProperty(0f, 1f)] private FloatReactiveProperty _rate = new FloatReactiveProperty(0f);
    [SerializeField] private Gradient _gradient = null;
    [SerializeField, ReadOnly] private Material _material = null;
    [SerializeField] private GameObject _meshParent = null;
    [SerializeField] private GameObject _boneParent = null;
    [SerializeField, ReadOnly] private List<SkinnedMeshRenderer> _skinnedMeshRenderers = new List<SkinnedMeshRenderer>();
    [SerializeField, ReadOnly] private List<Transform> _bones = new List<Transform>();

    private int _baseColorID = -1;

    public float rate { get => _rate.Value; set => _rate.Value = Mathf.Clamp01(value); }
    public bool isInitialized { get; private set; } = false;

    [ContextMenu("SetRenderers")]
    private void SetRenderers()
    {
        _skinnedMeshRenderers.Clear();
        foreach (var renderer in _meshParent.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            _skinnedMeshRenderers.Add(renderer);
        }
    }

    [ContextMenu("SetBones")]
    private void SetBones()
    {
        _bones.Clear();
        foreach (var bone in _boneParent.GetComponentsInChildren<Transform>())
        {
            _bones.Add(bone);
        }
    }

    private void Awake()
    {
        Initialize();
    }

    private void OnDestroy()
    {
        if (_material != null)
        {
            Destroy(_material);
        }

        _rate.Dispose();
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    public void Initialize()
    {
        if (isInitialized)
        {
            return;
        }

        if (_shader == null)
        {
            throw new Exception("Shader null");
        }
        if (_skinnedMeshRenderers == null || _skinnedMeshRenderers.Count <= 0)
        {
            throw new Exception("Skinned Mesh Renderer Empty");
        }

        // メッシュとボーンの参照を取得しておく
        if (_skinnedMeshRenderers.Count <= 0)
        {
            SetRenderers();
        }
        if (_bones.Count <= 0)
        {
            SetBones();
        }

        // ID取得
        _baseColorID = Shader.PropertyToID("_BaseColor");
        // シェーダーを元にマテリアルを作成
        _material = new Material(_shader);
        // マテリアルを設定して初期化
        foreach (var mesh in _skinnedMeshRenderers)
        {
            mesh.sharedMaterial = _material;
        }
        // Rateに変化があったらColorを設定するように登録
        _rate.Subscribe(value => _material.SetColor(_baseColorID, _gradient.Evaluate(value)));
        rate = 0f;

        isInitialized = true;
    }

    /// <summary>
    /// 残像の開始時の設定処理
    /// </summary>
    public void Setup(List<Transform> bones)
    {
        Transform tmp = null;
        for (int i = 0; i < _bones.Count; i++)
        {
            tmp = bones.Find(b => b.name == _bones[i].name);
            if (tmp != null)
            {
                _bones[i].position = tmp.position;
                _bones[i].rotation = tmp.rotation;
                _bones[i].localScale = tmp.localScale;
            }
        }
        rate = 0f;
    }
}
