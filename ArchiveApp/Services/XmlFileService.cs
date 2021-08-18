using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ArchiveApp.Services
{
    public class XmlFileService
    {

        public const string FILE_NAME_XML = "DropDownData.xml";

        public bool IsFileExist => File.Exists(FILE_NAME_XML);

        public void CreateFile()
        {
            string data = Properties.Resources.DropDownData;
            File.WriteAllText(FILE_NAME_XML, data);
        }

        public async Task<T> Deserialize<T>()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            
            await using(FileStream fs = new FileStream(FILE_NAME_XML, FileMode.Open, FileAccess.Read))
            {
                return (T)serializer.Deserialize(fs);
            }
        }

        public void Serialize<T>(T data)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            
            using(FileStream fs = new FileStream(FILE_NAME_XML, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                serializer.Serialize(fs, data);
            }
        }

        internal FileStream GetStream()
        {
            return new FileStream(FILE_NAME_XML, FileMode.Open, FileAccess.ReadWrite);
        }
    }
}
