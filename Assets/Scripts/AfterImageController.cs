using System;
using System.Threading;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;

/// <summary>
/// 残像の操作用コンポーネント
/// </summary>
[RequireComponent(typeof(ObservableDestroyTrigger))]
public class AfterImageController : MonoBehaviour
{
    [SerializeField] private AfterImage afterImage = null;
    [SerializeField] private int _preLoadCount = 5;
    [SerializeField, Header("残像の生成間隔")] private float _createIntervalTime = 0.1f;
    [SerializeField, Header("残像の生存時間")] private float _afterImageLifeTime = 0.2f;
    [SerializeField, Header("残像を生成するか")] private BoolReactiveProperty _isCreate = new BoolReactiveProperty(false);

    private AfterImagePool _pool = null;
    private CompositeDisposable _disposable = new CompositeDisposable();

    public bool isCreate { get => _isCreate.Value; set => _isCreate.Value = value; }
    public bool isInitialized { get; private set; } = false;

    private void Awake()
    {
        // プールの準備
        _pool = new AfterImagePool(afterImage);
        _pool.PreloadAsync(_preLoadCount, 1)
            .TakeUntilDestroy(this)
            .Subscribe(null, exception => { Debug.LogException(exception); }, () => { isInitialized = true; });

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
                            AfterImage image = _pool.Rent();
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
