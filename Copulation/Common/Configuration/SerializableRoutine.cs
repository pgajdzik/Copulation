using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace Copulation.Common
{
    public static class SerializableRoutine
    {
        public static T DeserializeObject<T>(string xml)
        {
            return DeserializeObject<T>(Encoding.UTF8, xml);
        }

        public static T DeserializeObject<T>(Encoding encoding, string xml)
        {
            try
            {
                using (MemoryStream memoryStream = new MemoryStream(StringToByteArray(encoding, xml)))
                {
                    using (XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, encoding))
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), string.Empty);
                        
                        return (T)xmlSerializer.Deserialize(memoryStream);
                    }
                }
            }
            catch
            {
                return default(T);
            }
        }

        public static string ToXml<T>(T o)
        {
            return SerializeObject<T>(Encoding.UTF8, o);
        }

        public static string SerializeObject<T>(Encoding encoding, T o)
        {
            try
            {
                MemoryStream memoryStream = new MemoryStream();

                using (XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, encoding))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), string.Empty);
                    xmlSerializer.Serialize(xmlTextWriter, o);

                    memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
                }

                return ByteArrayToString(encoding, memoryStream.ToArray());
            }
            catch
            {
                return string.Empty;
            }
        }

        private static Byte[] StringToByteArray(Encoding encoding, string xml)
        {
            return encoding.GetBytes(xml);
        }

        private static string ByteArrayToString(Encoding encoding, byte[] byteArray)
        {
            return encoding.GetString(byteArray);
        }
    }
}
