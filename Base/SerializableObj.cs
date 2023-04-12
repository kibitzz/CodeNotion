using basicClasses.Interfaces;
using basicClasses.models.file_system;
using basicClasses.models.WEB_api;
using Jint.Parser.Ast;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.Base
{
    public class SerializableObj: IObjectSerializable
    {
        public virtual string GetSaveEncodedData(string data)
        {
            return Compress.EncodeBase64(data);
        }

        public virtual string GetDecodedData(string data)
        {
            return Compress.DecodeBase64(data);
        }

        public string Serialize()
        {
            return GetSaveEncodedData(JsonConvert.SerializeObject(this));
        }

        public void Deserialize(string data)
        {
            JsonConvert.PopulateObject(GetDecodedData(data), this);
        }
    }
}
