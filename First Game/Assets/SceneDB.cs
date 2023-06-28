using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

    // Speichert alle Settings nach Kategorie
    #region SettingStorage

    // Chars zum Spliten von Informationen werden gegeben
    public const char SettingSeparator = '\u2016';
    public const char ContentSpliter = '\u2017';

    // HotKey Settings
    public static List<HotKeySetting> KeyBindings = new() { };

    // Audio Settings


    // Graphic Settings


    #endregion SettingStorage

    // Default Settings werden hier manuell gesetzt
    #region DefaultSettings



    #endregion DefaultSettings

    void Start()
    {
        EnemyID = 0;
        Application.targetFrameRate = 60;
        GetKeyBindings();
    }

    void Update()
    {
        // Für alle validen Inputs wird eine Aktion gesucht & ausgeführt
        foreach (int number in GetValidInputIndexes())
        {
            // Wenn Setting 1 zutrifft, dann ....
            if (number == 1)
            {

            }
            // Setting 2+ hier hin
        }
    }

    // Liest am Anfang alle HotKeySettings ein
    public static string KeyBindingsPath = "/Settings/KeyBindings";
    private static void GetKeyBindings()
    {
        if (!File.Exists(KeyBindingsPath))
            Debug.LogError("Couldn't find a HotKeySettings- File");
        else
        {
            KeyBindings = HotKeySetting.AnalyseKeyBindings(GameLanguageConverter.ReadFile(KeyBindingsPath));
        }
    }

    public static string CreateDynamicFilePath(string FileName)
    {
        return @"" + Application.dataPath + FileName + ".GData";
    }

    private List<int> GetValidInputIndexes()
    {
        List<int> Indexes = new() { };

        for (int i = 0; i < KeyBindings.Count; i++)
        {
            if (KeyBindings[i].InputIsValid())
                Indexes.Add(i);
        }

        return Indexes;
    }
}
