using System.Threading;
using Cysharp.Threading.Tasks;

namespace JJJ.Core.Interfaces
{
  public interface ITransitionEffect
  {
    public UniTask StartTransition(CancellationToken cancellationToken = default);
    public UniTask EndTransition(CancellationToken cancellationToken = default);
  }
}