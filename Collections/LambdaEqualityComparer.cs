namespace DotaData.Collections;

internal class LambdaEqualityComparer<T>(Func<T?, T?, bool> equality, Func<T, int> hashCode) : IEqualityComparer<T>
{
    public bool Equals(T? x, T? y)
    {
        return equality(x, y);
    }

    public int GetHashCode(T obj)
    {
        return hashCode(obj);
    }
}