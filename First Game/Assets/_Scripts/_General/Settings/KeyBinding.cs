using System;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class KeyBinding : Setting
{
    // Konstruktor für alle Settings auf einmal (überwiegend für Default Setting Speicherung)
    public KeyBinding(string Name, string Description, bool SplitUI, bool IsMainSetting, KeyCode Key, bool Modifier_Alt, bool Modifier_CapsLock, bool Modifier_Control, bool Modifier_Shift, bool IsMainKey)
    {
        this.Name = Name;
        this.Description = Description;
        this.SplitUI = SplitUI;
        this.IsMainSetting = IsMainSetting;
        this.Key = Key;
        this.Modifier_Alt = Modifier_Alt;
        this.Modifier_CapsLock = Modifier_CapsLock;
        this.Modifier_Control = Modifier_Control;
        this.Modifier_Shift = Modifier_Shift;
        this.IsMainKey = IsMainKey;
    }
    // Konstruktor für Laden von Settings aus Files
    public KeyBinding(string Content)
    {
        DeSerialise(Content);
    }

    // Speichert den Key für den Input
    public KeyCode Key;

    // Speichern Values, ob die jeweiligen Modifier an oder aus sind
    public bool Modifier_Alt;
    public bool Modifier_CapsLock;
    public bool Modifier_Control;
    public bool Modifier_Shift;

    // Bestimmt, ob für einen Key alle Modifier exakt stimmen müssen oder ob es egal ist
    public bool IsMainKey;
    public new List<string> Serialise()
    {
        List<string> strSetting = base.Serialise();

        strSetting.Add(Key.ToString());
        strSetting.Add(Modifier_Alt.ToString());
        strSetting.Add(Modifier_CapsLock.ToString());
        strSetting.Add(Modifier_Control.ToString());
        strSetting.Add(Modifier_Shift.ToString());

        return strSetting;
    }

    private new KeyBinding DeSerialise(string Content)
    {
        // Erstellt die Grundinformationen, die jedes Setting hat
        List<string> Data = base.DeSerialise(Content);

        // Speichert die Werte ab
        //Key = (KeyCode)Convert.ToInt32(Data[0]);
        Key = (KeyCode)Enum.Parse(typeof(KeyCode), Data[0]);
        Modifier_Alt = bool.Parse(Data[1]);
        Modifier_CapsLock = bool.Parse(Data[2]);
        Modifier_Control = bool.Parse(Data[3]);
        Modifier_Shift = bool.Parse(Data[4]);

        return this;
    }

    public void SetKeyBinding()
    {
        Key = GetPressedKey();
        
        UpdateModifiers();
    }

    // Setzt die Modifier zu den aktuell gedrückten
    private void UpdateModifiers()
    {
        Modifier_Alt = AltIsPressed();
        Modifier_CapsLock = CapsLockIsPressed();
        Modifier_Control = ControlIsPressed();
        Modifier_Shift = ShiftIsPressed();
    }

    // Prüft, ob der im Namen stehende Modifier aktuell gedrückt wurde
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

    // Prüft, ob das HotKeySetting aktuell gedrückt wurde
    public bool IsActive(InputMode InputMode = InputMode.WhileInput)
    {
        bool IsValid = false;

        // Wenn !IsMainKey wird alles auf direkte Übereinstimmung geprüft
        if (!IsMainKey)
        {
            // Überprüft, ob Key aktiv ist
            if ((Input.GetKey(Key) && InputMode == InputMode.WhileInput) || (Input.GetKeyDown(Key) && InputMode == InputMode.OnFirstInput) || (Input.GetKeyUp(Key) && InputMode == InputMode.OnLastInput))
            {
                IsValid = true;

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
        else if ((Input.GetKey(Key) && InputMode == InputMode.WhileInput) || Input.GetKeyDown(Key) && InputMode == InputMode.OnFirstInput || Input.GetKeyUp(Key) && InputMode == InputMode.OnLastInput)
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

        return IsValid;
    }

    // Prüft, welcher Key aktuell gedrückt wurde
    private KeyCode GetPressedKey()
    {
        // Iterate through all possible key codes
        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
        {
            // Check if the current key is pressed down
            if (Input.GetKeyDown(keyCode))
            {
                return keyCode; // Return the currently pressed key
            }
        }
        return KeyCode.None; // Return KeyCode.None if no key is pressed
    }
}
