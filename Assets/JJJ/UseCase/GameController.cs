using System;
using Cysharp.Threading.Tasks;
using R3;
using VContainer.Unity;

namespace JJJ.UseCase
{
  /// <summary>
  /// ゲーム全体を制御するコントローラー
  /// </summary>
  public class GameController : IStartable, IDisposable
  {
    private readonly GameInitializer _gameInitializer;
    private readonly GameStateProvider _gameStateProvider;
    private readonly GameSessionManager _gameSessionManager;

    private CompositeDisposable _disposables = new CompositeDisposable();

    public GameController(
      GameInitializer gameInitializer,
      GameStateProvider gameStateProvider,
      GameSessionManager gameSessionManager
    )
    {
      _gameInitializer = gameInitializer;
      _gameStateProvider = gameStateProvider;
      _gameSessionManager = gameSessionManager;
    }

    /// <summary>
    /// ゲーム開始処理
    /// </summary>
    public async UniTask StartGame()
    {
      // 初期化処理
      await _gameInitializer.InitializeGame(_gameStateProvider.GameEndCancellationTokenSource.Token);
      // ゲームセッション開始
      // 時間切れかタイトルに戻るボタンが押されるまで繰り返す
      while (_gameStateProvider.GameEndCancellationTokenSource.IsCancellationRequested == false)
      {
        await _gameSessionManager.StartSession(_gameStateProvider.GameEndCancellationTokenSource.Token);
      }
    }

    /// <summary>
    /// ゲーム終了処理
    /// </summary>
    /// <remarks>ここには時間切れで終了したときとタイトルに戻るときの両方が含まれる</remarks>
    private void OnGameEnd()
    {
      _gameStateProvider.GameEndCancellationTokenSource.Cancel();
    }

    void IStartable.Start()
    {
      _gameStateProvider.OnGameEnd.Subscribe(_ => OnGameEnd()).AddTo(_disposables);
      _gameStateProvider.OnTimerHasExpired.Subscribe(async _ => {
        OnGameEnd();
        await _gameInitializer.OnGameEnd();
      }).AddTo(_disposables);
      StartGame().Forget();
    }

    public void Dispose()
    {
      _disposables?.Dispose();
    }
  }
}