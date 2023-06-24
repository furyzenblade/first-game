using System.Collections;
using System.Collections.Generic;
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

    // Game Settings: 
    public static List<SettingSaver> AllSettings = new() { };
    public static List<SettingSaver> AdvancedSettings = new() { };
    public static List<SettingSaver> BasicSettings = new() { };

    void Start()
    {
        EnemyID = 0;
    }
}
