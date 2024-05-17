
using NUnit.Framework;
using FluentValidation.TestHelper;

[TestFixture]
class
FluentValidatorTests {

    [Test]
    public void
    IniFilenameRatherThanAlias()
    {
        IniFileAliasValidator valid = new();

        valid.TestValidate("colours.ini")
            .ShouldHaveValidationErrorFor(s => s);
            
        valid.TestValidate(".ini")
            .ShouldHaveValidationErrorFor(s => s);
        
        valid.TestValidate("colours")
            .ShouldNotHaveValidationErrorFor(s => s);
    }

    [Test]
    public void
    DisallowedIniFileAlias()
    {
        IniFileAliasValidator valid = new();

        valid.TestValidate("colours")
            .ShouldNotHaveValidationErrorFor(s => s);

        valid.TestValidate("FluentValidatorTests")
            .ShouldHaveValidationErrorFor(s => s);

        valid.TestValidate("\bnon-existent\b")
            .ShouldHaveValidationErrorFor(s => s);
    }

    [Test]
    public void
    PutEmptyKeyOrValue()
    {
        PutKeyValueValidator valid = new();

        var r1 = valid.TestValidate(
            new PutKeyValueRequest() { Key = "", Value = "" });
        r1.ShouldHaveValidationErrorFor(r => r.Key);
        r1.ShouldNotHaveValidationErrorFor(r => r.Value);

        valid.TestValidate(
            new PutKeyValueRequest() { Key = "Moon", Value = "" })
            .ShouldNotHaveAnyValidationErrors();

        valid.TestValidate(
            new PutKeyValueRequest() { Key = "", Value = "Dark." })
            .ShouldHaveValidationErrorFor(r => r.Key)
            .Only();
    }

    [Test]
    public void
    PutKeyWithSpacesOrEquals()
    {
        PutKeyValueValidator valid = new();

        valid.TestValidate(
            new PutKeyValueRequest() {
                Key = "dark grey",
                Value = "(Description)" })
            .ShouldHaveValidationErrorFor(r => r.Key)
            .Only();

        valid.TestValidate(
            new PutKeyValueRequest() {
                Key = "attempted = injection",
                Value = "(Description)" })
            .ShouldHaveValidationErrorFor(r => r.Key)
            .Only();
    }

    [Test]
    public void
    PutValueWithNewlines()
    {
        PutKeyValueValidator valid = new();

        valid.TestValidate(
            new PutKeyValueRequest() {
                Key = "OkayKey",
                Value = "A description\nwith newlines\nin it." })
            .ShouldHaveValidationErrorFor(r => r.Value);
    }

    [Test]
    public void
    PutValidKeyAndValue()
    {
        
    }

    [Test]
    public void
    GetEmptyKey()
    {
        GetValueValidator valid = new();

        valid.TestValidate(
            new GetValueRequest() { Key = "" })
            .ShouldHaveValidationErrorFor(r => r.Key);
    }

    [Test]
    public void
    GetKeyWithSpacesOrEquals()
    {
        GetValueValidator valid = new();

        valid.TestValidate(
            new GetValueRequest() { Key = "vivid reds" })
            .ShouldHaveValidationErrorFor(r => r.Key);
        
        valid.TestValidate(
            new GetValueRequest() { Key = "malicious=key" })
            .ShouldHaveValidationErrorFor(r => r.Key);
    }

    [Test]
    public void
    GetValidKey()
    {

    }

}