#nullable enable

using System;

namespace JJJ.Core.Entities
{
  /// <summary>
  /// じゃんけんのターン情報を表すクラス
  /// </summary>
  public class TurnContext
  {
    /// <summary>
    /// 現在のターン数
    /// </summary>
    public int TurnCount { get; private set; } = 0;
    /// <summary>
    /// 現在のターンが偶数かどうか
    /// </summary>
    public bool IsEvenTurn => TurnCount % 2 == 0;
    /// <summary>
    /// αの残りターン数
    /// </summary>
    private int _playerAlphaRemainingTurns;
    /// <summary>
    /// αを発動させた人
    /// </summary>
    private int _opponentAlphaRemainingTurns;
    /// <summary>
    /// βの残りターン数
    /// </summary>
    private int _playerBetaRemainingTurns;
    /// <summary>
    /// βを発動させた人
    /// </summary>
    private int _opponentBetaRemainingTurns;
    /// <summary>
    /// プレイヤーがβで封印した手の種類
    /// </summary>
    /// <remarks>
    /// βの残りターン数が0のときは null になる
    /// </remarks>
    private HandType? _playerSealedHandType;
    /// <summary>
    /// 相手がβで封印した手の種類
    /// </summary>
    /// <remarks>
    /// βの残りターン数が0のときは null になる
    /// </remarks>
    private HandType? _opponentSealedHandType;
    /// <summary>
    /// 以前のターンが両者とも反則だったかどうか
    /// </summary>
    public bool IsPreviousTurnDoubleViolation { get; private set; } = false;

    /// <summary>
    /// TurnContextクラスのコンストラクタ
    /// </summary>
    /// <param name="turnCount">現在のターン数</param>
    /// <param name="alphaRemainingTurns">αの残りターン数</param>
    /// <param name="betaRemainingTurns">βの残りターン数</param>
    /// <param name="sealedHand">βによって封印された手</param>
    public TurnContext(int turnCount = 0, int playerAlphaRemainingTurns = 0, int opponentAlphaRemainingTurns = 0,
                       int playerBetaRemainingTurns = 0, int opponentBetaRemainingTurns = 0,
                       HandType? playerSealedHandType = null, HandType? opponentSealedHandType = null)
    {
      TurnCount = turnCount;
      _playerAlphaRemainingTurns = playerAlphaRemainingTurns;
      _opponentAlphaRemainingTurns = opponentAlphaRemainingTurns;
      _playerBetaRemainingTurns = playerBetaRemainingTurns;
      _opponentBetaRemainingTurns = opponentBetaRemainingTurns;
      _playerSealedHandType = playerSealedHandType;
      _opponentSealedHandType = opponentSealedHandType;
    }

    public int GetAlphaRemainingTurns(PersonType personType)
    {
      return personType switch
      {
        PersonType.Player => _playerAlphaRemainingTurns,
        PersonType.Opponent => _opponentAlphaRemainingTurns,
        _ => throw new ArgumentOutOfRangeException(nameof(personType), "Invalid PersonType value."),
      };
    }

    public int GetBetaRemainingTurns(PersonType personType)
    {
      return personType switch
      {
        PersonType.Player => _playerBetaRemainingTurns,
        PersonType.Opponent => _opponentBetaRemainingTurns,
        _ => throw new ArgumentOutOfRangeException(nameof(personType), "Invalid PersonType value."),
      };
    }

    public HandType? GetSealedHandType(PersonType personType)
    {
      return personType switch
      {
        PersonType.Player => _playerSealedHandType,
        PersonType.Opponent => _opponentSealedHandType,
        _ => throw new ArgumentOutOfRangeException(nameof(personType), "Invalid PersonType value."),
      };
    }

    /// <summary>
    /// ターンを進める
    /// </summary>
    /// <returns>更新されたTurnContext</returns>
    public TurnContext NextTurn()
    {
      TurnCount++;
      if (_playerAlphaRemainingTurns > 0) _playerAlphaRemainingTurns--;
      if (_opponentAlphaRemainingTurns > 0) _opponentAlphaRemainingTurns--;
      if (_playerBetaRemainingTurns > 0) _playerBetaRemainingTurns--;
      if (_opponentBetaRemainingTurns > 0) _opponentBetaRemainingTurns--;
      if (_playerBetaRemainingTurns == 0) _playerSealedHandType = null;
      if (_opponentBetaRemainingTurns == 0) _opponentSealedHandType = null;
      return this;
    }

    /// <summary>
    /// αを有効化する
    /// </summary>
    /// <param name="turns">αの発動ターン数</param>
    /// <returns>更新されたTurnContext</returns>
    public TurnContext ActivateAlpha(int turns, PersonType activatedBy)
    {
      if (turns <= 0)
      {
        throw new ArgumentOutOfRangeException(nameof(turns), "Alpha remaining turns must be greater than 0.");
      } 
      switch (activatedBy)
      {
        case PersonType.Player:
          _playerAlphaRemainingTurns = turns;
          break;
        case PersonType.Opponent:
          _opponentAlphaRemainingTurns = turns;
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(activatedBy), "Invalid PersonType value.");
      }
      return this;
    }

    /// <summary>
    /// βを有効化する
    /// </summary>
    /// <param name="turns">βの発動ターン数</param>
    /// <param name="sealedHand">βによって封印される手</param>
    /// <returns>更新されたTurnContext</returns>
    public TurnContext ActivateBeta(int turns, HandType sealedHandType, PersonType activatedBy)
    {
      if (turns <= 0)
      {
        throw new ArgumentOutOfRangeException(nameof(turns), "Beta remaining turns must be greater than 0.");
      }
      switch (activatedBy)
      {
        case PersonType.Player:
          _playerBetaRemainingTurns = turns;
          _playerSealedHandType = sealedHandType;
          break;
        case PersonType.Opponent:
          _opponentBetaRemainingTurns = turns;
          _opponentSealedHandType = sealedHandType;
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(activatedBy), "Invalid PersonType value.");
      }
      return this;
    }

    /// <summary>
    /// 以前のターンが両者とも反則だったかどうかを設定する
    /// </summary>
    /// <param name="isDoubleViolation">両者とも反則だったかどうか</param>
    /// <returns>更新されたTurnContext</returns>
    public TurnContext SetPreviousTurnDoubleViolation(bool isDoubleViolation)
    {
      IsPreviousTurnDoubleViolation = isDoubleViolation;
      return this;
    }
  }
}