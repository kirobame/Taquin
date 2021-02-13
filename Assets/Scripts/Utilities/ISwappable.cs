public interface ISwappable<in T>
{
    void SwapWith(T other);
}