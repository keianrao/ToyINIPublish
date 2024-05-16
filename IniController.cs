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
        return String.Join('\n', new string[] {
            "/ini/<INI file alias>/PutKeyValue",
            "\tJSON body of 'key', 'value', both strings.",
            "",
            "/ini/<INI file alias>/GetKey",
            "\tJSON body of 'key', a string." });
        // (This endpoint is really just for a basic routing test.)
    }

    public IResult
    PutKeyValue(string alias, [FromBody] PutKeyValueRequest body)
    {
        string filename = alias + ".ini";
        if (!IsAllowedINIFilename(filename))
            return Results.BadRequest(
                DisallowedINIFilenameError(alias));

        INIApi.Put(filename, body.key, body.value);
        return Results.Ok();
    }

    public IResult
    GetValue(string alias, [FromBody] GetValueRequest body)
    {
        string filename = alias + ".ini";
        if (!IsAllowedINIFilename(filename))
            return Results.BadRequest(
                DisallowedINIFilenameError(alias));
        
        string? value = INIApi.Get(filename, body.key);
        if (value == null) return Results.NotFound();
        else return Results.Ok<string>(value);
    }

//  ---%-@-%---

    private static Boolean
    IsAllowedINIFilename(string filename)
    {
        return new string[] {
            "colours.ini"
        }.Contains(filename);
    }

    private static JsonObject
    DisallowedINIFilenameError(string alias)
    {
        string msg =
            "'" + alias + "' " +
            "is not an allowed INI file alias.";
        return new JsonObject() { { "error", msg } };
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