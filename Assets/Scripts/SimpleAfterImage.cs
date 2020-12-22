using System;
using UnityEngine;

/// <summary>
/// 残像用オブジェクトにアタッチするコンポーネント
/// </summary>
public class SimpleAfterImage : AfterImageBase
{
    [SerializeField] private MeshRenderer _meshRenderer = null;

    /// <summary>
    /// 初期化処理
    /// </summary>
    public override void Initialize()
    {
        if (isInitialized)
        {
            return;
        }
        base.Initialize();

        if (_meshRenderer == null)
        {
            throw new Exception("Mesh Renderer null");
        }

        // マテリアルを設定
        _meshRenderer.material = _material;
        isInitialized = true;
    }

    /// <summary>
    /// 残像の開始時の設定処理
    /// </summary>
    public override void Setup(IAfterImageSetupParam param)
    {
        if (param is SimpleAfterImageParam simpleParam)
        {
            transform.position = simpleParam.transform.position;
            transform.rotation = simpleParam.transform.rotation;
            transform.localScale = simpleParam.transform.localScale;
        }
        else
        {
            Debug.LogError("引数がSimpleAfterImageParamにキャスト出来ませんでした");
        }
        
        rate = 0f;
    }
}
