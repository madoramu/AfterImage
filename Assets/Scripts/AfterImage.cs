using System;
using UnityEngine;
using UniRx;

/// <summary>
/// 残像用オブジェクトにアタッチするコンポーネント
/// </summary>
public class AfterImage : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer = null;
    [SerializeField] private Shader _shader = null;
    [SerializeField, RangeReactiveProperty(0f, 1f)] private FloatReactiveProperty _rate = new FloatReactiveProperty(0f);
    [SerializeField] private Gradient _gradient = null;
    [SerializeField, ReadOnly] private Material _material = null;

    private int _baseColorID = -1;

    public float rate { get => _rate.Value; set => _rate.Value = Mathf.Clamp01(value); }
    public bool isInitialized { get; private set; } = false;

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
        if (_meshRenderer == null)
        {
            throw new Exception("Mesh Renderer null");
        }

        // Rateに変化があったらColorを設定するように登録
        _rate.Subscribe(value => _material.SetColor(_baseColorID, _gradient.Evaluate(value)));
        rate = 0f;
        // ID取得
        _baseColorID = Shader.PropertyToID("_BaseColor");
        // シェーダーを元にマテリアルを作成
        _material = new Material(_shader);
        // マテリアルを設定して初期化
        _meshRenderer.material = _material;

        
        isInitialized = true;
    }

    /// <summary>
    /// 残像の開始時の設定処理
    /// </summary>
    public void Setup(Transform baseTransform)
    {
        transform.position = baseTransform.position;
        transform.rotation = baseTransform.rotation;
        transform.localScale = baseTransform.localScale;

        rate = 0f;
    }
}
