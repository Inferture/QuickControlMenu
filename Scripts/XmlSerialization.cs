using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;


namespace QuickControlMenu
{
    public class XmlSerialization
    {
        public static T ReadFromXmlResource<T>(TextAsset ta)
        {
            using (TextReader textReader = new StringReader(ta.text))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));

                T XmlData = (T)serializer.Deserialize(textReader);

                return XmlData;
            }
        }
        public static void WriteToXmlResource<T>(string filePath, T obj, bool append = false)
        {
            TextWriter writer = null;

            if (!Directory.Exists(Path.Combine(Application.dataPath, "Resources")))
            {
                Directory.CreateDirectory(Path.Combine(Application.dataPath, "Resources"));
            }
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                writer = new StreamWriter(Path.Combine(Path.Combine(Application.dataPath, "Resources"), filePath), append);
                serializer.Serialize(writer, obj);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
                Debug.Log("xml serialized into " + filePath);
            }
        }
    }
}