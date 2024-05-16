/* copyright

This file is part of ToyINIPublish.
Written in 2024 by Keian Rao <keian.rao@gmail.com>

This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with this program.  If not, see <https://www.gnu.org/licenses/>.

copyright */

using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json.Nodes;

public class
IniController : Controller
{    
    public string
    Help()
    {
        StringBuilder b = new StringBuilder();
        b.Append("/ini/<INI file alias>/PutKeyValue");
        b.Append("\n\tJSON body of 'key', 'value', both strings.");
        b.Append("\n");
        b.Append("\n/ini/<INI file alias>/GetKey");
        b.Append("\n\tJSON body of 'key', a string.");

        return b.ToString();
        // (This endpoint is really just for a basic routing test.)
    }

    public IResult
    PutKeyValue(string name, [FromBody] PutKeyValueRequest body)
    {
        string filename = name + ".ini";
        if (!IsAllowedINIFilename(filename))
            return Results.BadRequest(
                DisallowedINIFilenameError(name));

        INIApi.Put(filename, body.key, body.value);
        return Results.Ok();
    }

    public IResult
    GetValue(string name, [FromBody] GetValueRequest body)
    {
        string filename = name + ".ini";
        if (!IsAllowedINIFilename(filename))
            return Results.BadRequest(
                DisallowedINIFilenameError(name));
        
        string? value = INIApi.Get(filename, body.key);
        return Results.Ok<string?>(value);
    }

//  ---%-@-%---

    private static Boolean
    IsAllowedINIFilename(string filename)
    {
        return new string[] {
            "colours"
        }.Contains(filename);
    }

    private static JsonObject
    DisallowedINIFilenameError(string name)
    {
        string msg = "'" + name + "' is not an allowed INI file name.";
        JsonObject returnee = new();
        returnee.Add("error", msg);
        return returnee;
    }

    
//  ---%-@-%---

    public class
    PutKeyValueRequest {

        public required string
        key { get; set; }

        public required string
        value { get; set; }

    }

    public class
    GetValueRequest {

        public required string
        key { get; set; }

    }

}