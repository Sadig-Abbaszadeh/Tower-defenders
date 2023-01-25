using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace DartsGames.CUT.Editors
{
    public static class SceneLoader
    {
        [Shortcut("Open scene 0", KeyCode.Alpha0, ShortcutModifiers.Alt | ShortcutModifiers.Shift)]
        private static void OpenScene0() => OpenScene(0);

        [Shortcut("Open scene 1", KeyCode.Alpha1, ShortcutModifiers.Alt | ShortcutModifiers.Shift)]
        private static void OpenScene1() => OpenScene(1);

        [Shortcut("Open scene 2", KeyCode.Alpha2, ShortcutModifiers.Alt | ShortcutModifiers.Shift)]
        private static void OpenScene2() => OpenScene(2);
        
        [Shortcut("Open scene 3", KeyCode.Alpha3, ShortcutModifiers.Alt | ShortcutModifiers.Shift)]
        private static void OpenScene3() => OpenScene(3);

        [Shortcut("Open scene 4", KeyCode.Alpha4, ShortcutModifiers.Alt | ShortcutModifiers.Shift)]
        private static void OpenScene4() => OpenScene(4);
        
        [Shortcut("Open scene 5", KeyCode.Alpha5, ShortcutModifiers.Alt | ShortcutModifiers.Shift)]
        private static void OpenScene5() => OpenScene(5);

        [Shortcut("Open scene 6", KeyCode.Alpha6, ShortcutModifiers.Alt | ShortcutModifiers.Shift)]
        private static void OpenScene6() => OpenScene(6);

        [Shortcut("Open scene 7", KeyCode.Alpha7, ShortcutModifiers.Alt | ShortcutModifiers.Shift)]
        private static void OpenScene7() => OpenScene(7);

        [Shortcut("Open scene 8", KeyCode.Alpha8, ShortcutModifiers.Alt | ShortcutModifiers.Shift)]
        private static void OpenScene8() => OpenScene(8);

        [Shortcut("Open scene 9", KeyCode.Alpha9, ShortcutModifiers.Alt | ShortcutModifiers.Shift)]
        private static void OpenScene9() => OpenScene(9);

        private static void OpenScene(int index)
        {
            if (EditorBuildSettings.scenes.Length <= index &&
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(EditorBuildSettings.scenes[index].path);
            }
        }
    }
}