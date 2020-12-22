using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 残像の操作用コンポーネント
/// ボーン付きアニメーション対応バージョン
/// </summary>
public class AfterImageSkinnedMeshController : AfterImageControllerBase
{
    [SerializeField, Header("発生元となるオブジェクトのボーン親オブジェクト")] private GameObject _boneParent = null;
    private List<Transform> _bones = new List<Transform>();

    [ContextMenu("SetBones")]
    private void SetBones()
    {
        _bones.Clear();
        foreach (var bone in _boneParent.GetComponentsInChildren<Transform>())
        {
            _bones.Add(bone);
        }
    }

    protected override void SetupParam()
    {
        if (_bones.Count <= 0)
        {
            SetBones();
        }
        _param = new AfterImageSkinnedMeshParam(_bones);
    }
}
