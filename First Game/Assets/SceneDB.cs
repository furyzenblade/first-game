using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

// Eine globale, �berwiegend statische Klasse, die z.B. User Inputs verwaltet, um Performance zu sparen
// Oder das �bergibt neue IDs etc., ohne mit anderen Klassen oder GameObjects komunizieren zu m�ssen
// Speichert / berechnet auch z.B. globale Werte wie den highest aggro character oder so
public class SceneDB : MonoBehaviour
{
    // Chars zum Spliten von Informationen aus Files werden gegeben
    public const char ObjectSeparator = '\u2016';
    public const char ContentSpliter = '\u2017';

    // Gibt eine globale, individuelle Enemy ID an
    public static int EnemyID { get; private set; }
    // Generiert die n�chste EnemyID & gibt diese zur�ck
    public static int AddEnemyID()
    {
        EnemyID++;
        return EnemyID - 1;
    }

    // Speichert alle Settings nach Kategorie
    #region SettingStorage

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
        new() { KeyCode.Mouse1.GetHashCode().ToString(), false.ToString(), false.ToString(), false.ToString(), false.ToString(), "BasicAttack", true.ToString(), "" },
        new() { KeyCode.Alpha1.GetHashCode().ToString(), false.ToString(), false.ToString(), false.ToString(), false.ToString(), "Ability 1", true.ToString(), ""},
        new() { KeyCode.Alpha2.GetHashCode().ToString(), false.ToString(), false.ToString(), false.ToString(), false.ToString(), "Ability 2", true.ToString(), ""}
    };

    // Liste an allen Abilitys im Game (Muss denke ich (philipp) nochmal reworked werden & generischer programmiert werden) 
    public static List<GameObject> AllAbilitys = new() { };

    // Aktuell kontrollierter Character
    public static GameObject ControlledCharacter;

    // Bestimmt, ob der Befehl f�r einen BasicAttack gegeben wurde
    public static bool CharacterIsAttacking;

    // L�dt das Game mit Settings etc.
    void Start()
    {
        // Find all prefab GUIDs in the Assets folder
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");

        foreach (string guid in prefabGuids)
        {
            // Get the file path of the prefab using the GUID
            string prefabPath = AssetDatabase.GUIDToAssetPath(guid);

            // Load the prefab at the path
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            // Do something with the prefab
            if (prefab != null && prefab.CompareTag("Ability"))
            {
                AllAbilitys.Add(prefab);
            }
        }

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

    // Updatet globale Werte wie Aggro etc. framewise, damit nicht mehrere GameObjects dies machen m�ssen
    void Update()
    {
        // Aktuell kontrollierter Character wird ermittelt
        foreach (GameObject Character in GameObject.FindGameObjectsWithTag("Character"))
            if (Character.GetComponent<CharacterController>().IsControlledChar)
                ControlledCharacter = Character;

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

                // F�r alle validen Inputs wird eine Aktion gesucht & ausgef�hrt
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
                            GameObject HitObject = GetHoveredObject();
                            if (HitObject != null)
                            {
                                ControlledCharacter.GetComponent<CharacterController>().UseBasicAttack(HitObject);
                            }
                            break;
                        case 5:
                            // Holt die Ability auf einem Slot
                            GameObject Ability = AllAbilitys[CharacterController.Abilitys[0]];
                            // Wenn eine Ability gefunden wurde, wird sie ausgel�st
                            if (Ability != null)
                                CharacterController.UseAbility(0);
                            break;
                        case 6:
                            // Holt die Ability auf einem Slot
                            Ability = AllAbilitys[CharacterController.Abilitys[1]];
                            // Wenn eine Ability gefunden wurde, wird sie ausgel�st
                            if (Ability != null)
                                CharacterController.UseAbility(1);
                            break;
                        // Weitere Settings hier hin

                    }
                }
                Character.GetComponent<CharacterController>().MoveCharacter(MoveCharacterDirection);
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
        // string f�r die unverschl�sselte, vollst�ndige File wird vorbereitet
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
                AllSettings += ObjectSeparator;
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
            KeyBindings = HotKeySetting.AnalyseKeyBindings(GameLanguageConverter.StrDecode(DefaultKeyBindingsPath));
    }

    // Ruft die Character Aggro jedes Characters ab
    private static List<int> GetCharacterAggros()
    {
        // Liste mit den Aggros von den Charactern
        List<int> Aggros = new() { };

        // F�r jeden Character wird die Aggro abgerufen
        foreach (GameObject Character in GameObject.FindGameObjectsWithTag("Character"))
        {
            Aggros.Add(Character.GetComponent<CharacterController>().Aggro);
        }

        return Aggros;
    }

    // Holt den h�chsten Aggro Character 
    private static GameObject GetHighestAggroCharacter()
    {
        // Liste mit den Aggros von den Charactern & Charactern an sich
        List<int> Aggros = new() { };
        List<GameObject> Characters = new() { };

        // F�r jeden Character wird die Aggro abgerufen
        foreach (GameObject Character in GameObject.FindGameObjectsWithTag("Character"))
        {
            Aggros.Add(Character.GetComponent<CharacterController>().Aggro);
            Characters.Add(Character);
        }

        // Der Character mit der highest Aggro wird zur�ck gegeben
        return Characters[Aggros.IndexOf(Aggros.Max())];
    }

    public GameObject GetHoveredObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
        try
        {
            if (hit.collider.gameObject != null)
            {
                return hit.collider.gameObject;
            }
        }
        catch { Debug.Log("Hit GameObject has no Collider"); }

        return null;
    }

    public static GameObject GetAbilityOnSlot(int SlotIndex)
    {
        List<GameObject> Abilitys = GameObject.FindGameObjectsWithTag("Ability").ToList();

        foreach (GameObject Ability in Abilitys)
        {
            if (Ability.GetComponent<Ability>().Slot == SlotIndex)
                return Ability;
        }

        return null;
    }


    // Vergleicht zwei Components miteinander (ChatGPT Code)
    public static bool CompareComponents<T>(T component1, T component2)
    {
        string json1 = JsonUtility.ToJson(component1);
        string json2 = JsonUtility.ToJson(component2);

        return json1 == json2;
    }
}
