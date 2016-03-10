using System.Collections.Generic;
using System.Text;

public class ListToStringConverter
{
    public static List<int> GetListFromString(string inputString)
    {
        var list = new List<int>();

        if (inputString != "")
        {
            foreach (var s in inputString.Split(','))
                list.Add(int.Parse(s));
        }

        return list;
    }

    public static string MakeStringFromList<T>(List<T> source) //, string delimiter)
    {
        var s = new StringBuilder();
        var first = true;

        foreach (var t in source)
        {
            if (first)
                first = false;
            else
                s.Append(','); //delimiter);

            s.Append(t);
        }

        return s.ToString();
    }
}