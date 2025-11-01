using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using JJJ.Core.Entities;

namespace JJJ.Core.Interfaces
{
  /// <summary>
  /// ランキングデータの保存・取得を担当するリポジトリのインターフェース
  /// </summary>
  public interface IRankingRepository
  {
    /// <summary>
    /// スコアを保存する
    /// </summary>
    /// <param name="gameMode">ゲームモード</param>
    /// <param name="rankingData">保存するスコアデータ</param>
    /// <returns>保存に成功、または失敗したときに完了するUniTask</returns>
    /// <remarks>例外が発生した場合、UniTaskのStatusがFaultedになる</remarks>
    public UniTask SaveScore(GameMode gameMode, RankingData rankingData, CancellationToken? cancellationToken = null);

    /// <summary>
    /// 上位N件のスコアを取得する
    /// </summary>
    /// <param name="gameMode">ゲームモード</param>
    /// <param name="n">取得するスコアの件数</param>
    /// <returns>取得したスコアデータのリストを含むUniTask</returns>
    /// <remarks>例外が発生した場合、UniTaskのStatusがFaultedになる</remarks>
    public UniTask<IReadOnlyList<RankingData>> LoadTopNScores(GameMode gameMode, int n, CancellationToken? cancellationToken = null);
  }
}