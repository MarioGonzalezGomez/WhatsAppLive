using System;
using System.IO;
using IniParser;
using IniParser.Model;

namespace WhatsAppLive;
public static class ConfigManager
{
    private static readonly string configPath = "config.ini";

    public static string Device_OFF
    {
        get; private set;
    }
    public static string Device_LIVE
    {
        get; private set;
    }

    static ConfigManager()
    {
        LoadConfig();
    }

    public static void LoadConfig()
    {
        if (!File.Exists(configPath))
        {
            CreateDefaultConfig();
        }

        var parser = new FileIniDataParser();
        IniData data = parser.ReadFile(configPath);

        Device_OFF = data["Settings"]["Device_OFF"];
        Device_LIVE = data["Settings"]["Device_LIVE"];
    }

    private static void CreateDefaultConfig()
    {
        var parser = new FileIniDataParser();
        var data = new IniData();

        data["Settings"]["Device_OFF"] = "Altavoces (High Definition Audio Device)";
        data["Settings"]["Device_LIVE"] = "Altavoces (Blackmagic DeckLink Duo 2 (1) Audio)";

        parser.WriteFile(configPath, data);
    }
}
