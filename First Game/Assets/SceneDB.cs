using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class SceneDB : MonoBehaviour
{
    // Gibt eine globale, individuelle Enemy ID an
    public static int EnemyID { get; private set; }
    public static int AddEnemyID()
    {
        EnemyID++;
        return EnemyID - 1;
    }

    // Gibt eine Setting-ID an
    public static int SettingID { get; private set; }
    public static int AddSettingID()
    {
        SettingID++;
        return SettingID - 1;
    }

    // Speichert alle Settings nach Kategorie
    #region SettingStorage

    // HotKey Settings
    public static List<HotKeySetting> AllSettings = new() { };
    public static List<HotKeySetting> AdvancedSettings = new() { };
    public static List<HotKeySetting> BasicSettings = new() { };

    // Audio Settings


    // Graphic Settings


    #endregion SettingStorage

    void Start()
    {
        EnemyID = 0;
        Application.targetFrameRate = 60;

    }

    // Liest am Anfang alle HotKeySettings ein
    public static string HotKeySettingsPath = @""; 
    private static void GetHotKeySettings()
    {
        if (!File.Exists(HotKeySettingsPath))
            Debug.LogError("Couldn't find a HotKeySettings- File");
        else
        {
            StreamReader StreamReader = new(HotKeySettingsPath);

            while(!StreamReader.EndOfStream)
            {
                HotKeySetting CurrentSetting = AnalyseHotKeySetting(StreamReader.ReadLine());
            }
        }
    }

    // Analysiert einen Teil der 
    private static HotKeySetting AnalyseHotKeySetting(string FileContent)
    {
        HotKeySetting CurrentSetting = null;

        // Variablen zur zukünftigen Zuweisung deklarieren
        KeyCode Key;
        string Name;
        List<EventModifiers> Modifiers = null;
        bool BasicSetting = true;
        string Description = "";



        return CurrentSetting;
    }
}
