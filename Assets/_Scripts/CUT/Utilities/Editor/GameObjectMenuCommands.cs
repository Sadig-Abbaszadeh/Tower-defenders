using UnityEditor;
using UnityEngine;

namespace DartsGames.CUT.Editors
{
    public static class GameObjectMenuCommands
    {
        #region Joystick
        [MenuItem("GameObject/UI/DartsGames/Joystick")]
        private static void CreateJoystick() => Object.Instantiate(Resources.Load("JoystickActivator"), Selection.activeGameObject.transform).name = "JoystickActivator";

        [MenuItem("GameObject/UI/DartsGames/Joystick", true)]
        private static bool CreateJoystickValidation() => Selection.activeGameObject != null && Selection.activeGameObject.TryGetComponent(out RectTransform rt);
        #endregion
    }
}