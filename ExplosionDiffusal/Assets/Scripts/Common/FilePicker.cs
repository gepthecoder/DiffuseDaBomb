using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FilePicker : MonoBehaviour
{

    private string m_FinalPath = string.Empty;

    public string LoadSelectedFilePath()
    {
        string fileType = NativeFilePicker.ConvertExtensionToFileType("png,jpg,jpeg");

        NativeFilePicker.Permission permission = NativeFilePicker.PickFile((path) => { 
            if(path == null)
            {
                Debug.Log("FilePicker::Operation Canceled!");
            }else
            {
                m_FinalPath = path;
                Debug.Log($"FilePicker::Picked File: {m_FinalPath}");
            }
        }, new string[] { fileType } );

        return m_FinalPath;
    }

    public void SaveFile()
    {
        string filePath = Path.Combine(Application.temporaryCachePath, "bomba.txt");
        File.WriteAllText(filePath, "G3PRODUCTION");

        // export
        NativeFilePicker.Permission permission = NativeFilePicker.ExportFile(
            filePath,
            (success)=> {
                Debug.Log($"FilePicker:: File Exported: {success}");
            }
        );
    }
}
