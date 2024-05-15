
using System.Reflection.Metadata;

public class
INIApi {

    private static Dictionary<string, string>
    interim = new();

//  ---%-@-%---

    public static void
    Put(string filename, string key, string value)
    {
        interim.Add(key, value);
    }

    public static string?
    Get(string filename, string key)
    {
        return interim.GetValueOrDefault(key);
    }

}