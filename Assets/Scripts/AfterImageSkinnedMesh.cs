using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SkinnedMeshと複数のボーンからなる残像用オブジェクトにアタッチするコンポーネント
/// </summary>
public class AfterImageSkinnedMesh : AfterImageBase
{
    [SerializeField] private GameObject _meshParent = null;
    [SerializeField] private GameObject _boneParent = null;
    [SerializeField, ReadOnly] private List<SkinnedMeshRenderer> _skinnedMeshRenderers = new List<SkinnedMeshRenderer>();
    [SerializeField, ReadOnly] private List<Transform> _bones = new List<Transform>();

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

        // マテリアルを設定して初期化
        foreach (var mesh in _skinnedMeshRenderers)
        {
            mesh.sharedMaterial = _material;
        }

        isInitialized = true;
    }

    /// <summary>
    /// 残像の開始時の設定処理
    /// </summary>
    public override void Setup(IAfterImageSetupParam param)
    {
        if (param is AfterImageSkinnedMeshParam useBonesParam)
        {
            Transform tmp = null;
            for (int i = 0; i < _bones.Count; i++)
            {
                tmp = useBonesParam.transforms.Find(b => b.name == _bones[i].name);
                if (tmp != null)
                {
                    _bones[i].position = tmp.position;
                    _bones[i].rotation = tmp.rotation;
                    _bones[i].localScale = tmp.localScale;
                }
            }
        }
        else
        {
            Debug.LogError("引数がUseBonesAfterImageParamにキャスト出来ませんでした");
        }

        rate = 0f;
    }
}
