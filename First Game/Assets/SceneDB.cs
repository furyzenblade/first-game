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
    private const char SettingSeparator = '\u2016';
    private const char ContentSpliter = '\u2017';

    // HotKey Settings
    public static List<HotKeySetting> KeyBindings = new() { };

    // Audio Settings


    // Graphic Settings


    #endregion SettingStorage

    void Start()
    {
        EnemyID = 0;
        Application.targetFrameRate = 60;
        GetKeyBindings();
    }

    // Liest am Anfang alle HotKeySettings ein
    public static string KeyBindingsPath = "/Settings/KeyBindings";
    private static void GetKeyBindings()
    {
        if (!File.Exists(KeyBindingsPath))
            Debug.LogError("Couldn't find a HotKeySettings- File");
        else
        {
            StreamReader StreamReader = new(KeyBindingsPath);

            while(!StreamReader.EndOfStream)
            {
                AnalyseKeyBindings(GameLanguageConverter.ReadFile(KeyBindingsPath));
            }
        }
    }

    // Analysiert die Settings
    private static void AnalyseKeyBindings(string FileContent)
    {
        // File wird mit char gesplittet
        string[] SingleSettings = FileContent.Split(SettingSeparator);

        // Analyse der einzelnen Settings
        foreach (string Setting in SingleSettings)
        {
            // Aktueller Hotkey
            HotKeySetting CurrentSetting = null;

            // Variablen zum Speichern von Daten
            KeyCode Key;
            string Name;
            bool BasicSetting;
            string Description;
            List<EventModifiers> Modifiers = new() { };

            // Values werden gesplittet
            string[] Values = Setting.Split(ContentSpliter);

            Key = (KeyCode)Convert.ToInt32(Values[0]);
            Name = Values[1];
            try { BasicSetting = bool.TryParse(Values[2], out BasicSetting); } 
            catch { BasicSetting = true; }
            Description = Values[3];

            for (int i = 4; i < Values.Length; i++)
            {
                try
                {
                    Modifiers.Add((EventModifiers)Convert.ToInt32(Values[i]));
                }
                catch { Debug.LogError("Invalid Modifier"); }
            }

            // Werte vom aktuellen HotKeySetting setzen
            CurrentSetting.Key = Key;
            CurrentSetting.Name = Name;
            CurrentSetting.BasicSetting = BasicSetting;
            CurrentSetting.Description = Description;
            CurrentSetting.Modifiers = Modifiers;

            // CurrentSetting wird in die Liste eingetragen
            KeyBindings.Add(CurrentSetting);
        }
    }

    public static void SaveKeyBindings()
    {
        string FinalFileContent = "";

        // Jedes KeyBinding wird einzeln verarbeitet
        foreach (HotKeySetting KeyBinding in KeyBindings)
        {
            // Einzelne Werte werden in einen gut lesbaren string convertet und hinzugefügt
            FinalFileContent += KeyBinding.Key.GetHashCode() + ContentSpliter;
            FinalFileContent += KeyBinding.Name + ContentSpliter;
            FinalFileContent += KeyBinding.BasicSetting.ToString() + ContentSpliter;
            FinalFileContent += KeyBinding.Description + ContentSpliter;

            for (int i = 0; i < KeyBinding.Modifiers.Count; i++)
            {
                // Modifier Hash Code wird geaddet
                FinalFileContent += KeyBinding.Modifiers[i].GetHashCode();

                // Wenn nicht der letzte Modifier wird ein Separator geaddet
                if (KeyBinding.Modifiers[i] != KeyBinding.Modifiers.Last())
                    FinalFileContent += ContentSpliter;
            }

            // Wenn nicht das letzte KeyBinding wird ein Separator geaddet
            if (KeyBinding != KeyBindings.Last())
                FinalFileContent += SettingSeparator;
        }

        // Encodet die File und speichert sie dynamisch am Speicherort
        File.WriteAllBytes(CreateDynamicFilePath(KeyBindingsPath), GameLanguageConverter.Encode(FinalFileContent, KeyBindingsPath));
    }

    public static string CreateDynamicFilePath(string FileName)
    {
        return @"" + Application.dataPath + FileName + ".GData";
    }
}
