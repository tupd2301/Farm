using UnityEngine;
using UnityEditor;
using System.IO;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Athena.Common.Editor
{
    public static class UtilMenu
    {
#if UNITY_EDITOR
        [MenuItem("Athena/Utils/Data/Clear Player Pref", false, 20)]
        public static void ClearPlayerPref()
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("Cleared player preferences!");
        }

        [MenuItem("Athena/Utils/Data/Clear Persistant Data", false, 20)]
        public static void ClearPersistantData()
        {
            Debug.Log("Persistant data located at :" + Application.persistentDataPath);
            DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath);
            dir.Delete(true);
            Debug.Log("Deleted player persistant data!");
        }

        [MenuItem("Athena/Utils/Data/Clear temporary Data", false, 20)]
        public static void ClearTempData()
        {
            Debug.Log("Temporary data located at :" + Application.temporaryCachePath);
            DirectoryInfo dir = new DirectoryInfo(Application.temporaryCachePath);
            dir.Delete(true);
            Debug.Log("Deleted player temporary data!");
        }

        [MenuItem("Athena/Utils/Data/Clear All", false, 12)]
        public static void ClearAll()
        {
            ClearPlayerPref();
            ClearPersistantData();
            ClearTempData();
        }

        [MenuItem("Athena/Utils/Memory/UnloadAllUnusedAssets", false, 20)]
        public static void UnloadAllUnusedAsset()
        {
            Debug.Log("Unload all unused assets ");

            Resources.UnloadUnusedAssets();
            EditorUtility.UnloadUnusedAssetsImmediate();
        }

        [MenuItem("Project/Play", false, 20)]
        public static void Play()
        {
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Farm/Scenes/bootstrap.unity");
            EditorApplication.isPlaying = true;
        }

        [MenuItem("Project/Utils/Open Folder", false, 20)]
        public static void OpenProjectFolderInExplorer()
        {
            string projectFolderPath = Application.dataPath;
            projectFolderPath = System.IO.Path.GetDirectoryName(projectFolderPath);
            if (projectFolderPath != null)
        {
#if UNITY_EDITOR_WIN
            Process.Start("explorer.exe", projectFolderPath.Replace("/", "\\"));
#elif UNITY_EDITOR_OSX
            Process.Start("open", projectFolderPath);
#elif UNITY_EDITOR_LINUX
            Process.Start("xdg-open", projectFolderPath);
#endif
            UnityEngine.Debug.Log("Opened project folder: " + projectFolderPath);
        }
        }

        [MenuItem("Project/1. bootstrap", false, 21)]
        public static void ToSceneBootstrap()
        {
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Farm/Scenes/bootstrap.unity");
        }
        [MenuItem("Project/2. Factory", false, 22)]
        public static void ToSceneHome()
        {
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Factory/Scenes/Factory.unity");
        }

        [MenuItem("Project/Edit Script...", false, 10000)]
        public static void OpenScript()
        {
            var currentFilePath = GetCurrentFilePath();
            var currentLineNumber = GetCurrentLineNumber();
            UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(currentFilePath, currentLineNumber + 1);
        }

        public static string GetCurrentFilePath([CallerFilePath] string filePath = "")
        {
            return filePath;
        }

        public static int GetCurrentLineNumber([CallerLineNumber] int lineNumber = 0)
        {
            return lineNumber;
        }
#endif
    }
}