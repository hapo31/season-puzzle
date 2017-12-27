using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Util
{
    [Serializable]
    struct SaveData
    {
        public int HiScore;
        public SaveData(int HiScore)
        {
            this.HiScore = HiScore;
        }
    }

    class SaveDataManager
    {

        private string filename;
        public string Filename => filename;

        private char datakey;

        public SaveDataManager(string filename = "save.dat", char savedataKey = 'O')
        {
            this.filename = filename;
            this.datakey = savedataKey;
        }

        public bool Save(SaveData data)
        {
            try
            {
                var encodedData = encode(data);
                File.WriteAllBytes(filename, encodedData);
            } 
            catch(Exception e)
            {

                return false;
            }

            return true;
        }

        public SaveData Load()
        {
            return new SaveData();
        }

        private byte[] encode(SaveData data)
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(SaveData));
            var ms = new MemoryStream();
            serializer.Serialize(ms, data);
            return ms.ToArray();
        }

        private SaveData? decode(byte[] data)
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(SaveData));
            var ms = new MemoryStream(data);
            return serializer.Deserialize(ms) as SaveData?;
        }

        private void xor(ref byte[] src)
        {
            for (var i = 0; i < src.Length; ++i)
            {
                src[i] = (byte)(src[i] ^ datakey);
            }
        }

    }
}
