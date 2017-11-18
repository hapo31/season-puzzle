using System.Collections.Generic;

public static class ListPositionMappingExtendMethod {
    public static T PositionAt<T>(this List<T> list, int x, int y, int width)
    {
        return list[y * width + x];
    }

    public static void SetValuePosition<T>(this List<T> list, int x, int y, int width, T value)
    {
        list[y * width + x] = value;
    }

    public static bool InField<T>(this List<T> list, int x, int y, int width)
    {
        return x >= 0 && y >= 0 && x < width && y < width;
    }

    public static T PositionAt<T>(this T[] list, int x, int y, int width)
    {
        return list[y * width + x];
    }

    public static void SetValuePosition<T>(this T[] list, int x, int y, int width, T value)
    {
        list[y * width + x] = value;
    }

    public static bool InField<T>(this T[] list, int x, int y, int width)
    {
        return x >= 0 && y >= 0 && x < width && y < width;
    }
}
