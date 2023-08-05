using System.Collections.Generic;
using System.Linq;

public class Setting
{
    // Generelle Settings
    public string Name;
    public string Description = "";

    // Design Settings
    public bool SplitUI = false;
    public bool IsMainSetting = true;

    // Convertet den Setting zu einer List<string> zum späteren Abspeichern
    public List<string> SaveSetting()
    {
        return new List<string>() { Name, Description, SplitUI.ToString(), IsMainSetting.ToString() };
    }

    public List<string> LoadSetting(string Content)
    {
        List<string> Data = Content.Split(Settings.InformationSeparator).ToList();

        Name = Data[0];
        Description = Data[1];
        SplitUI = bool.Parse(Data[2]);
        IsMainSetting = bool.Parse(Data[3]);

        Data.RemoveRange(0, 4);

        return Data;
    }
}