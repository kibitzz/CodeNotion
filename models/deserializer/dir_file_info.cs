using basicClasses.Interfaces;
using basicClasses.models.file_system;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.deserializer
{
    public class dir_file_info : ModelBase
    {
        public override void Process(opis message)
        {
            string obj = null; 
            if (message.bodyObject != null && (obj = message.bodyObject as string)!= null)
            {

                if (message.PartitionKind == "dir")
                {
                    message.bodyObject = new DirInfo();
                    ((IObjectSerializable)message.bodyObject).Deserialize(obj);
                }
                else if (message.PartitionKind == "`")
                {
                    message.bodyObject = new mFileInfo();
                    ((IObjectSerializable)message.bodyObject).Deserialize(obj);
                }
            }

        }


    }
}
