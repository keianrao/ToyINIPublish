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
