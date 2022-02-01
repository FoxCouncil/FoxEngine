namespace FoxEngineLib.Types;

public static class Extensions
{
    public static int RangeConvert(this int input, int x1, int x2, int y1, int y2)
    {
        return (((input - x1) * (y2 - y1)) / (x2 - x1)) + y1;
    }

    public static void Resize<T>(this List<T> list, int sz, T c)
    {
        int cur = list.Count;

        if (sz < cur)
        {
            list.RemoveRange(sz, cur - sz);
        }
        else if (sz > cur)
        {
            if (sz > list.Capacity) //this bit is purely an optimisation, to avoid multiple automatic capacity changes.
            {
                list.Capacity = sz;
            }

            list.AddRange(Enumerable.Repeat(c, sz - cur));
        }
    }

    public static void Resize<T>(this List<T> list, int sz) where T : new()
    {
        Resize(list, sz, new T());
    }
}
