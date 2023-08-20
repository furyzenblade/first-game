using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

// Eine globale, überwiegend statische Klasse, die z.B. User Inputs verwaltet, um Performance zu sparen
// Oder das übergibt neue IDs etc., ohne mit anderen Klassen oder GameObjects komunizieren zu müssen
// Speichert / berechnet auch z.B. globale Werte wie den highest aggro character oder so
public class SceneDB : MonoBehaviour
{
    // Gibt eine globale, individuelle Enemy ID an
    public static int EntityID { get; private set; }
    // Generiert die nächste EnemyID & gibt diese zurück
    public static int AddEntityID()
    {
        EntityID++;
        return EntityID - 1;
    }

    // Managed die Aggro der Characters
    public static List<int> CharacterAggros = new() { };
    public static GameObject HighestAggroCharacter;

    // Liste an allen Abilitys im Game
    public static List<GameObject> AllAbilitys = new() { };

    // Aktuell kontrollierter Character
    public static GameObject ControlledCharacter;

    // Bestimmt, ob der Befehl für einen BasicAttack gegeben wurde
    public static bool CharacterIsAttacking;

    // Lädt das Game mit Settings etc.
    void Start()
    {
        // FrameRate vom Game in FPS
        Application.targetFrameRate = 60;

        // Lädt die Settings
        try { GetSettings(); }
        catch { Debug.LogError("Unable to load settings"); }
        // Load all prefabs
        string pathToPrefabs = "Abilitys"; // Relative path from "Resources" folder

        GameObject[] allPrefabs = Resources.LoadAll<GameObject>(pathToPrefabs);

        foreach (GameObject prefab in allPrefabs)
        {
            if (prefab.CompareTag("Ability"))
            {
                AllAbilitys.Add(prefab);
            }
        }

        // Der erste Character wird controlled, weil bei Online Games eig. immer der lokale als erstes erscheint denke ich (philipp)
        //GameObject.FindGameObjectWithTag("Ally").GetComponent<CharacterController>().IsControlledChar = true;

        // EnemyID wird auf 0 gesetzt, weil noch kein Enemy gespawned wurde
        EntityID = 0;
    }

    // Updatet globale Werte wie Aggro etc. framewise, damit nicht mehrere GameObjects dies machen müssen
    void Update()
    {
        // Aktuell kontrollierter Character wird ermittelt
        foreach (GameObject Character in GameObject.FindGameObjectsWithTag("Ally"))
            if (Character.GetComponent<CharacterController>().IsControlledChar)
                ControlledCharacter = Character;

        // Character Aggro wird ermittelt
        CharacterAggros = GetCharacterAggros();
        HighestAggroCharacter = GetHighestAggroCharacter();

        #region InputManager

        // Character wird erfasst
        foreach (GameObject Character in GameObject.FindGameObjectsWithTag("Ally"))
        {
            if (Character.GetComponent<CharacterController>().IsControlledChar)
            {
                // Gibt die Direction an, in die der Character sich bewegt
                Vector3 MoveCharacterDirection = Vector3.zero;

                CharacterController CharacterController = Character.GetComponent<CharacterController>();

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
                        case 4:
                            GameObject HitObject = GetHoveredObject();
                            if (HitObject != null)
                            {
                                ControlledCharacter.GetComponent<CharacterController>().UseBasicAttack(HitObject);
                            }
                            break;
                        case 5:
                            // Holt die Ability auf einem Slot
                            GameObject Ability = CharacterController.Abilitys[0];
                            // Wenn eine Ability gefunden wurde, wird sie ausgelöst
                            if (Ability != null)
                                CharacterController.UseAbility(0);
                            break;
                        case 6:
                            // Holt die Ability auf einem Slot
                            Ability = CharacterController.Abilitys[1];
                            // Wenn eine Ability gefunden wurde, wird sie ausgelöst
                            if (Ability != null)
                                CharacterController.UseAbility(1);
                            break;
                        case 7:
                            // Holt die Ability auf einem Slot
                            Ability = CharacterController.Abilitys[2];
                            // Wenn eine Ability gefunden wurde, wird sie ausgelöst
                            if (Ability != null)
                                CharacterController.UseAbility(2);
                            break;
                        case 8:
                            // Holt die Ability auf einem Slot
                            Ability = CharacterController.Abilitys[3];
                            // Wenn eine Ability gefunden wurde, wird sie ausgelöst
                            if (Ability != null)
                                CharacterController.UseAbility(3);
                            break;
                        case 9:
                            // Holt die Ability auf einem Slot
                            Ability = CharacterController.Abilitys[4];
                            // Wenn eine Ability gefunden wurde, wird sie ausgelöst
                            if (Ability != null)
                                CharacterController.UseAbility(4);
                            break;
                            // Weitere Settings hier hin

                    }
                }
                // Wenn sich der Character bewegt ...
                if (MoveCharacterDirection != Vector3.zero)
                {
                    // ... Bewegt er sich
                    CharacterController.MoveCharacter(MoveCharacterDirection);

                    // ... Löscht er alle BasicAttacks
                    try { Destroy(CharacterController.gameObject.GetComponent<BasicAttack>()); } catch { }
                }
            }
        }


        #endregion InputManagers
    }

    // Liest am Anfang alle Settings ein
    public static Settings Settings { get; set; }
    private static void GetSettings()
    {
        Settings = new Settings();
    }

    private List<int> GetValidInputIndexes()
    {
        List<int> Indexes = new() { };

        for (int i = 0; i < Settings.KeyBindings.Count; i++)
        {
            if (Settings.KeyBindings[i].InputIsValid())
                Indexes.Add(i);
        }

        return Indexes;
    }

    // Ruft die Character Aggro jedes Characters ab
    private static List<int> GetCharacterAggros()
    {
        // Liste mit den Aggros von den Charactern
        List<int> Aggros = new() { };

        // Für jeden Character wird die Aggro abgerufenF
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
        foreach (GameObject Character in GameObject.FindGameObjectsWithTag("Ally"))
        {
            Aggros.Add(Character.GetComponent<CharacterController>().Aggro);
            Characters.Add(Character);
        }

        // Der Character mit der highest Aggro wird zurück gegeben
        return Characters[Aggros.IndexOf(GetMaxValue(Aggros))];
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

    public static string CreateDynamicFilePath(string FileName)
    {
        return @"" + Application.dataPath + "/" + FileName + GameLanguageConverter.FileEnding;
    }

    public static int GetMaxValue(List<int> List)
    {
        List.Sort();
        return List[^1];
    }
}
