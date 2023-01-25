using UnityEngine;
using UnityEditor;

namespace DartsGames.CUT.Editors
{
    [CustomEditor(typeof(ScriptableObject), true), CanEditMultipleObjects]
    public class ExtendedEditor_SO : ExtendedEditor { }
}