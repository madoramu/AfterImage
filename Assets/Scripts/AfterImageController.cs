using UnityEngine;

/// <summary>
/// 残像の操作用コンポーネント
/// </summary>
public class AfterImageController : AfterImageControllerBase
{
    [SerializeField, Header("残像の発生元となるオブジェクト")] private Transform _originalTransform = null;

    protected override void SetupParam()
    {
        _param = new SimpleAfterImageParam(_originalTransform);
    }
}
