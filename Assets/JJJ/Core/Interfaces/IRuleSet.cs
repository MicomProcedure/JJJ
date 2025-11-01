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
    /// <param name="personType">手を出した人の種類</param>
    /// <returns>判定の結果</returns>
    public HandValidationResult ValidateHand(Hand hand, TurnContext turnContext, PersonType personType);
  }
}