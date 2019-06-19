using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace POSIXComm
{
    public class POSIXConfig
    {
        public string QueueName { get; set; }
        public int AccessModifier { get; set; }
        public int QueueSize { get; set; }
        public int MessageSize { get; set; }

        [XmlIgnore]
        private static readonly XmlSerializer xmlSerializer = new XmlSerializer(typeof(POSIXConfig));

        public void Save(string filepath)
        {
            using (var textWriter = new StreamWriter(filepath))
            {
                using (var writer = XmlWriter.Create(textWriter, new XmlWriterSettings {Indent = true}))
                {
                    xmlSerializer.Serialize(writer, this);
                }
            }            
        }

        public void Load(string filepath)
        {
            POSIXConfig temp = null;

            using (var textReader = new StreamReader(filepath))
            {
                using (var reader = XmlReader.Create(textReader))
                {
                    temp = xmlSerializer.Deserialize(reader) as POSIXConfig;

                    if (temp != null)
                    {
                        Copy(temp);
                    }
                }
            }            
        }

        public bool IsValid(out string errorMsg)
        {
            bool invalid = false;
            errorMsg = string.Empty;

            var path = "/proc/sys/fs/mqueue/";

            if (string.IsNullOrEmpty(QueueName))
            {
                invalid |= true;
                errorMsg += "Queue Name is empty\r\n";
            }
            else if (QueueName.Contains(" "))
            {
                invalid |= true;
                errorMsg += "Queue Name contains a space\r\n";
            }

            if (AccessModifier < 0 || AccessModifier > 511)
            {
                invalid |= true;
                errorMsg += "Access Modifier not in valid range\r\n";
            }

            if (Type.GetType("Mono.Runtime") != null)
            {
                var lines = File.ReadAllLines(path + "msg_max");
                if (QueueSize <= 0 || int.Parse(lines[0]) < QueueSize)
                {
                    invalid |= true;
                    errorMsg += "Queue Size not in valid range\r\n";
                }
            }
            else
            {
                if (QueueSize <= 0)
                {
                    invalid |= true;
                    errorMsg += "Queue Size not in valid range\r\n";
                }
            }


            if (Type.GetType("Mono.Runtime") != null)
            {
                var lines = File.ReadAllLines(path + "msgsize_max");
                if (MessageSize <= 0 || int.Parse(lines[0]) < MessageSize)
                {
                    invalid |= true;
                    errorMsg += "Message Size not in valid range\r\n";
                }
            }
            else
            {
                if (MessageSize <= 0)
                {
                    invalid |= true;
                    errorMsg += "Message Size not in valid range\r\n";
                }
            }

            return !invalid;
        }

        public void Copy(POSIXConfig config)
        {
            QueueName = config.QueueName;
            AccessModifier = config.AccessModifier;
            QueueSize = config.QueueSize;
            MessageSize = config.MessageSize;
        }
    }
}
