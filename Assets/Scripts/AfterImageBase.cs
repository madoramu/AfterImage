using System;
using UnityEngine;
using UniRx;

/// <summary>
/// 残像用オブジェクトの基底クラス
/// </summary>
public abstract class AfterImageBase : MonoBehaviour
{
    [SerializeField] protected Shader _shader = null;
    [SerializeField, RangeReactiveProperty(0f, 1f)] protected FloatReactiveProperty _rate = new FloatReactiveProperty(0f);
    [SerializeField] protected Gradient _gradient = null;
    [SerializeField, ReadOnly] protected Material _material = null;

    protected int _baseColorID = -1;

    public float rate { get => _rate.Value; set => _rate.Value = Mathf.Clamp01(value); }
    public bool isInitialized { get; protected set; } = false;

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
    public virtual void Initialize()
    {
        if (isInitialized)
        {
            return;
        }

        if (_shader == null)
        {
            throw new Exception("Shader null");
        }

        // ID取得
        _baseColorID = Shader.PropertyToID("_BaseColor");
        // シェーダーを元にマテリアルを作成
        _material = new Material(_shader);
        // Rateに変化があったらColorを設定するように登録
        _rate.Subscribe(value => _material.SetColor(_baseColorID, _gradient.Evaluate(value)));
        rate = 0f;
    }

    /// <summary>
    /// 残像の開始時の設定処理
    /// </summary>
    public abstract void Setup(IAfterImageSetupParam param);
}
