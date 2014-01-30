
namespace Bebbs.LightWack
{
    public interface IFactory
    {
        T Construct<T>();
    }
}
