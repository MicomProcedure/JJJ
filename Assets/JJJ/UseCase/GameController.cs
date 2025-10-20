using System;
using Cysharp.Threading.Tasks;
using R3;
using VContainer.Unity;

namespace JJJ.UseCase
{
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

    public async UniTask StartGame()
    {
      await _gameInitializer.InitializeGame(_gameStateProvider.GameEndCancellationTokenSource.Token);
      while (_gameStateProvider.GameEndCancellationTokenSource.IsCancellationRequested == false)
      {
        await _gameSessionManager.StartSession(_gameStateProvider.GameEndCancellationTokenSource.Token);
      }
    }

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