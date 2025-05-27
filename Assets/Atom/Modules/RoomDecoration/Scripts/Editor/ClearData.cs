using System.Collections;
using System.Collections.Generic;
using System.IO;
using RoomDecoration;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class ClearData : EditorWindow
{
    [MenuItem("Tools/RoomDecoration/Delete room decoration data")]
    public static void DeleteRoomDecorationData()
    {
        string path = Path.Combine(Application.persistentDataPath, RoomDecorationConstants.SAVE_NAME);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    [MenuItem("Tools/RoomDecoration/Open sample scene")]
    public static void OpenSampleScene()
    {
        EditorSceneManager.OpenScene("Assets/Atom/Modules/RoomDecoration/Scenes/RoomDecoration.unity");
    }
}
