using Kaitai;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RCP.Model
{
    public class Packet : IParameter
    {
        public RcpTypes.Command Command { get; set; }
        public uint Id { get; set; }
        public ulong Timestamp { get; set; }
        public dynamic Data { get; set; }

        public Packet(RcpTypes.Command command)
        {
            Command = command;
        }

        public static Packet Parse(KaitaiStream input)
        {
            // get command
            var command = (RcpTypes.Command)input.ReadU1();
            if (!Enum.IsDefined(typeof(RcpTypes.Command), command)) 
                throw new RCPDataErrorException();

            var packet = new Packet(command);

            // read packet options
            while (!input.IsEof)
            {
                var code = input.ReadU1();
                if (code == 0) // terminator
                    break;

                var property = (RcpTypes.Packet)code;
				if (!Enum.IsDefined(typeof(RcpTypes.Packet), property)) 
                	throw new RCPDataErrorException();

                switch (property)
                {
                    case RcpTypes.Packet.Data:
                        switch (command)
                        {
//	                        case RcpTypes.Command.Initialize:
//	                            // init - should not happen
//	                            throw new RCPDataErrorException();

                            case RcpTypes.Command.Add:
                            case RcpTypes.Command.Remove:
                            case RcpTypes.Command.Update:
                                // expect parameter
                                packet.Data = Parameter<dynamic>.Parse(input);
                                break;

                            case RcpTypes.Command.Version:
                                throw new RCPUnsupportedFeatureException();
                        }

                        break;

                    case RcpTypes.Packet.Id:
                        packet.Id = input.ReadU4be();
                        break;

                    case RcpTypes.Packet.Timestamp:
                        packet.Timestamp = input.ReadU8be();
                        break;
                    
                	default:
                        throw new RCPDataErrorException();
                }
            }

            return packet;
        }

        public void Write(BinaryWriter writer)
        {
            //command
            writer.Write((byte)Command);

        	//id
        	//timestamp
        	
            //data
        	if (Data != null)
        	{
            	writer.Write((byte)RcpTypes.Packet.Data);
            	Data.Write(writer);
        	}

            //terminate
            writer.Write((byte)0);
        }
    }
}