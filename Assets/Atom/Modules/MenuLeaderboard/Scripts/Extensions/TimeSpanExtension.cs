using System;

public static class TimeSpanExtensionMethods
{
    //Even though they are used like normal methods, extension
    //methods must be declared static. Notice that the first
    //parameter has the 'this' keyword followed by a Transform
    //variable. This variable denotes which class the extension
    //method becomes a part of.
    public static string FormatTimeSpan(this TimeSpan ts)
    {
        int days = ts.Days;
        int hours = ts.Hours;
        int minutes = ts.Minutes;

        if (days > 0)
        {
            return $"{days}d {hours}h";
        }
        else if (hours > 0)
        {
            return $"{hours}h {minutes}m";
        }
        else
        {
            return $"{minutes}m";
        }
    }
}