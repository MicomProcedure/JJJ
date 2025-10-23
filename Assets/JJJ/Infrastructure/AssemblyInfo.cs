using System.Runtime.CompilerServices;

// テストアセンブリからinternalクラスへのアクセスを許可
[assembly: InternalsVisibleTo("JJJ.EditModeTests")]
[assembly: InternalsVisibleTo("JJJ.PlayModeTests")]