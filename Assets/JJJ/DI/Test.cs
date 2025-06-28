using JJJ.Core.Entities;
using JJJ.Core.Interfaces;
using VContainer.Unity;

public class Test : IStartable
{
    private readonly IJudgeService judgeService;

    public Test(IJudgeService judgeService)
    {
        this.judgeService = judgeService;
    }

    void IStartable.Start()
    {
        UnityEngine.Debug.Log(judgeService.Judge(Hand.Rock, Hand.Scissors, new TurnContext()));
    }
}

public class JudgeService : IJudgeService
{
  public JudgeResult Judge(Hand playerHand, Hand opponentHand, TurnContext turnContext)
  {
    return new JudgeResult(JudgeResultType.Win, playerHand, opponentHand);
  }
}