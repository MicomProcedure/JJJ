using MackySoft.Navigathena.SceneManagement;

namespace JJJ.Utils
{
  public static class SceneNavigationUtil
  {
    public static readonly ISceneIdentifier TitleSceneIdentifier = new BuiltInSceneIdentifier("Title");
    public static readonly ISceneIdentifier GameSceneIdentifier = new BuiltInSceneIdentifier("Game");
    public static readonly ISceneIdentifier FadeTransitionIdentifier = new BuiltInSceneIdentifier("FadeTransition");
  }
}