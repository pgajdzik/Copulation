using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Collections;

namespace Copulation.Common.Configuration
{
    public static class SectionFactory
    {
        public static T Create<T>() where T : Section
        {
            T section = Activator.CreateInstance<T>();

            return (T)section.Defaults();
        }

        public static T LoadFromFile<T>(string fileName) where T : Section
        {
            try
            {
                var xmlContent = new StringReader(File.ReadAllText(fileName, Encoding.UTF8));
                var readerSettings = new XmlReaderSettings()
                {
                    CloseInput = true,
                };
                
                var reader = XmlReader.Create(xmlContent, readerSettings);

                T section = Create<T>();

                section.UpdateFrom(reader);

                return section;
                
            }
            catch
            {
            }

            return (T)null;
        }

        public static bool SaveToFile(string fileName, Section section)
        {
            if (section == null)
                throw new ArgumentNullException("section");

            section.Update();

            var xmlContent = new StringBuilder();

            var writerSettings = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "  ",
                Encoding = Encoding.UTF8,
            };

            var writer = XmlWriter.Create(xmlContent, writerSettings);

            section.RenderXml(writer);

            writer.Flush();
            writer.Close();

            try
            {
                File.WriteAllText(fileName, xmlContent.ToString(), Encoding.UTF8);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
