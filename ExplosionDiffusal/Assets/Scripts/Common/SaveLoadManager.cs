using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager INSTANCE;

    private void Awake()
    {
        if(INSTANCE == null)
        {
            INSTANCE = this;
        } else { Destroy(INSTANCE); }
    }


    public void SaveHistoryObject(HistoryItemData obj)
    {
        string filePath = Path.Combine(Application.persistentDataPath, "history.dat");

        BinaryFormatter formatter = new BinaryFormatter();

        using (FileStream fileStream = new FileStream(filePath, FileMode.Append))
        {
            formatter.Serialize(fileStream, obj);
        }
    }


    public List<HistoryItemData> LoadHistoryObjects()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "history.dat");

        if (File.Exists(filePath))
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                List<HistoryItemData> historyItems = new List<HistoryItemData>();

                using (FileStream fileStream = File.OpenRead(filePath))
                {
                    while (fileStream.Position < fileStream.Length)
                    {
                        HistoryItemData historyItem = (HistoryItemData)formatter.Deserialize(fileStream);
                        historyItems.Add(historyItem);
                    }
                }

                return historyItems;
            }
            catch (IOException e)
            {
                Debug.LogError($"Failed to read history file: {e.Message}");
                return null;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to deserialize history objects: {e.Message}");
                return null;
            }
        }
        else
        {
            Debug.LogWarning("No history file found.");
            return null;
        }
    }
}
