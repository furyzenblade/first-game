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

    // Liest am Anfang alle Settings ein
    public static Settings Settings { get; set; }
    private static void GetSettings()
    {
        Settings = new Settings();
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
