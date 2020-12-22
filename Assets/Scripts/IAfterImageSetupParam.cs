using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 残像出現時に設定するためのパラメーター抽象クラス
/// </summary>
public interface IAfterImageSetupParam { }

/// <summary>
/// 通常の残像
/// </summary>
[System.Serializable]
public class SimpleAfterImageParam : IAfterImageSetupParam
{
    public SimpleAfterImageParam(){}
    public SimpleAfterImageParam(Transform original)
    {
        transform = original;
    }
    ~SimpleAfterImageParam()
    {
        transform = null;
    }
    public Transform transform = null;
}

/// <summary>
/// 複数のボーンを使用する場合
/// </summary>
[System.Serializable]
public class AfterImageSkinnedMeshParam : IAfterImageSetupParam
{
    public AfterImageSkinnedMeshParam() { }
    public AfterImageSkinnedMeshParam(List<Transform> original)
    {
        transforms = original;
    }
    ~AfterImageSkinnedMeshParam()
    {
        transforms.Clear();
        transforms = null;
    }
    public List<Transform> transforms = null;
}

