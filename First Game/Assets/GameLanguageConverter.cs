using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

// MAX. 2.1GB Dateien können hiermit verarbeitet werden
// Verarbeiten von Dateien ab 100MB+ werden nicht empfohlen wegen Performance

public class GameLanguageConverter : MonoBehaviour
{
    // file name zum Entschlüsseln
    public static string FileName;

    // seed zum entschlüsseln
    public static int Seed { get; private set; }

    // File oder Daten, die verschlüsselt werden sollen
    public static byte[] FileContent;

    // Methode zum Converten des File Names in einen Seed zum auslesen
    private static int GetSeed(string FileName)
    {
        // Bytes werden aus dem FileName erzeugt
        byte[] RawSeed = Encoding.ASCII.GetBytes(FileName);

        // Value jedes Bytes wird in Seed geaddet
        foreach (byte b in RawSeed)
        {
            Seed += b.GetHashCode() % 256;
        }

        // Seed wird auf 256 stellen limitiert für Effizienz 
        Seed %= 256;

        return Seed;
    }

    // Liest und encodet eine File
    public static string ReadFile(string Filename)
    {
        // FileName wird übergeben beim Einlesen
        FileName = Filename;

        // Seed wird vom FileName analysiert
        GetSeed(Filename.Split("/").ToList().Last());

        // Wenn die File nicht existeirt, wird ein Error ausgespuckt
        if (!File.Exists(SceneDB.CreateDynamicFilePath(FileName)))
            Debug.LogError("File: " + SceneDB.CreateDynamicFilePath(FileName) + " was not found");
        else
        {
            // FilePath dynamisch mit Filename codieren (aus Assets- Folder)
            return Encoding.ASCII.GetString(Decode(SceneDB.CreateDynamicFilePath(FileName)));
        }
        return "";
    }

    // Encodet die File von Byte[] in string & umgekehrt
    private static byte[] Decode(string FilePath)
    {
        FileContent = File.ReadAllBytes(FilePath);

        // Verschieben von byte values mithilfe vom seed für encoding
        for (int i = 0; i < FileContent.Length; i++)
        {
            int numbers = FileContent[i].GetHashCode() % 256;

            numbers += -1 * ((Seed + i) % 256);
            if (numbers < 0)
            {
                //müsste wegen int Aufrundung funktionieren
                numbers += Math.Abs(numbers) / 256 * 256;
            }

            FileContent[i] = (byte)(numbers % 256);
        }

        return FileContent;
    }
    public static byte[] Encode(string Content, string Filename)
    {
        GetSeed(Filename.Split("/").ToList().Last());

        FileContent = Encoding.ASCII.GetBytes(Content);

        // Verschieben von byte values mithilfe vom seed für encoding
        for (int i = 0; i < FileContent.Length; i++)
        {
            int numbers = FileContent[i].GetHashCode() % 256;

            numbers += (Seed + i) % 256;
            if (numbers < 0)
            {
                //müsste wegen int Aufrundung funktionieren
                numbers += Math.Abs(numbers) / 256 * 256;
            }

            FileContent[i] = (byte)(numbers % 256);
        }

        return FileContent;
    }
}
