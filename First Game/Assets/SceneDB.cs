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

    // Default Settings werden hier manuell gespeichert
    public List<List<string>> DefaultSettings = new()
    {
        // Die einzelnen Settings als List<string>
        new() { KeyCode.W.GetHashCode().ToString(), false.ToString(), false.ToString(), false.ToString(), false.ToString(), "MoveUp", true.ToString(), ""},
        new() { KeyCode.A.GetHashCode().ToString(), false.ToString(), false.ToString(), false.ToString(), false.ToString(), "MoveLeft", true.ToString(), ""},
        new() { KeyCode.S.GetHashCode().ToString(), false.ToString(), false.ToString(), false.ToString(), false.ToString(), "MoveDown", true.ToString(), ""},
        new() { KeyCode.D.GetHashCode().ToString(), false.ToString(), false.ToString(), false.ToString(), false.ToString(), "MoveRight", true.ToString(), ""}
    };

    void Start()
    {
        GameObject.FindGameObjectWithTag("Character").GetComponent<CharacterController>().IsControlledChar = true;

        EnemyID = 0;
        Application.targetFrameRate = 60;
        //GetKeyBindings();

        SaveDefaultSettings();
        GetDefaultSettings();
    }

    void Update()
    {
        // Gibt die Direction an, in die der Character sich bewegt
        Vector3 MoveCharacterDirection = Vector3.zero;

        // Für alle validen Inputs wird eine Aktion gesucht & ausgeführt
        foreach (int number in GetValidInputIndexes())
        {
            // switch() case verarbeitet die Input Numbers
            switch (number)
            {
                case 0:
                    MoveCharacterDirection += Vector3.up;
                    break;
                case 1:
                    MoveCharacterDirection += Vector3.left;
                    break;
                case 2:
                    MoveCharacterDirection += Vector3.down;
                    break;
                case 3:
                    MoveCharacterDirection += Vector3.right;
                    break;
                // Weitere Settings hier hin



            }
        }

        // Character wird erfasst
        foreach (GameObject Character in GameObject.FindGameObjectsWithTag("Character"))
        {
            if (Character.GetComponent<CharacterController>().IsControlledChar)
                Character.GetComponent<CharacterController>().MoveCharacter(MoveCharacterDirection);
        }
    }

    // Liest am Anfang alle HotKeySettings ein
    public static string KeyBindingsPath = "/Settings/KeyBindings";
    public static string DefaultKeyBindingsPath = "/Settings/DefaultKeyBindings";
    private static void GetKeyBindings()
    {
        if (!File.Exists(CreateDynamicFilePath(KeyBindingsPath)))
            Debug.LogError("Couldn't find a HotKeySettings- File");
        else
        {
            KeyBindings = HotKeySetting.AnalyseKeyBindings(GameLanguageConverter.StrDecode(KeyBindingsPath));
        }
    }
    private void SaveDefaultSettings()
    {
        // string für die unverschlüsselte, vollständige File wird vorbereitet
        string AllSettings = "";

        foreach (List<string> SingleSetting in DefaultSettings)
        {
            foreach (string Data in SingleSetting)
            {
                AllSettings += Data;
                // Wenn letzte Data wird nicht separiert
                if (Data != SingleSetting.Last())
                    AllSettings += ContentSpliter;
            }
            // Wenn letztes Setting wird nicht separiert
            if (SingleSetting != DefaultSettings.Last())
                AllSettings += SettingSeparator;
        }

        File.WriteAllBytes(CreateDynamicFilePath(DefaultKeyBindingsPath), GameLanguageConverter.Encode(DefaultKeyBindingsPath, AllSettings, true));
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

    private void GetDefaultSettings()
    {
        if (!File.Exists(CreateDynamicFilePath(DefaultKeyBindingsPath)))
            Debug.LogError("Couldn't find a DefaultHotKeySettings- File");
        else
        {
            KeyBindings = HotKeySetting.AnalyseKeyBindings(GameLanguageConverter.StrDecode(DefaultKeyBindingsPath));
            KeyBindings = HotKeySetting.AnalyseKeyBindings(GameLanguageConverter.StrDecode(DefaultKeyBindingsPath));
        }
    }
}
