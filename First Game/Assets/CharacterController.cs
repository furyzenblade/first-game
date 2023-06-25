using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using Unity.VisualScripting;

public class CharacterController : MonoBehaviour
{
    // Speichert Settings 
    #region Settings

    // Movement Settings
    public HotKeySetting MoveUp;
    public HotKeySetting MoveLeft;
    public HotKeySetting MoveDown;
    public HotKeySetting MoveRight;

    public HotKeySetting DefaultDash;

    // Ability Hotkeys
    public HotKeySetting Ability1;
    public HotKeySetting Ability2;
    public HotKeySetting Ability3;
    public HotKeySetting Ability4;

    // Sonstige Hotkeys



    #endregion Settings

    void Update()
    {
        MoveUp = SaveSetting(KeyCode.W, "MoveUp");
        MoveLeft = SaveSetting(KeyCode.A, "MoveLeft");
        MoveDown = SaveSetting(KeyCode.S, "MoveDown");
        MoveRight = SaveSetting(KeyCode.D, "MoveRight");

        DefaultDash = SaveSetting(KeyCode.Space, "DefaultDash");

        Ability1 = SaveSetting(KeyCode.Alpha1, "Ability 1");
        Ability2 = SaveSetting(KeyCode.Alpha2, "Ability 2");
        Ability3 = SaveSetting(KeyCode.Alpha3, "Ability 3");
        Ability4 = SaveSetting(KeyCode.Alpha4, "Ability 4");
    }

    public HotKeySetting SaveSetting(KeyCode Key, string Name, List<EventModifiers> Modifiers = null, bool BasicSetting = true, string Description = "")
    {
        // Setting wird generiert
        HotKeySetting Setting = gameObject.AddComponent<HotKeySetting>();

        // Speichert die gegebenen Werte
        Setting.Key = Key;
        Setting.Name = Name;
        Setting.Modifiers = Modifiers;
        Setting.BasicSetting = BasicSetting;
        Setting.Description = Description;

        // Setting zur Liste hinzufügen
        SceneDB.AllSettings.Add(Setting);
        if (BasicSetting)
            SceneDB.BasicSettings.Add(Setting);
        else
            SceneDB.AdvancedSettings.Add(Setting);

        return Setting;
    }

}
