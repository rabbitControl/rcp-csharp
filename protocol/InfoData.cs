using System;
using System.IO;

using Kaitai;
using RCP.Exceptions;

namespace RCP.Protocol
{
    public class InfoData : IWriteable
    {
        public string Version { get; set; }
        public string ApplicationId { get; set; }

        public InfoData (string version, string applicationId = "")
        {
            Version = version;
            ApplicationId = applicationId;
        }

        public void Write(BinaryWriter writer)
        {
            //mandatory
            RcpTypes.TinyString.Write(Version, writer);

            if (!string.IsNullOrEmpty(ApplicationId))
            {
                writer.Write((byte)RcpTypes.InfodataOptions.Applicationid);
                RcpTypes.TinyString.Write(ApplicationId, writer);
            }

            //terminate
            writer.Write((byte)0);
        }

        public static InfoData Parse(KaitaiStream input)
        {
            // get mandatory version
            var version = new RcpTypes.TinyString(input).Data;
            var appId = "";

            // get options from the stream
            while (true)
            {
                var code = input.ReadU1();
                if (code == 0)
                    break;

                var option = (RcpTypes.InfodataOptions)code;
                if (!Enum.IsDefined(typeof(RcpTypes.InfodataOptions), option))
                    throw new RCPDataErrorException("InfoData parsing: Unknown option: " + option.ToString());

                switch (option)
                {
                    case RcpTypes.InfodataOptions.Applicationid:
                        appId = new RcpTypes.TinyString(input).Data;
                        break;
                }
            }

            return new InfoData(version, appId);
        }
    }
}
