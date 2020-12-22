using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;

/// <summary>
/// 残像の操作用コンポーネント
/// ボーン付きアニメーション対応バージョン
/// </summary>
[RequireComponent(typeof(ObservableUpdateTrigger), typeof(ObservableDestroyTrigger))]
public class AfterImageSkinnedMeshController : MonoBehaviour
{
    [SerializeField, Header("発生元となるオブジェクトのボーン親オブジェクト")] private GameObject _boneParent = null;
    [SerializeField, Header("発生させる残像オブジェクト")] private SimpleAfterImage _afterImage = null;
    [SerializeField, Header("残像の親オブジェクト")] private Transform _afterImageParent = null;
    [SerializeField, Header("事前生成数")] private int _preLoadCount = 5;
    [SerializeField, Header("残像の生成間隔")] private float _createIntervalTime = 0.1f;
    [SerializeField, Header("残像の生存時間")] private float _afterImageLifeTime = 0.2f;
    [SerializeField, Header("残像を生成するか")] private BoolReactiveProperty _isCreate = new BoolReactiveProperty(false);
    [SerializeField, ReadOnly] private List<Transform> _bones = new List<Transform>();

    private AfterImageSkinnedMeshParam _param = null;
    private AfterImagePool _pool = null;
    private CompositeDisposable _disposable = new CompositeDisposable();

    // 最適化用
    private ObservableUpdateTrigger _updateTrigger = null;

    public bool isCreate { get => _isCreate.Value; set => _isCreate.Value = value; }
    public bool isInitialized { get; private set; } = false;

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
        // パラメーター生成
        if (_bones.Count <= 0)
        {
            SetBones();
        }
        _param = new AfterImageSkinnedMeshParam(_bones);

        // Updateの発行元をキャッシュしておく
        _updateTrigger = GetComponent<ObservableUpdateTrigger>();

        // プールの準備
        _pool = new AfterImagePool(_afterImage, _afterImageParent);
        _pool.PreloadAsync(_preLoadCount, 1)
            .TakeUntilDestroy(this)
            .Subscribe(_ => { }, exception => { Debug.LogException(exception); }, () => { isInitialized = true; });

        // フラグに応じて処理の登録と破棄を行う
        _isCreate
            .TakeUntilDestroy(this)
            .Where(_ => isInitialized)
            .Subscribe(enable =>
            {
                if (!enable)
                {
                    _disposable.Clear();
                    return;
                }

                Observable.Interval(TimeSpan.FromSeconds(_createIntervalTime))
                        .Subscribe(_ =>
                        {
                            // プールから残像を取得してオリジナルのポーズと合わせる
                            AfterImageSkinnedMesh image = _pool.Rent() as AfterImageSkinnedMesh;
                            image.Setup(_param);

                            // 時間経過処理と終了時にプールに戻す処理を登録しておく
                            float currentTime = 0f;
                            _updateTrigger.UpdateAsObservable()
                                .TakeUntilDisable(image)
                                .Subscribe(unit =>
                                {
                                    currentTime += Time.deltaTime;
                                    image.rate = currentTime / _afterImageLifeTime;
                                    if (currentTime >= _afterImageLifeTime)
                                    {
                                        _pool.Return(image);
                                    }
                                },
                                () => { Debug.Log("正常に破棄されています"); });
                        })
                        .AddTo(_disposable);
            });

        isInitialized = true;
    }

    private void OnDestroy()
    {
        _disposable.Dispose();
        _disposable = null;
    }
}
