using JJJ.Core.Entities;

namespace JJJ.Core.Interfaces
{
  /// <summary>
  /// じゃんけんの結果を判定するサービスのインターフェース
  /// </summary>
  public interface IJudgeService
  {
    /// <summary>
    /// じゃんけんの結果を判定する
    /// </summary>
    /// <param name="playerHand">プレイヤーの手</param>
    /// <param name="opponentHand">対戦相手の手</param>
    /// <param name="turnContext">ターン情報のコンテキスト</param>
    /// <returns>じゃんけんの結果</returns>
    public JudgeResult Judge(Hand playerHand, Hand opponentHand, TurnContext turnContext);
  }
}