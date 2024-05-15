/* copyright

ToyINIPublish
Written in 2024 by Keian Rao <keian.rao@gmail.com>

This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with this program.  If not, see <https://www.gnu.org/licenses/>.

copyright */

using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;

static Boolean
IsAllowedINIFilename(string filename)
{
    return true;
}

static JsonObject
DisallowedINIFilenameError(string name)
{
    string msg = "'" + name + "' is not an allowed INI file name.";
    JsonObject returnee = new();
    returnee.Add("error", msg);
    return returnee;
}

static JsonObject
WrongParamTypeError(string fieldName, string type)
{
    string msg = fieldName + " parameter is not a " + type + ".";
    JsonObject returnee = new();
    returnee.Add("error", msg);
    return returnee;
}

IResult PutKeyValue(string name, [FromBody] JsonObject jsonBody)
{
    string filename = name + ".ini";

    if (!IsAllowedINIFilename(filename))
        return Results.BadRequest(
            DisallowedINIFilenameError(name));
    if (jsonBody["key"]?.GetValueKind() != JsonValueKind.String)
        return Results.BadRequest(
            WrongParamTypeError("key", "string"));
    if (jsonBody["value"]?.GetValueKind() != JsonValueKind.String)
        return Results.BadRequest(
            WrongParamTypeError("value", "string"));

    string key = jsonBody["key"].GetValue<string>();
    string value = jsonBody["value"].GetValue<string>();

    INIApi.Put(filename, key, value);
    return Results.Ok();
}

IResult GetValue(string name, [FromBody] JsonObject jsonBody)
{
    string filename = name + ".ini";
    if (!IsAllowedINIFilename(filename))
        return Results.BadRequest(
            DisallowedINIFilenameError(name));
    if (jsonBody["key"]?.GetValueKind() != JsonValueKind.String)
        return Results.BadRequest(
            WrongParamTypeError("key", "string"));
    
    string key = jsonBody["key"].GetValue<string>();
    string? value = INIApi.Get(filename, key);
    return Results.Ok<string?>(value);
}

//  ---%-@-%---

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseHttpsRedirection();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPut("/ini/{name}/put", PutKeyValue)
    .WithName("PutKeyValue")
    .WithOpenApi();

app.MapGet("/ini/{name}/get", GetValue)
    .WithName("GetValue")
    .WithOpenApi();

app.Run();
