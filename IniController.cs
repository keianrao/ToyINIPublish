/* copyright

This file is part of ToyINIPublish.
Written in 2024 by Keian Rao <keian.rao@gmail.com>

This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with this program.  If not, see <https://www.gnu.org/licenses/>.

copyright */

using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using FluentValidationResult = FluentValidation.Results
    .ValidationResult;
using System.Linq.Expressions;

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
        FluentValidationResult valid1 = new IniFileAliasValidator()
            .Validate(alias);
        FluentValidationResult valid2 = new PutKeyValueValidator()
            .Validate(body);
        if (!valid1.IsValid) return Results
            .ValidationProblem(valid1.ToDictionary());
        if (!valid2.IsValid) return Results
            .ValidationProblem(valid2.ToDictionary());

        string filename = alias + ".ini";

        INIApi.Put(filename, body.Key, body.Value);
        return Results.Ok();
    }

    public IResult
    GetValue(string alias, [FromBody] GetValueRequest body)
    {
        FluentValidationResult valid1 = new IniFileAliasValidator()
            .Validate(alias);
        FluentValidationResult valid2 = new GetValueValidator()
            .Validate(body);
        if (!valid1.IsValid) return Results
            .ValidationProblem(valid1.ToDictionary());
        if (!valid2.IsValid) return Results
            .ValidationProblem(valid2.ToDictionary());

        string filename = alias + ".ini";
        
        string? value = INIApi.Get(filename, body.Key);
        if (value == null) return Results.NotFound();
        else return Results.Ok<string>(value);
    }

}

public class
PutKeyValueRequest {

    public string Key { get; set; }
    public string Value { get; set; }

}

public class
GetValueRequest {

    public string Key { get; set; }

}

public class
IniFileAliasValidator : AbstractValidator<string> {

    public
    IniFileAliasValidator()
    {
        RuleFor(s => s).NotNull();
        RuleFor(s => s).Must(s => !s.EndsWith(".ini"));
        RuleFor(s => s).Must(s =>
            INIApi.IsAllowedINIFilename(s + ".ini"));
    }

}

public class
PutKeyValueValidator : AbstractValidator<PutKeyValueRequest> {

    public
    PutKeyValueValidator()
    {
        RuleFor(r => r.Key)
            .NotEmpty()
            .Must(s => Sans(s, ' '))
            .WithMessage("Key must not have spaces.")
            .Must(s => Sans(s, '='))
            .WithMessage("Key must not have an equals sign.");
        RuleFor(r => r.Value)
            .NotNull()
            .Must(s => Sans(s, '\n'))
            .WithMessage("Key must not have newlines.");
    }

    private static bool
    Sans(string s, char c)
    {
        return s == null || !s.Contains(c);
    }

}

public class
GetValueValidator : AbstractValidator<GetValueRequest> {

    public
    GetValueValidator()
    {
        RuleFor(r => r.Key)
            .NotEmpty()
            .Must(s => Sans(s, ' '))
            .WithMessage("Key must not have spaces.")
            .Must(s => Sans(s, '='))
            .WithMessage("Key must not have an equals sign.");
    }

    private static bool
    Sans(string s, char c)
    {
        return s == null || !s.Contains(c);
    }

}