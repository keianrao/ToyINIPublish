/* copyright

This file is part of ToyINIPublish.
Written in 2024 by Keian Rao <keian.rao@gmail.com>

This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with this program.  If not, see <https://www.gnu.org/licenses/>.

copyright */


using System.Reflection.Metadata;

public class
IniApi {

    public static void
    Put(string filename, string key, string value)
    {
        SortedDictionary<string, string> interim;
        try {
            interim = IniFileToSortedDictionary(filename);
        }
        catch (FileNotFoundException) {
            interim = new();
        }
        
        key = TransformNonCategorisedKey(key);
        interim[key] = value;
        
        SortedDictionaryToIniFile(interim, filename);
    }

    public static string?
    Get(string filename, string key)
    {
        return IniFileToSortedDictionary(filename)
            .GetValueOrDefault(TransformNonCategorisedKey(key));
    }

    public static bool
    IsAllowedINIFilename(string filename)
    {
        return new string[] {
            "colours.ini",
            "ProgramTesting.ini"
        }.Contains(filename);
    }

//   -  -%-  -

    private static string
    TransformNonCategorisedKey(string key)
    {
        return !key.Contains('.') ? "." + key : key;
    }

    private static SortedDictionary<string, string>
    IniFileToSortedDictionary(string filename)
    {
        SortedDictionary<string, string> returnee = new();

        string[] lines = File.ReadAllLines(filename);
        string category = ".";
        foreach (string line in lines)
        {
            bool catStart = line.StartsWith('[');
            bool catEnd = line.TrimEnd().EndsWith(']');
            if (catStart && catEnd)
            {
                category = line[1..(line.TrimEnd().Length - 1)];
                category += ".";
                continue; 
            }

            if (line.Trim().Equals(string.Empty))
                continue;

            int eqOffset = line.IndexOf('=');
            if (eqOffset != -1)
            {
                string key = line[0..eqOffset].Trim();
                string value = line[(eqOffset + 1)..].Trim();
                returnee[category + key] = value;
                continue;
            }

            // Invalid line in INI. Ignore.
            continue;
        }

        return returnee;
    }

    private static void
    SortedDictionaryToIniFile(
        SortedDictionary<string, string> dictionary,
        string filename)
    {
        List<string> outputLines = [];

        string lastCategory = "";
        foreach (KeyValuePair<string, string> e in dictionary)
        {
            int dotOffset = e.Key.LastIndexOf('.');
            string category = e.Key[0..dotOffset];
            string key = e.Key[(dotOffset + 1)..];

            if (!category.Equals(lastCategory))
            {
                outputLines.Add("");
                outputLines.Add("[" + category + "]");               
            }

            outputLines.Add(key + " = " + e.Value);

            lastCategory = category;
        }

        File.WriteAllLines(filename, outputLines);
    }

}