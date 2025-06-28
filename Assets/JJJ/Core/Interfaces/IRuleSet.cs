using JJJ.Core.Entities;

namespace JJJ.Core.Interfaces
{
  /// <summary>
  /// じゃんけんのルールセットを定義するインターフェース
  /// </summary>
  public interface IRuleSet
  {
    /// <summary>
    /// じゃんけんの結果をルールに基づいて判定する
    /// </summary>
    /// <param name="playerHand">プレイヤーの手</param>
    /// <param name="opponentHand">対戦相手の手</param>
    /// <param name="turnContext">ターンのコンテキスト</param>
    /// <returns>じゃんけんの結果</returns>
    public JudgeResult Judge(Hand playerHand, Hand opponentHand, TurnContext turnContext);

    /// <summary>
    /// じゃんけんの手がルールに基づいて反則でないかどうかを判定する
    /// </summary>
    /// <param name="hand">判定する手</param>
    /// <param name="turnContext">ターンのコンテキスト</param>
    /// <returns>手が有効な場合は true、無効な場合は false</returns>
    public bool IsValidHand(Hand hand, TurnContext turnContext);
  }
}