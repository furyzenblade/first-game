using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

[SerializeField]
public class Settings
{
    public Settings(bool LoadSettings = true)
    {
        // Combines the File Path and correct Unity's Errors by hand
        FullPath = Path.Combine(Application.streamingAssetsPath, path);
        FullPath = FullPath.Split("\\")[0] + "/" + FullPath.Split("\\")[1];

        if (LoadSettings)
            DeSerialise();
    }

    // chars, die Settings separieren und nicht vom GameLanguageConverter erschaffen werden können
    public const char SettingSeparator = '\u2016';
    public const char InformationSeparator = '\u2017';

    // Settings werden hier gespeichert
    public List<KeyBinding> KeyBindings = new() { };

    // Property, was DefaultSettings zurück gibt
    public static readonly Settings DefaultSettings = new(false)
    {
        KeyBindings = new List<KeyBinding>
        {
            new KeyBinding("MoveUp", "", false, true, KeyCode.W, false, false, false, false, true),
            new KeyBinding("MoveLeft", "", false, true, KeyCode.A, false, false, false, false, true),
            new KeyBinding("MoveDown", "", false, true, KeyCode.S, false, false, false, false, true),
            new KeyBinding("MoveRight", "", false, true, KeyCode.D, false, false, false, false, true),
            new KeyBinding("BasicAttack", "", false, true, KeyCode.Mouse0, false, false, false, false, true),
            new KeyBinding("Base Ability", "", false, true, KeyCode.Mouse1, false, false, false, false, true),
            new KeyBinding("Ability 1", "", false, true, KeyCode.Alpha1, false, false, false, false, true),
            new KeyBinding("Ability 2", "", false, true, KeyCode.Alpha2, false, false, false, false, true),
            new KeyBinding("Ability 3", "", false, true, KeyCode.Alpha3, false, false, false, false, true),
            new KeyBinding("Ability 4", "", false, true, KeyCode.Alpha4, false, false, false, false, true),
            new KeyBinding("Ability 5", "", false, true, KeyCode.Alpha5, false, false, false, false, true)
        }


        
    };

    public void Serialise()
    {
        string strSettings = "";

        foreach (KeyBinding KeyBinding in KeyBindings)
        {
            // Settings separieren sich und werden in eine Liste eingetragen
            List<string> Data = KeyBinding.Serialise();
            for (int i = 0; i < Data.Count; i++)
            {
                // Daten werden an strSettings angehangen
                strSettings += Data[i];

                // Wenn nicht das letzte Datenpacket wird separiert
                if (i != Data.Count - 1)
                    strSettings += InformationSeparator;
            }

            // Wenn nicht das letzte Setting werden sie separiert
            if (KeyBinding != KeyBindings.Last())
                strSettings += SettingSeparator;
        }

        GameLanguageConverter.Encode(FullPath, strSettings, true);
    }

    // Settings werden hier gespeichert, Default Settings sind im Code
    public const string path = "GData/Settings.GData";
    public string FullPath;



    #region LoadFile

    private IEnumerator LoadFile()
    {
        using UnityWebRequest request = UnityWebRequest.Get(FullPath);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to load settings: " + request.error);
            yield break;
        }

        UnencodedSettings = request.downloadHandler.data;
    }

    private byte[] UnencodedSettings;

    #endregion LoadFile

    private void DeSerialise()
    {
        try
        {
            // Lädt die Settings File aus dem Resources Folder
            //LoadFile();
            UnencodedSettings = File.ReadAllBytes(FullPath);


            string Content = GameLanguageConverter.StrDecode(FullPath, UnencodedSettings);

            List<string> Settings = Content.Split(SettingSeparator).ToList();

            for (int i = 0; i < DefaultSettings.KeyBindings.Count; i++)
            {
                KeyBindings.Add(new KeyBinding(Settings[i]));
            }
            // Wenn weitere Settings- Arten kommen, hier hinzufügen
        }
        catch
        {
            Debug.Log("Loading Default Settings");
            LoadDefaultSettings();
            Serialise();
        }
    }

    // Default Settings (teilweise) laden
    public void LoadDefaultSettings()
    {
        KeyBindings = DefaultSettings.KeyBindings;
    }
    public void LoadDefaultKeyBindings() { KeyBindings = DefaultSettings.KeyBindings; }

}