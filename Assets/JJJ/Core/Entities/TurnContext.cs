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
    public int AlphaRemainingTurns { get; private set; }
    /// <summary>
    /// αを発動させた人
    /// </summary>
    public PersonType? AlphaActivatedBy { get; private set; }
    /// <summary>
    /// βの残りターン数
    /// </summary>
    public int BetaRemainingTurns { get; private set; }
    /// <summary>
    /// βを発動させた人
    /// </summary>
    public PersonType? BetaActivatedBy { get; private set; }
    /// <summary>
    /// βによって封印されている手の種類
    /// </summary>
    /// <remarks>
    /// βの残りターン数が0のときは null になる
    /// </remarks>
    public HandType? SealedHandType { get; private set; }
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
    public TurnContext(int turnCount = 0, int alphaRemainingTurns = 0, int betaRemainingTurns = 0, HandType? sealedHandType = null)
    {
      TurnCount = turnCount;
      AlphaRemainingTurns = alphaRemainingTurns;
      BetaRemainingTurns = betaRemainingTurns;
      SealedHandType = sealedHandType;
    }

    /// <summary>
    /// ターンを進める
    /// </summary>
    /// <returns>更新されたTurnContext</returns>
    public TurnContext NextTurn()
    {
      TurnCount++;
      if (AlphaRemainingTurns > 0) AlphaRemainingTurns--;
      if (AlphaRemainingTurns == 0) AlphaActivatedBy = null;
      if (BetaRemainingTurns > 0) BetaRemainingTurns--;
      if (BetaRemainingTurns == 0) {
        SealedHandType = null;
        BetaActivatedBy = null; 
      }
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
      AlphaRemainingTurns = turns;
      AlphaActivatedBy = activatedBy;
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
      BetaRemainingTurns = turns;
      SealedHandType = sealedHandType;
      BetaActivatedBy = activatedBy;
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