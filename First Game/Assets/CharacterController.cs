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
    public SettingSaver MoveUp;
    public SettingSaver MoveLeft;
    public SettingSaver MoveDown;
    public SettingSaver MoveRight;

    public SettingSaver DefaultDash;

    // Ability Hotkeys
    public SettingSaver Ability1;
    public SettingSaver Ability2;
    public SettingSaver Ability3;
    public SettingSaver Ability4;

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

    public SettingSaver SaveSetting(KeyCode Key, string Name, EventModifiers Modifiers = EventModifiers.None, bool BasicSetting = true, string Description = "")
    {
        // Setting wird generiert
        SettingSaver Setting = gameObject.AddComponent<SettingSaver>();

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
