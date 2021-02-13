using UnityEngine;

public static class Extensions
{
    public static string Format(this int time)
    {
        var minutes = time / 60;
        var seconds = time % 60;

        var l = minutes >= 10 ? minutes.ToString() : $"0{minutes}";
        var r = seconds >= 10 ? seconds.ToString() : $"0{seconds}";

        return $"{l}:{r}";
    }
    
    public static bool IsEven(this Vector2Int value) => IsEven(value.x + value.y);
    public static bool IsEven(this int value) => value % 2 == 0;

    public static void Swap<T>(this T[,] array, Vector2Int l, Vector2Int r, bool swapValues = true) where T : ISwappable<T>
    {
        var copy = array[l.x, l.y];
        if (swapValues) copy.SwapWith(array[r.x, r.y]);
        
        array[l.x, l.y] = array[r.x, r.y];
        array[r.x, r.y] = copy;
    }

    public static float InverseLerp(this Vector2 value, Vector2 a, Vector2 b)
    {
        var AB = b - a;
        var AV = value - a;
        
        return Vector2.Dot(AV, AB) / Vector2.Dot(AB, AB);
    }
}