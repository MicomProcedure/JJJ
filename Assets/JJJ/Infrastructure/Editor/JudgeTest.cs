#if UNITY_EDITOR
using System.Collections.Generic;
using System.Text;
using JJJ.Core.Entities;
using JJJ.Core.Interfaces;
using UnityEditor;
using UnityEngine;

namespace JJJ.Infrastructure.Editor
{
  /// <summary>
  /// 指定したジャッジセッションを順に評価してログを出力するエディタ拡張
  /// </summary>
  internal sealed class JudgeTestWindow : EditorWindow
  {
    private const string WindowTitle = "Judge Session Simulator";

    private readonly List<JudgeSession> _sessions = new() { new JudgeSession() };
    private readonly List<string> _logs = new();
    private readonly StringBuilder _logBuilder = new();

    private GameMode _selectedGameMode = GameMode.Normal;
    private Vector2 _sessionScroll;
    private Vector2 _logScroll;

    [MenuItem("JJJ/Judge Session Simulator")]
    private static void Open()
    {
      var window = GetWindow<JudgeTestWindow>(utility: false, title: WindowTitle);
      window.minSize = new Vector2(420f, 520f);
    }

    private void OnGUI()
    {
      DrawHeader();
      EditorGUILayout.Space();
      DrawSessions();
      EditorGUILayout.Space();
      DrawControls();
      EditorGUILayout.Space();
      DrawLogs();
    }

    private void DrawHeader()
    {
      EditorGUILayout.LabelField("Judge Session Simulator", EditorStyles.boldLabel);
      EditorGUILayout.HelpBox("リストの内容を先頭から評価し、各ターンのジャッジ結果とルール適用状況を確認できます。", MessageType.Info);

      using (new EditorGUILayout.HorizontalScope())
      {
        EditorGUILayout.LabelField("Game Mode", GUILayout.Width(80f));
        _selectedGameMode = (GameMode)EditorGUILayout.EnumPopup(_selectedGameMode);
      }
    }

    private void DrawSessions()
    {
      EditorGUILayout.LabelField("Judge Sessions", EditorStyles.boldLabel);

      _sessionScroll = EditorGUILayout.BeginScrollView(_sessionScroll, GUILayout.Height(220f));
      for (int i = 0; i < _sessions.Count; i++)
      {
        var session = _sessions[i];
        using (new EditorGUILayout.VerticalScope("box"))
        {
          EditorGUILayout.LabelField($"Turn {i + 1}", EditorStyles.boldLabel);

          using (new EditorGUILayout.HorizontalScope())
          {
            EditorGUILayout.LabelField("Player", GUILayout.Width(50f));
            session.PlayerHand = (HandType)EditorGUILayout.EnumPopup(session.PlayerHand);
            session.PlayerTimeout = EditorGUILayout.ToggleLeft("Timeout", session.PlayerTimeout, GUILayout.Width(80f));
          }

          using (new EditorGUILayout.HorizontalScope())
          {
            EditorGUILayout.LabelField("Opponent", GUILayout.Width(50f));
            session.OpponentHand = (HandType)EditorGUILayout.EnumPopup(session.OpponentHand);
            session.OpponentTimeout = EditorGUILayout.ToggleLeft("Timeout", session.OpponentTimeout, GUILayout.Width(80f));
          }

          using (new EditorGUILayout.HorizontalScope())
          {
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Remove", GUILayout.Width(70f)))
            {
              _sessions.RemoveAt(i);
              GUIUtility.keyboardControl = 0;
              GUIUtility.hotControl = 0;
              break;
            }
          }
        }
      }
      EditorGUILayout.EndScrollView();

      using (new EditorGUILayout.HorizontalScope())
      {
        if (GUILayout.Button("Add Session"))
        {
          _sessions.Add(new JudgeSession());
        }
        if (GUILayout.Button("Clear Sessions"))
        {
          _sessions.Clear();
          _sessions.Add(new JudgeSession());
        }
      }
    }

    private void DrawControls()
    {
      using (new EditorGUILayout.HorizontalScope())
      {
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Run Simulation", GUILayout.Height(28f), GUILayout.Width(160f)))
        {
          RunSimulation();
        }
      }
    }

    private void DrawLogs()
    {
      EditorGUILayout.LabelField("Result Logs", EditorStyles.boldLabel);
      if (_logs.Count == 0)
      {
        EditorGUILayout.HelpBox("シミュレーションを実行するとログが表示されます。", MessageType.None);
        return;
      }

      _logBuilder.Clear();
      for (int i = 0; i < _logs.Count; i++)
      {
        _logBuilder.AppendLine(_logs[i]);
      }

      _logScroll = EditorGUILayout.BeginScrollView(_logScroll, GUILayout.ExpandHeight(true));
      EditorGUILayout.TextArea(_logBuilder.ToString(), GUILayout.ExpandHeight(true));
      EditorGUILayout.EndScrollView();
    }

