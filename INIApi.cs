/* copyright

This file is part of ToyINIPublish.
Written in 2024 by Keian Rao <keian.rao@gmail.com>

This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with this program.  If not, see <https://www.gnu.org/licenses/>.

copyright */


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