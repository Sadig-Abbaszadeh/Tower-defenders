using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace DartsGames.CUT
{
    public static class SaveSystem
    {
        public static void SaveObject(string fileName, object saveObject) => _SaveObject(Path.Combine(Application.persistentDataPath, fileName), saveObject);

#if UNITY_EDITOR
        /// <summary>
        /// Save in dataPath rather than persistentDataPath. Only exists in editor.
        /// </summary>
        public static void SaveRelativeToAssetsFolder(string fileName, object saveObject) => _SaveObject(Path.Combine(Application.dataPath, fileName), saveObject);
#endif

        private static void _SaveObject(string fullPath, object saveObject)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(fullPath, FileMode.Create);

            formatter.Serialize(stream, saveObject);
            stream.Close();
        }

        public static object LoadObject(string fileName)
        {
            string fullPath = Path.Combine(Application.persistentDataPath, fileName);

            if (File.Exists(fullPath))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(fullPath, FileMode.Open);

                object obj = formatter.Deserialize(stream);
                stream.Close();
                return obj;
            }
            else
            {
                return null;
            }
        }

        public static void DeleteFile(string fileName)
        {
            string fullPath = Path.Combine(Application.persistentDataPath, fileName);

            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
    }
}