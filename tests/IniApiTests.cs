
using NUnit.Framework;

[TestFixture]
class
IniApiTests {

    /*
    * IniApi at present has barely any defences against malicious
    * input at all, it basically trusts the caller. Therefore we
    * won't do any aggressive moves in these tests, simply basic
    * INI file I/O behaviour.
    */

    [Test]
    public void
    GetFromNonExistent()
    {
        Assert.That(!File.Exists("new.ini"));
        
        Assert.Throws<FileNotFoundException>(delegate {
            string? v1 = IniApi.Get("new.ini", "blue");
        });
    }

    [Test]
    public void
    PutIntoNonExistent()
    {
        Assert.That(!File.Exists("new.ini"));
        
        string d = "Something like, the colour of the sea, maybe.";
        IniApi.Put("new.ini", "blue", d);
        Assert.That(File.Exists("new.ini"));
        string? v1 = IniApi.Get("new.ini", "blue");
        Assert.That(d.Equals(v1));
        File.Delete("new.ini");
    }
    
    [Test]
    public void
    FirstWriteIniFormat()
    {
        IniApi.Put(
            "ProgramTesting.ini", "black",
            "Utterly devoid of colour and completely dark.");
        IniApi.Put(
            "ProgramTesting.ini", "pink",
            "A red-purple that's immensely intensely bright.");
        IniApi.Put(
            "ProgramTesting.ini", "yellows.gold",
            "A mildly desaturated intense yellow.");
        IniApi.Put(
            "ProgramTesting.ini", "yellows.old.clay",
            "A rather dull and unassuming brown.");
        IniApi.Put(
            "ProgramTesting.ini", "lime",
            "A sort of green that's extremely bright.");

        string output = File.ReadAllText("ProgramTesting.ini");
        Assert.That(output.Equals(
           "black = Utterly devoid of colour and completely dark."
           + "\r\nlime = A sort of green that's extremely bright."
           + "\r\npink = A red-purple that's immensely intensely bright."
           + "\r\n"
           + "\r\n[yellows]"
           + "\r\ngold = A mildly desaturated intense yellow."
           + "\r\n"
           + "\r\n[yellows.old]"
           + "\r\nclay = A rather dull and unassuming brown."
           + "\r\n"));
    }

    public void
    CaseSensitiveGet()
    {
        string? v1 = IniApi.Get("ProgramTesting.ini", "Lime");
        Assert.That(v1 == null);
        string? v2 = IniApi.Get("ProgramTesting.ini", "lime");
        Assert.That(v2 != null);
    }

    [Test]
    public void
    PutAndGet()
    {
        string? v1 = IniApi.Get("ProgramTesting.ini", "beige");
        Assert.That(v1 == null);

        IniApi.Put("ProgramTesting.ini", "beige", "A light brown.");
        string? v2 = IniApi.Get("ProgramTesting.ini", "beige");
        Assert.That(v2 != null);

        IniApi.Put(
            "ProgramTesting.ini",
            "greens.pandan", "A vivid lighter green.");
        string? v3 = IniApi.Get("ProgramTesting.ini", "greens.pandan");
        Assert.That(v3 != null);
        Assert.That("A vivid lighter green.".Equals(v3));
    }

//  ---%-@-%---

    [SetUp]
    public void
    SetUp()
    {
        File.Create("ProgramTesting.ini").Close();
    }

    [TearDown]
    public void
    TearDown()
    {
        File.Delete("ProgramTesting.ini");
    }

}