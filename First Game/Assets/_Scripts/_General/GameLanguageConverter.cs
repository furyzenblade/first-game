using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

// MAX. 2.1GB Dateien können hiermit verschlüsselt / entschlüsselt werden
// Verarbeiten von Dateien ab 100MB+ werden nicht empfohlen wegen Performance
public class GameLanguageConverter
{
    public static string FileEnding = ".GData";

    // Methode zum Converten des File Names in einen Seed zum auslesen
    private static int GetSeed(string FileName)
    {
        // Wenn der komplette, dynamische Dateipfad vom Game vorhanden ist, wird er abgeschnitten
        if (FileName.Contains(Application.dataPath))
        {
            FileName = FileName[Application.dataPath.Count()..];
            // Wenn das File Ending vom Game vorhanden ist, wird es abgeschnitten
            if (FileName.Contains(FileEnding))
                FileName = FileName.Split(FileEnding)[0];
        }

        // Bytes werden aus dem FileName erzeugt
        byte[] RawSeed = Encoding.UTF8.GetBytes(FileName.Split('/').ToList().Last());

        // Wird erstellt, um keine Loop zu erzeugen
        int CurrentSeed = 0;

        // Value jedes Bytes wird in Seed geaddet
        foreach (byte b in RawSeed)
            CurrentSeed += b.GetHashCode() % 256;

        // Seed wird auf 256 stellen limitiert 
        CurrentSeed %= 256;

        return CurrentSeed;
    }


    // Encodet einen bool[] Content, optional mit Speicherung
    // Coding Richtung ist 1
    public static byte[] Encode(string FileName, byte[] Content, bool SaveFile = false)
    {
        // Seed wird vom FileName erstellt
        int Seed = GetSeed(FileName);

        // Ändert den FileContent
        Content = ChangeBytes(Content, Seed, 1);

        // Wenn SaveFile dann speichert die File
        if (SaveFile)
            File.WriteAllBytes(SceneDB.CreateDynamicFilePath(FileName), Content);

        return Content;
    }
    // Encodet einen string Content zu einer File, optional mit Speicherung
    // Coding Richtung ist 1
    public static byte[] Encode(string FileName, string Content, bool SaveFile = false)
    {
        byte[] BContent = Encoding.UTF8.GetBytes(Content);
        return Encode(FileName, BContent, SaveFile);
    }
    // Liest & Encodet eine File, optional mit Speicherung
    // Coding Richtung ist 1
    public static byte[] Encode(string FileName, bool SaveFile = false)
    {
        // Liest FileContent ein, wenn möglich
        byte[] Content;
        try
        {
            Content = File.ReadAllBytes(FileName);
        }
        catch
        {
            // Gibt einen Error aus, wenn keine File gefunden wurde
            Debug.LogError("File: " + FileName + " was not found");
            return null;
        }
        try
        {
            // Wenn die File String kompatibel ist, wird sie als string encodet
            string StrContent = Encoding.UTF8.GetString(Content);
            return Encode(FileName, StrContent, SaveFile);
        }
        catch
        { return Encode(FileName, Content, SaveFile); }
    }


    // Encodet einen bool[] Content in einen string, optional mit Speicherung
    // Coding Richtung ist 1
    public static string StrEncode(string FileName, byte[] Content, bool SaveFile = false)
    {
        return Encoding.UTF8.GetString(Encode(FileName, Content, SaveFile));
    }
    // Encodet einen string Content in einen string, optional mit Speicherung
    // Coding Richtung ist 1
    public static string StrEncode(string FileName, string Content, bool SaveFile = false)
    {
        byte[] BContent = Encoding.UTF8.GetBytes(Content);
        return Encoding.UTF8.GetString(Encode(FileName, BContent, SaveFile));
    }
    // Liest & Encodet eine File in einen string, optional mit Speicherung
    // Coding Richtung ist 1
    public static string StrEncode(string FileName, bool SaveFile = false)
    {
        return Encoding.UTF8.GetString(Encode(FileName, SaveFile));
    }


