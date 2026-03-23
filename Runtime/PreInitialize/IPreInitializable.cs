using Cysharp.Threading.Tasks;

namespace LumosLib
{
    public interface IPreInitializable
    {
        UniTask<bool> InitAsync(PreInitContext ctx);
    }
}