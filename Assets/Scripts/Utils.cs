using UnityEngine;

public static class Utils
{
    public static Color GetChromakeyColor(int id)
    {
        return id switch
        {
            0 => Color.green,
            1 => Color.blue,
            2 => Color.magenta,
            _ => Color.clear
        };
    }
}