    // Decodet einen bool[] Content, optional mit Speicherung
    // Coding Richtung ist -1
    public static byte[] Decode(string FileName, byte[] Content, bool SaveFile = false)
    {
        // Seed wird vom FileName erstellt
        int Seed = GetSeed(FileName);

        // Ändert den FileContent
        Content = ChangeBytes(Content, Seed, -1);

        // Wenn SaveFile dann speichert die File
        if (SaveFile)
            File.WriteAllBytes(SceneDB.CreateDynamicFilePath(FileName), Content);

        return Content;
    }
    // Decodet einen string Content zu einer File, optional mit Speicherung
    // Coding Richtung ist -1
    public static byte[] Decode(string FileName, string Content, bool SaveFile = false)
    {
        byte[] BContent = Encoding.UTF8.GetBytes(Content);
        return Decode(FileName, BContent, SaveFile);
    }
    // Liest & Decodet eine File, optional mit Speicherung
    // Coding Richtung ist -1
    public static byte[] Decode(string FileName, bool SaveFile = false)
    {
        // Liest FileContent ein, wenn möglich
        byte[] Content;
        try
        {
            Content = File.ReadAllBytes(FileName);
        }
        catch
        {
            // Gibt einen Error aus, wenn keine File gefunden wurde
            Debug.LogError("File: " + FileName + " was not found");
            return null;
        }

        // Decodet auf Byte Ebene
        return Decode(FileName, Content, SaveFile);
    }


    // Decodet einen bool[] Content in einen string, optional mit Speicherung
    // Coding Richtung ist -1
    public static string StrDecode(string FileName, byte[] Content, bool SaveFile = false)
    {
        return Encoding.UTF8.GetString(Decode(FileName, Content, SaveFile));
    }
    // Decodet einen string Content in einen string, optional mit Speicherung
    // Coding Richtung ist -1
    public static string StrDecode(string FileName, string Content, bool SaveFile = false)
    {
        byte[] BContent = Encoding.UTF8.GetBytes(Content);
        return Encoding.UTF8.GetString(Decode(FileName, BContent, SaveFile));
    }
    // Liest & Decodet eine File in einen string, optional mit Speicherung
    // Coding Richtung ist -1
    public static string StrDecode(string FileName, bool SaveFile = false)
    {
        return Encoding.UTF8.GetString(Decode(FileName, SaveFile));
    }


    public static T Decode<T>(string FileName, bool SaveFile = false)
    {
        byte[] Content = Decode(FileName, SaveFile);

        if (Content == null)
            throw new ArgumentNullException(nameof(Content));

        using MemoryStream memoryStream = new(Content);

        BinaryFormatter binaryFormatter = new();
        object obj = binaryFormatter.Deserialize(memoryStream);

        if (obj is T result)
        {
            return result;
        }
        else
        {
            throw new InvalidCastException($"Failed to cast deserialized object to type {typeof(T)}.");
        }
    }
    public static byte[] Encode<T>(string FileName, T obj, bool ReturnUnencodedByteArray = false, bool SaveFile = false)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj));

        using MemoryStream memoryStream = new();

        BinaryFormatter binaryFormatter = new();
        binaryFormatter.Serialize(memoryStream, obj);

        // Returnt das verschlüsselte Byte Array
        if (!ReturnUnencodedByteArray)
            return Encode(FileName, memoryStream.ToArray(), SaveFile);

        // Returnt das nicht verschlüsselte Byte Array
        Encode(FileName, memoryStream.ToArray(), SaveFile);
        return memoryStream.ToArray();
    }


    // Verschiebt die Bytes algorithmisch
    private static byte[] ChangeBytes(byte[] Content, int Seed, int Direction)
    {
        // Verschiebt jeden Byte algorithmisch
        for (int i = 0; i < Content.Length; i++)
        {
            // Liest den Byte ein
            int numbers = Content[i].GetHashCode() % 256;
            
            // Manipuliert die Numbers algorithmisch zum Verschlüsseln
            numbers += Direction * ((Seed + i) % 256);

            // Addet so lange +256 so lange numbers < 0 ist
            if (numbers < 0)
                numbers += Math.Abs(numbers) / 256 * 256;

            // Schreibt den Byte
            Content[i] = (byte)(numbers % 256);
        }

        return Content;
    }
}
