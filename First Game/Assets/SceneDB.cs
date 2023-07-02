using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;

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


    // Managed die Aggro der Characters
    public static List<int> CharacterAggros = new() { };
    public static GameObject HighestAggroCharacter;

    #endregion SettingStorage

    // Default Settings werden hier manuell gespeichert
    public List<List<string>> DefaultSettings = new()
    {
        // Die einzelnen Settings als List<string>
        new() { KeyCode.W.GetHashCode().ToString(), false.ToString(), false.ToString(), false.ToString(), false.ToString(), "MoveUp", true.ToString(), ""},
        new() { KeyCode.A.GetHashCode().ToString(), false.ToString(), false.ToString(), false.ToString(), false.ToString(), "MoveLeft", true.ToString(), ""},
        new() { KeyCode.S.GetHashCode().ToString(), false.ToString(), false.ToString(), false.ToString(), false.ToString(), "MoveDown", true.ToString(), ""},
        new() { KeyCode.D.GetHashCode().ToString(), false.ToString(), false.ToString(), false.ToString(), false.ToString(), "MoveRight", true.ToString(), ""},
        new() { KeyCode.Alpha1.GetHashCode().ToString(), false.ToString(), false.ToString(), false.ToString(), false.ToString(), "Ability 1", true.ToString(), ""}
    };

    // Liste an allen Abilitys im Game
    public static List<GameObject> AllAbilitys = new() { };
    public GameObject FireBall;
    public GameObject NextAbility;

    // Setzt 1x am Anfang alle Abilitys in 1 Liste
    private void PlaceAbilitysInList()
    {
        // Jede Ability muss so einzeln eingefügt werden
        AllAbilitys.Add(FireBall);

    }

    void Start()
    {
        // Alle existierenden Abilitys werden geholt
        PlaceAbilitysInList();

        // Der erste Character wird controlled, weil bei Online Games eig. immer der lokale als erstes erscheint denke ich (philipp)
        GameObject.FindGameObjectWithTag("Character").GetComponent<CharacterController>().IsControlledChar = true;

        // EnemyID wird auf 0 gesetzt, weil noch kein Enemy gespawned wurde
        EnemyID = 0;

        // FrameRate vom Game in FPS
        Application.targetFrameRate = 60;

        // KeyBindings werden geladen
        //GetKeyBindings();

        SaveDefaultSettings();
        GetDefaultSettings();
    }

    void Update()
    {
        // Character Aggro wird ermittelt
        CharacterAggros = GetCharacterAggros();
        HighestAggroCharacter = GetHighestAggroCharacter();

        #region InputManager

        // Character wird erfasst
        foreach (GameObject Character in GameObject.FindGameObjectsWithTag("Character"))
        {
            if (Character.GetComponent<CharacterController>().IsControlledChar)
            {
                // Gibt die Direction an, in die der Character sich bewegt
                Vector3 MoveCharacterDirection = Vector3.zero;

                // Für alle validen Inputs wird eine Aktion gesucht & ausgeführt
                foreach (int number in GetValidInputIndexes())
                {
                    CharacterController CharacterController = Character.GetComponent<CharacterController>();

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
                        case 4:
                            // Holt die Ability auf einem Slot
                            GameObject Ability = AllAbilitys[CharacterController.Abilitys[0]];
                            // Wenn eine Ability gefunden wurde, wird sie ausgelöst
                            if (Ability != null)
                                CharacterController.UseAbility(0);
                            break;
                            // Weitere Settings hier hin



                    }
                    Character.GetComponent<CharacterController>().MoveCharacter(MoveCharacterDirection);
                }
            }
        }
        

        #endregion InputManagers
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

    // Ruft die Character Aggro jedes Characters ab
    private static List<int> GetCharacterAggros()
    {
        // Liste mit den Aggros von den Charactern
        List<int> Aggros = new() { };

        // Für jeden Character wird die Aggro abgerufen
        foreach (GameObject Character in GameObject.FindGameObjectsWithTag("Character"))
        {
            Aggros.Add(Character.GetComponent<CharacterController>().Aggro);
        }

        return Aggros;
    }

    // Holt den höchsten Aggro Character 
    private static GameObject GetHighestAggroCharacter()
    {
        // Liste mit den Aggros von den Charactern & Charactern an sich
        List<int> Aggros = new() { };
        List<GameObject> Characters = new() { };

        // Für jeden Character wird die Aggro abgerufen
        foreach (GameObject Character in GameObject.FindGameObjectsWithTag("Character"))
        {
            Aggros.Add(Character.GetComponent<CharacterController>().Aggro);
            Characters.Add(Character);
        }

        // Der Character mit der highest Aggro wird zurück gegeben
        return Characters[Aggros.IndexOf(Aggros.Max())];
    }

    public static GameObject GetAbilityOnSlot(int SlotIndex)
    {
        List<GameObject> Abilitys = GameObject.FindGameObjectsWithTag("Ability").ToList();

        foreach (GameObject Ability in Abilitys)
        {
            if (Ability.GetComponent<AbilityManager>().Slot == SlotIndex)
                return Ability;
        }

        return null;
    }
}
