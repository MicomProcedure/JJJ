using JJJ.Core.Entities;

namespace JJJ.Core.Interfaces
{
  /// <summary>
  /// じゃんけんのスコアを計算するインターフェース
  /// </summary>
  public interface IScoreCalculator
  {
    /// <summary>
    /// プレイヤーが入力した判定結果と真実の判定結果を比較してスコアを加算する
    /// </summary>
    /// <param name="playerJudgeResult">プレイヤーが入力した判定結果</param>
    /// <param name="truthJudgeResult">真実の判定結果</param>
    /// <param name="turnContext">ターンのコンテキスト</param>
    // NOTE: 実装に合わせてplayerJudgeResultとtruthJudgeResultの型を変更する
    public void AddScore(JudgeResultType playerJudgeResult, JudgeResult truthJudgeResult, TurnContext turnContext);

    /// <summary>
    /// 現在の合計スコアを取得する
    /// </summary>
    /// <returns>現在の合計スコア</returns>
    // TODO: 型を変更する
    public int GetTotalScore();
  }
}