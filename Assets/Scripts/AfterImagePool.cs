using UnityEngine;
using UniRx.Toolkit;

/// <summary>
/// 残像オブジェクトのプーリングクラス
/// </summary>
public class AfterImagePool : ObjectPool<AfterImageBase>
{
    private AfterImageBase _gameObject = null;
    private Transform _parent = null;

    public AfterImagePool(AfterImageBase gameObject, Transform parent)
    {
        SetPoolObject(gameObject);
        SetParent(parent);
    }

    ~AfterImagePool()
    {
        Clear();
    }

    protected override AfterImageBase CreateInstance()
    {
        if (_gameObject == null)
        {
            Debug.LogError("生成元のGameObjectがnullです");
            return null;
        }
        if (_parent == null)
        {
            Debug.LogError("親Transformがnullです");
            return null;
        }

        AfterImageBase afterImage = GameObject.Instantiate<AfterImageBase>(_gameObject, _parent, true);
        afterImage.Initialize();
        return afterImage;
    }

    /// <summary>
    /// 生成オブジェクトの設定
    /// </summary>
    public void SetPoolObject(AfterImageBase gameObject)
    {
        _gameObject = gameObject;
    }

    /// <summary>
    /// 生成オブジェクトの親オブジェクトの設定
    /// </summary>
    public void SetParent(Transform parent)
    {
        _parent = parent;
    }
}
