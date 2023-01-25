using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DartsGames.CUT.Editors
{
    public static class MenuCommands
    {
#if UNITY_EDITOR_WIN
        [MenuItem("DartsGames/CUT/Open persistent data path")]
        private static void ShowPersPath()
        {
            System.Diagnostics.Process.Start("explorer.exe",
                Application.persistentDataPath.Replace(@"/", @"\"));
        }
#endif
    }
}