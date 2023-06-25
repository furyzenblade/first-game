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

        // Wenn kein Modifier �bergeben wurde, wird None rein gepackt, sonst werden Modifier so �bergeben
        if (Modifiers != null)
            this.Modifiers = Modifiers;
        else { this.Modifiers.Add(EventModifiers.None); }

        // Setting zur Liste hinzuf�gen
        SceneDB.KeyBindings.Add(this);
    }

    // Variablen zum Speichern des Keys und der Modifier
    public KeyCode Key;
    public List<EventModifiers> Modifiers = new() { };

    // Gibt an, ob das Setting zu Basic oder Advanced Settings geh�rt
    public bool BasicSetting;

    // Variablen zur eindeutigen Identifizierung eines Settings
    public string Name;
    public string Description;

    // Guckt, ob aktuell die Inputs f�r das Setting gegeben sind
    public bool CheckInput()
    {
        bool Match = true;

        // Gibt false zur�ck, sollten die Werte nicht perfekt �bereinstimmen 
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
