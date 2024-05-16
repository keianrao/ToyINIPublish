/* copyright

This file is part of ToyINIPublish.
Written in 2024 by Keian Rao <keian.rao@gmail.com>

This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with this program.  If not, see <https://www.gnu.org/licenses/>.

copyright */


public class
INIApi {

    public static void
    Put(string filename, string key, string value)
    {
        SortedDictionary<string, string> interim =
            IniFileToSortedDictionary(filename);

        interim[key] = value;
        
        SortedDictionaryToIniFile(interim, filename);
    }

    public static string?
    Get(string filename, string key)
    {
        return IniFileToSortedDictionary(filename)
            .GetValueOrDefault(key);
    }

    public static bool
    IsAllowedINIFilename(string filename)
    {
        return new string[] {
            "colours.ini"
        }.Contains(filename);
    }

//  ---%-@-%---

    private static SortedDictionary<string, string>
    IniFileToSortedDictionary(string filename)
    {
        SortedDictionary<string, string> returnee = new();

        string[] lines = File.ReadAllLines(filename);
        string category = "";
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
            string category = "";
            string key = e.Key;

            int dotOffset = e.Key.LastIndexOf('.');
            if (dotOffset != -1)
            {
                category = e.Key[0..dotOffset];
                key = e.Key[(dotOffset + 1)..];
            }

            if (!category.Equals(lastCategory))
            {
                outputLines.Add("");
                outputLines.Add("[" + category + "]");               
            }
            /*
            * (悪) Known bug: When keys without categories are
            * added, they are sorted in the dictionary by the
            * basename itself, which can be farther down than
            * the intended beginning of the file before any
            * of the categories.
            *
            * There's no hack through sorting we can do, so
            * mitigation is probably prepending a period to
            * non-category keys so that they have a blank
            * category. It would make things wrong for direct
            * dictionary lookup, so INIApi.GetValue would
            * have to be the one to specially compensate.
            */

            outputLines.Add(key + " = " + e.Value);

            lastCategory = category;
        }

        File.WriteAllLines(filename, outputLines);
    }

}