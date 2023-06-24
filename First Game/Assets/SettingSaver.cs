using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingSaver : MonoBehaviour
{
    // Erstellt ein Setting mit Key & Name
    // Optional sind KeyModifier & Description
    public SettingSaver(KeyCode Key, string Name, EventModifiers Modifiers = EventModifiers.None, bool BasicSetting = true, string Description = "")
    {
        // Speichert die gegebenen Werte
        this.Key = Key;
        this.Name = Name;
        this.Modifiers = Modifiers;
        this.BasicSetting = BasicSetting;
        this.Description = Description;

        // Setting zur Liste hinzufügen
        SceneDB.AllSettings.Add(this);
        if (BasicSetting)
            SceneDB.BasicSettings.Add(this);
        else
            SceneDB.AdvancedSettings.Add(this);
    }

    // Variablen zum Speichern des Keys und der Modifier
    public KeyCode Key;
    public EventModifiers  Modifiers;

    // Gibt an, ob das Setting zu Basic oder Advanced Settings gehört
    public bool BasicSetting;

    // Variablen zur eindeutigen Identifizierung eines Settings
    public int ID = SceneDB.AddSettingID();

    public string Name;
    public string Description;

    // Guckt, ob aktuell die Inputs für das Setting gegeben sind
    public bool CheckInput()
    {
        bool Match = true;

        // Gibt false zurück, sollten die Werte nicht perfekt übereinstimmen 
        if (!Input.GetKey(Key))
            Match = false;
        if (Event.current.modifiers != Modifiers)
                Match = false;

        return Match;
    }
}
