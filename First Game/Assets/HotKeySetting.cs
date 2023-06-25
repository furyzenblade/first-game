using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotKeySetting : MonoBehaviour
{
    // Erstellt ein Setting mit Key & Name
    // Optional sind KeyModifier & Description
    public HotKeySetting(KeyCode Key, string Name, List<EventModifiers> Modifiers = null, bool BasicSetting = true, string Description = "")
    {
        // Speichert die gegebenen Werte
        this.Key = Key;
        this.Name = Name;
        this.BasicSetting = BasicSetting;
        this.Description = Description;

        // Wenn kein Modifier übergeben wurde, wird None rein gepackt, sonst werden Modifier so übergeben
        if (Modifiers != null)
            this.Modifiers = Modifiers;
        else { this.Modifiers.Add(EventModifiers.None); }

        // Setting zur Liste hinzufügen
        SceneDB.AllSettings.Add(this);
        if (BasicSetting)
            SceneDB.BasicSettings.Add(this);
        else
            SceneDB.AdvancedSettings.Add(this);
    }

    // Variablen zum Speichern des Keys und der Modifier
    public KeyCode Key;
    public List<EventModifiers> Modifiers = new() { };

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

        foreach (EventModifiers Modifier in Modifiers)
        {
            if ((Event.current.modifiers & Modifier) == 0)
            {
                Match = false;
            }
        }

        return Match;
    }
}
