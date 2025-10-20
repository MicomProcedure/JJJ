using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using JJJ.Core.Entities;
using JJJ.Core.Interfaces;
using JJJ.Utils;
using JJJ.View;
using KanKikuchi.AudioManager;
using R3;
using ZLogger;

namespace JJJ.UseCase
{
  public class GameInitializer : IDisposable
  {
    private readonly ITimerService _timerService;
    private readonly ISceneManager _sceneManager;

    private readonly IGameModeProvider _gameModeProvider;
    private readonly IGameSettingsProvider _gameSettingsProvider;
    private readonly GameStateProvider _gameStateProvider;

    private readonly CurrentScorePresenter _currentScorePresenter;
    private readonly CurrentJudgesPresenter _currentJudgesPresenter;
    private readonly GameRemainTimePresenter _remainGameTimePresenter;
    private readonly ITimerRemainsPresenter _timerRemainsPresenter;
    private readonly IGameReadyAnimationPresenter _gameReadyAnimationPresenter;
    private readonly IGameEndAnimationPresenter _gameEndAnimationPresenter;
    private readonly IJudgeInput _judgeInput;

    private CompositeDisposable _disposables = new CompositeDisposable();
    private readonly Microsoft.Extensions.Logging.ILogger _logger = LogManager.CreateLogger<GameInitializer>();

    public GameInitializer(
      ITimerService timerService,
      ISceneManager sceneManager,
      IGameModeProvider gameModeProvider,
      IGameSettingsProvider gameSettingsProvider,
      GameStateProvider gameStateProvider,
      CurrentScorePresenter currentScorePresenter,
      CurrentJudgesPresenter currentJudgesPresenter,
      GameRemainTimePresenter remainGameTimePresenter,
      ITimerRemainsPresenter timerRemainsPresenter,
      IGameReadyAnimationPresenter gameReadyAnimationPresenter,
      IGameEndAnimationPresenter gameEndAnimationPresenter,
      IJudgeInput judgeInput
    )
    {
      _timerService = timerService;
      _sceneManager = sceneManager;
      _gameModeProvider = gameModeProvider;
      _gameSettingsProvider = gameSettingsProvider;
      _gameStateProvider = gameStateProvider;
      _currentScorePresenter = currentScorePresenter;
      _currentJudgesPresenter = currentJudgesPresenter;
      _remainGameTimePresenter = remainGameTimePresenter;
      _timerRemainsPresenter = timerRemainsPresenter;
      _gameReadyAnimationPresenter = gameReadyAnimationPresenter;
      _gameEndAnimationPresenter = gameEndAnimationPresenter;
      _judgeInput = judgeInput;
    }

    public async UniTask InitializeGame(CancellationToken cancellationToken = default)
    {
      BindPresenters();
      SetTimeLimit();
      await UniTask.Delay(500, cancellationToken: cancellationToken);
      await PlayGameReadyAnimationAsync(cancellationToken);
    }

    private void SetTimeLimit()
    {
      switch (_gameModeProvider.Current)
      {
        case GameMode.Easy:
          _gameStateProvider.JudgeTimeLimit = TimeSpan.FromSeconds(_gameSettingsProvider.EasyJudgeTimeLimit);
          _gameStateProvider.GameEndLimit = TimeSpan.FromSeconds(_gameSettingsProvider.EasyGameTimeLimit);
          break;
        case GameMode.Normal:
          _gameStateProvider.JudgeTimeLimit = TimeSpan.FromSeconds(_gameSettingsProvider.NormalJudgeTimeLimit);
          _gameStateProvider.GameEndLimit = TimeSpan.FromSeconds(_gameSettingsProvider.NormalGameTimeLimit);
          break;
        case GameMode.Hard:
          _gameStateProvider.JudgeTimeLimit = TimeSpan.FromSeconds(_gameSettingsProvider.HardJudgeTimeLimit);
          _gameStateProvider.GameEndLimit = TimeSpan.FromSeconds(_gameSettingsProvider.HardGameTimeLimit);
          break;
        default:
          _logger.ZLogWarning($"[GameInitializer] Unknown GameMode {_gameModeProvider.Current}, defaulting judge limit to 10 seconds.");
          _logger.ZLogWarning($"[GameInitializer] Unknown GameMode {_gameModeProvider.Current}, defaulting game end limit to 60 seconds.");
          _gameStateProvider.JudgeTimeLimit = TimeSpan.FromSeconds(10);
          _gameStateProvider.GameEndLimit = TimeSpan.FromSeconds(60);
          break;
      }
      _logger.ZLogDebug($"[GameInitializer] Set JudgeTimeLimit to {_gameStateProvider.JudgeTimeLimit}, GameEndLimit to {_gameStateProvider.GameEndLimit}");
      _gameStateProvider.JudgeRemainTime.Value = _gameStateProvider.JudgeTimeLimit;
      _gameStateProvider.GameRemainTime.Value = _gameStateProvider.GameEndLimit;
    }

    private void BindPresenters()
    {
      _gameStateProvider.JudgeCount
        .Subscribe(count => _currentJudgesPresenter.SetCurrentJudges(count))
        .AddTo(_disposables);
      _gameStateProvider.CurrentScore
        .Subscribe(score => _currentScorePresenter.SetCurrentScore(score))
        .AddTo(_disposables);
      _gameStateProvider.ScoreDiff
        .Skip(1) // 初期値の0を無視する
        .Subscribe(diff => _currentScorePresenter.SetScoreDiff(diff))
        .AddTo(_disposables);
      _gameStateProvider.GameRemainTime
        .Subscribe(limit => _remainGameTimePresenter.SetRemainGameTime((int)limit.TotalSeconds))
        .AddTo(_disposables);
      _gameStateProvider.JudgeRemainTime
        .Subscribe(limit => _timerRemainsPresenter.SetTimerRemains((float)limit.TotalSeconds, (float)_gameStateProvider.JudgeTimeLimit.TotalSeconds))
        .AddTo(_disposables);
      _gameStateProvider.IsInputEnabled
        .Subscribe(enabled => _judgeInput.SetInputEnabled(enabled))
        .AddTo(_disposables);
    }

    private async UniTask PlayGameReadyAnimationAsync(CancellationToken cancellationToken)
    {
      await _gameReadyAnimationPresenter.PlayGameReadyAnimation(cancellationToken);
      _timerService.Countdown(_gameStateProvider.GameEndLimit, TimeSpan.FromSeconds(1), cancellationToken)
        .Subscribe(remaining =>
        {
          _gameStateProvider.GameRemainTime.Value = remaining;
        },
        _ =>
        {
          _gameStateProvider.IsInputEnabled.Value = false;
          if (cancellationToken.IsCancellationRequested) return;
          _gameStateProvider.OnTimerHasExpired.OnNext(Unit.Default);
        })
        .AddTo(_disposables);
      _gameStateProvider.IsInputEnabled.Value = true;
      BGMManager.Instance.Play(BGMPath.BGM2);
    }

    public async UniTask OnGameEnd(CancellationToken cancellationToken = default)
    {
      _gameStateProvider.IsInputEnabled.Value = false;
      await _gameEndAnimationPresenter.PlayGameEndAnimation(cancellationToken);
      _gameStateProvider.CurrentResultSceneData.Score = _gameStateProvider.CurrentScore.Value;
      await _sceneManager.PushWithFade(SceneNavigationUtil.ResultSceneIdentifier, _gameStateProvider.CurrentResultSceneData);
    }

    public void Dispose()
    {
      _disposables?.Dispose();
    }
  }
}