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
    /// βの残りターン数
    /// </summary>
    public int BetaRemainingTurns { get; private set; }
    /// <summary>
    /// βによって封印された手の種類
    /// </summary>
    /// <remarks>
    /// βの残りターン数が0のときは null になる
    /// </remarks>
    public HandType? SealedHandType { get; private set; }

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
      if (BetaRemainingTurns > 0) BetaRemainingTurns--;
      if (BetaRemainingTurns == 0) SealedHandType = null;
      return this;
    }

    /// <summary>
    /// αを有効化する
    /// </summary>
    /// <param name="turns">αの発動ターン数</param>
    /// <returns>更新されたTurnContext</returns>
    public TurnContext ActivateAlpha(int turns)
    {
      if (turns <= 0)
      {
        throw new ArgumentOutOfRangeException(nameof(turns), "Alpha remaining turns must be greater than 0.");
      } 
      AlphaRemainingTurns = turns;
      return this;
    }

    /// <summary>
    /// βを有効化する
    /// </summary>
    /// <param name="turns">βの発動ターン数</param>
    /// <param name="sealedHand">βによって封印される手</param>
    /// <returns>更新されたTurnContext</returns>
    public TurnContext ActivateBeta(int turns, HandType sealedHandType)
    {
      if (turns <= 0)
      {
        throw new ArgumentOutOfRangeException(nameof(turns), "Beta remaining turns must be greater than 0.");
      }
      BetaRemainingTurns = turns;
      SealedHandType = SealedHandType;
      return this;
    }
  }
}