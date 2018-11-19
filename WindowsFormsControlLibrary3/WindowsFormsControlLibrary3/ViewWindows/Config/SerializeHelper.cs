using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using System.Text;
using System.Xml;
using System.Xml.Serialization;
namespace ViewWindows.Config
{
    public class SerializeHelper
    {
        public static void Save(object obj, string filename)
        {
            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
                xmlSerializer.Serialize(fileStream, obj);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }
        public static object Load(Type type, string filename)
        {
            FileStream fileStream = null;
            object result;
            try
            {
                fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                XmlSerializer xmlSerializer = new XmlSerializer(type);
                result = xmlSerializer.Deserialize(fileStream);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
            return result;
        }
        public string ToXml<T>(T item)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(item.GetType());
            StringBuilder stringBuilder = new StringBuilder();
            string result;
            using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder))
            {
                xmlSerializer.Serialize(xmlWriter, item);
                result = stringBuilder.ToString();
            }
            return result;
        }
        public T FromXml<T>(string str)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            T result;
            using (XmlReader xmlReader = new XmlTextReader(new StringReader(str)))
            {
                result = (T)((object)xmlSerializer.Deserialize(xmlReader));
            }
            return result;
        }
        public string ToSoap<T>(T item)
        {
            BinaryFormatter soapFormatter = new BinaryFormatter();
            string innerXml;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                soapFormatter.Serialize(memoryStream, item);
                memoryStream.Position = 0L;
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(memoryStream);
                innerXml = xmlDocument.InnerXml;
            }
            return innerXml;
        }
        public T FromSoap<T>(string str)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(str);
            BinaryFormatter soapFormatter = new BinaryFormatter();
            T result;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                xmlDocument.Save(memoryStream);
                memoryStream.Position = 0L;
                result = (T)((object)soapFormatter.Deserialize(memoryStream));
            }
            return result;
        }
        public string ToBinary<T>(T item)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            string result;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, item);
                memoryStream.Position = 0L;
                byte[] array = memoryStream.ToArray();
                StringBuilder stringBuilder = new StringBuilder();
                byte[] array2 = array;
                for (int i = 0; i < array2.Length; i++)
                {
                    byte b = array2[i];
                    stringBuilder.Append(string.Format("{0:X2}", b));
                }
                result = stringBuilder.ToString();
            }
            return result;
        }
        public T FromBinary<T>(string str)
        {
            int num = str.Length / 2;
            byte[] array = new byte[num];
            for (int i = 0; i < num; i++)
            {
                int num2 = Convert.ToInt32(str.Substring(i * 2, 2), 16);
                array[i] = (byte)num2;
            }
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            T result;
            using (MemoryStream memoryStream = new MemoryStream(array))
            {
                result = (T)((object)binaryFormatter.Deserialize(memoryStream));
            }
            return result;
        }
        public static byte[] GetBytes(object pObj)
        {
            byte[] result;
            if (pObj == null)
            {
                result = null;
            }
            else
            {
                MemoryStream memoryStream = new MemoryStream();
                new BinaryFormatter().Serialize(memoryStream, pObj);
                memoryStream.Position = 0L;
                byte[] array = new byte[memoryStream.Length];
                memoryStream.Read(array, 0, array.Length);
                memoryStream.Close();
                result = array;
            }
            return result;
        }
        public static XmlDocument GetXmlDoc(object pObj)
        {
            XmlDocument result;
            if (pObj == null)
            {
                result = null;
            }
            else
            {
                XmlSerializer xmlSerializer = new XmlSerializer(pObj.GetType());
                StringBuilder stringBuilder = new StringBuilder();
                StringWriter stringWriter = new StringWriter(stringBuilder);
                xmlSerializer.Serialize(stringWriter, pObj);
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(stringBuilder.ToString());
                stringWriter.Close();
                result = xmlDocument;
            }
            return result;
        }
        public static T GetObject<T>(byte[] binData)
        {
            T result;
            if (binData == null)
            {
                result = default(T);
            }
            else
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                MemoryStream serializationStream = new MemoryStream(binData);
                result = (T)((object)binaryFormatter.Deserialize(serializationStream));
            }
            return result;
        }
        public static T GetObject<T>(XmlDocument xmlDoc)
        {
            T result;
            if (xmlDoc == null)
            {
                result = default(T);
            }
            else
            {
                XmlNodeReader xmlReader = new XmlNodeReader(xmlDoc.DocumentElement);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                result = (T)((object)xmlSerializer.Deserialize(xmlReader));
            }
            return result;
        }
    }
}
