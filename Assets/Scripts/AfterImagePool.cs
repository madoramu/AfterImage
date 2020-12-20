using UnityEngine;
using UniRx.Toolkit;

/// <summary>
/// 残像オブジェクトのプーリングクラス
/// </summary>
public class AfterImagePool : ObjectPool<AfterImage>
{
    private AfterImage _gameObject = null;

    public AfterImagePool(AfterImage gameObject)
    {
        SetPoolObject(gameObject);
    }

    ~AfterImagePool()
    {
        Clear();
    }

    protected override AfterImage CreateInstance()
    {
        if (_gameObject == null)
        {
            Debug.LogError("生成元のGameObjectがnullです");
            return null;
        }

        AfterImage afterImage = GameObject.Instantiate<AfterImage>(_gameObject);
        afterImage.Initialize();
        return afterImage;
    }

    /// <summary>
    /// 生成オブジェクトの設定
    /// </summary>
    public void SetPoolObject(AfterImage gameObject)
    {
        _gameObject = gameObject;
    }
}
