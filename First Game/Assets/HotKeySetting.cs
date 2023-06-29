using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using UnityEditor.PackageManager;

// Erstellt für das Setting UI einen Namen
// Optional: Name & ob es ein Base Setting ist
public class HotKeySetting : MonoBehaviour
{
    #region HotKeySettings

    // Speichert den Key für den Input
    public KeyCode Key;

    // Speichern Values, ob die jeweiligen Modifier an oder aus sind
    public bool Modifier_Alt = false;
    public bool Modifier_CapsLock = false;
    public bool Modifier_Control = false;
    public bool Modifier_Shift = false;

    // Bestimmt, ob für einen Key alle Modifier exakt stimmen müssen oder ob es egal ist
    public bool IsMainKey = false;

    #endregion HotKeySettings

    #region UISettings

    // Gibt an, ob das Setting zu Basic oder Advanced Settings gehört
    public bool IsBasicSetting;

    // Variablen zur eindeutigen Identifizierung eines Settings
    public string Name;
    public string Description;

    #endregion UISettings

    public static List<HotKeySetting> AnalyseKeyBindings(string FileContent)
    {
        // File wird mit char gesplittet
        string[] SingleSettings = FileContent.Split(SceneDB.SettingSeparator);

        // Liste mit HotKeySettings
        List<HotKeySetting> HotKeySettings = new() { };

        // Analyse der einzelnen Settings
        foreach (string Setting in SingleSettings)
        {
            // einzelnes HotKeySetting wird erstellt
            HotKeySetting CurrentSetting = GameObject.FindGameObjectWithTag("KeyBindings").AddComponent<HotKeySetting>();

            // Values werden gesplittet
            string[] Values = Setting.Split(SceneDB.ContentSpliter);

            // Werte von Values ins aktuelle Setting eintragen
            CurrentSetting.Key = (KeyCode)Convert.ToInt32(Values[0]);
            CurrentSetting.Modifier_Alt = Convert.ToBoolean(Values[1]);
            CurrentSetting.Modifier_CapsLock = Convert.ToBoolean(Values[2]);
            CurrentSetting.Modifier_Control = Convert.ToBoolean(Values[3]);
            CurrentSetting.Modifier_Shift = Convert.ToBoolean(Values[4]);
            CurrentSetting.Name = Values[5];
            CurrentSetting.IsBasicSetting = Convert.ToBoolean(Values[6]);
            CurrentSetting.Description = Values[7];
            // CurrentSetting wird in die Liste eingetragen
            HotKeySettings.Add(CurrentSetting);
        }

        return HotKeySettings;
    }

    public static void SaveKeyBindings(List<HotKeySetting> KeyBindings)
    {
        string FinalFileContent = "";

        // Jedes KeyBinding wird einzeln verarbeitet
        foreach (HotKeySetting KeyBinding in KeyBindings)
        {
            // Einzelne Werte werden in einen gut lesbaren string convertet und hinzugefügt
            FinalFileContent += KeyBinding.Key.GetHashCode() + SceneDB.ContentSpliter;
            FinalFileContent += KeyBinding.Modifier_Alt.ToString() + SceneDB.ContentSpliter;
            FinalFileContent += KeyBinding.Modifier_CapsLock.ToString() + SceneDB.ContentSpliter;
            FinalFileContent += KeyBinding.Modifier_Control.ToString() + SceneDB.ContentSpliter;
            FinalFileContent += KeyBinding.Modifier_Shift.ToString() + SceneDB.ContentSpliter;
            FinalFileContent += KeyBinding.Name + SceneDB.ContentSpliter;
            FinalFileContent += KeyBinding.IsBasicSetting.ToString() + SceneDB.ContentSpliter;
            FinalFileContent += KeyBinding.Description + SceneDB.ContentSpliter;

            // Wenn nicht das letzte KeyBinding wird ein Separator geaddet
            if (KeyBinding != KeyBindings.Last())
                FinalFileContent += SceneDB.SettingSeparator;
        }

        // Encodet die File und speichert sie dynamisch am Speicherort
        File.WriteAllBytes(SceneDB.CreateDynamicFilePath(SceneDB.KeyBindingsPath), GameLanguageConverter.Encode(FinalFileContent, SceneDB.KeyBindingsPath));
    }

    public void UpdateModifiers()
    {
        Modifier_Alt = AltIsPressed();
        Modifier_CapsLock = CapsLockIsPressed();
        Modifier_Control = ControlIsPressed();
        Modifier_Shift = ShiftIsPressed();
    }

    public static bool AltIsPressed()
    {
        // Prüft, ob Alt gedrückt wurde
        bool AltIsPressed = Input.GetKey(KeyCode.LeftAlt);
        // Wenn !AltIsPressed wird nach RightAlt geprüft
        if (!AltIsPressed)
            AltIsPressed = Input.GetKey(KeyCode.RightAlt);

        // Wert wird zurück gegeben
        return AltIsPressed;
    }
    public static bool CapsLockIsPressed()
    {
        // Prüft, ob CapsLock gedrückt wurde
        return Input.GetKey(KeyCode.CapsLock);
    }
    public static bool ControlIsPressed()
    {
        // Prüft, ob Control gedrückt wurde
        bool ControlIsPressed = Input.GetKey(KeyCode.LeftControl);
        if (!ControlIsPressed)
            ControlIsPressed = Input.GetKey(KeyCode.RightControl);
        if (!ControlIsPressed)
            ControlIsPressed = Input.GetKey(KeyCode.LeftCommand);
        if (!ControlIsPressed)
            ControlIsPressed = Input.GetKey(KeyCode.RightCommand);
        return ControlIsPressed;
    }
    public static bool ShiftIsPressed()
    {
        bool ShiftIsPressed = Input.GetKey(KeyCode.LeftShift);
        if (!ShiftIsPressed)
            ShiftIsPressed = Input.GetKey(KeyCode.RightShift);
        return ShiftIsPressed;
    }

    public bool InputIsValid()
    {
        bool IsValid = true;

        // Wenn !IsMainKey wird alles auf direkte Übereinstimmung geprüft
        if (!IsMainKey)
        {
            // Überprüft, ob Key aktiv ist
            if (!Input.GetKey(Key) && Key != KeyCode.None)
                IsValid = false;
            else
            {
                // Wenn die aktuellen Modifier exakt die Values haben, wird IsValid true sein
                if (AltIsPressed() != Modifier_Alt)
                    IsValid = false;
                if (CapsLockIsPressed() != Modifier_CapsLock)
                    IsValid = false;
                if (ControlIsPressed() != Modifier_Control)
                    IsValid = false;
                if (ShiftIsPressed() != Modifier_Shift)
                    IsValid = false;
            }
        }

        // Wenn IsMainKey dann wird nur geguckt, ob "true" Modifier stimmen
        else
        {
            // Überprüft, ob Key aktiv ist
            if (Input.GetKey(Key) && Key != KeyCode.None)
            {
                // Überprüft, ob alle Modifier von Key auch aktiv sind
                IsValid = true;

                if (Modifier_Alt && !AltIsPressed())
                    IsValid = false;
                if (Modifier_CapsLock && !CapsLockIsPressed())
                    IsValid = false;
                if (Modifier_CapsLock && !ControlIsPressed())
                    IsValid = false;
                if (Modifier_Shift && !ShiftIsPressed())
                    IsValid = false;
            }
        }

        return IsValid;
    }
}