    private void RunSimulation()
    {
      _logs.Clear();

      if (_sessions.Count == 0)
      {
        _logs.Add("セッションが設定されていません。");
        return;
      }

      var ruleSet = CreateRuleSetForMode(_selectedGameMode);

      var turnContext = new TurnContext();

      for (int i = 0; i < _sessions.Count; i++)
      {
        if (!turnContext.IsPreviousTurnDoubleViolation)
        {
          turnContext.NextTurn();
        }
        else
        {
          _logs.Add($"以前のターンが両者とも反則だったため、ターンを進めません。");
          turnContext.SetPreviousTurnDoubleViolation(false);
        }

        var session = _sessions[i];
        var playerHand = new Hand(session.PlayerHand, session.PlayerTimeout);
        var opponentHand = new Hand(session.OpponentHand, session.OpponentTimeout);

        JudgeResult result;
        try
        {
          result = ruleSet.Judge(playerHand, opponentHand, turnContext);
        }
        catch (System.Exception ex)
        {
          _logs.Add($"Turn {turnContext.TurnCount}: 判定中にエラーが発生しました - {ex.Message}");
          break;
        }

        _logs.Add(FormatResultMessage(turnContext, playerHand, opponentHand, result));
        _logs.Add(FormatContextState(turnContext));

        if (result.Type is not (JudgeResultType.Draw or JudgeResultType.DoubleViolation))
        {
          _logs.Add("勝敗が確定したため TurnContext をリセットしました。");
          turnContext = new TurnContext();
        }
        if (result.Type == JudgeResultType.DoubleViolation)
        {
          turnContext.SetPreviousTurnDoubleViolation(true);
        }
      }
    }

    private static IRuleSet CreateRuleSetForMode(GameMode mode)
    {
      return mode switch
      {
        GameMode.Easy => new EasyRuleSet(),
        GameMode.Normal => new NormalRuleSet(),
        GameMode.Hard => new HardRuleSet(),
        _ => new NormalRuleSet()
      };
    }

    private static string FormatResultMessage(TurnContext turnContext, Hand playerHand, Hand opponentHand, JudgeResult result)
    {
      var builder = new StringBuilder(256);
      builder.Append($"Turn {turnContext.TurnCount}: ");
      builder.Append($"Player [{playerHand.Name}{(playerHand.IsTimeout ? ", Timeout" : string.Empty)}] vs ");
      builder.Append($"Opponent [{opponentHand.Name}{(opponentHand.IsTimeout ? ", Timeout" : string.Empty)}] -> ");
      builder.Append(result.Type);

      if (!result.IsValid)
      {
        builder.Append($" | Player Violation: {result.PlayerViolationType}, Opponent Violation: {result.OpponentViolationType}");
      }

      return builder.ToString();
    }

    private static string FormatContextState(TurnContext context)
    {
      var builder = new StringBuilder(128);
      builder.Append("  TurnContext => ");
      builder.Append($"TurnCount: {context.TurnCount}, ");
      builder.Append($"IsEvenTurn: {context.IsEvenTurn}, ");
      builder.Append($"PlayerAlphaRemaining: {context.GetAlphaRemainingTurns(PersonType.Player)}, ");
      builder.Append($"PlayerBetaRemaining: {context.GetBetaRemainingTurns(PersonType.Player)}, ");
      var playerSealedHand = context.GetSealedHandType(PersonType.Player);
      builder.Append($"PlayerSealedHand: {(playerSealedHand != null ? new Hand(playerSealedHand.Value).Name : "None")}, ");
      builder.Append($"OpponentAlphaRemaining: {context.GetAlphaRemainingTurns(PersonType.Opponent)}, ");
      builder.Append($"OpponentBetaRemaining: {context.GetBetaRemainingTurns(PersonType.Opponent)}, ");
      var opponentSealedHand = context.GetSealedHandType(PersonType.Opponent);
      builder.Append($"OpponentSealedHand: {(opponentSealedHand != null ? new Hand(opponentSealedHand.Value).Name : "None")}");
      return builder.ToString();
    }

    private sealed class JudgeSession
    {
      public HandType PlayerHand = HandType.Rock;
      public bool PlayerTimeout;
      public HandType OpponentHand = HandType.Scissors;
      public bool OpponentTimeout;
    }
  }
}
#endif
