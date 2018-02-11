using System;
using System.IO;
using Kaitai;

using RCP.Exceptions;
using RCP.Parameter;

namespace RCP.Protocol
{
    public class Packet : IWriteable
    {
        public RcpTypes.Command Command { get; set; }
        public uint Id { get; set; }
        public ulong Timestamp { get; set; }
        public IParameter Data { get; set; }

        public Packet(RcpTypes.Command command)
        {
            Command = command;
        }

        public static Packet Parse(KaitaiStream input)
        {
            // get command
            var command = (RcpTypes.Command)input.ReadU1();
            if (!Enum.IsDefined(typeof(RcpTypes.Command), command)) 
                throw new RCPDataErrorException("Packed parsing: Unknown command: " + command.ToString());

            var packet = new Packet(command);

            // read packet options
            while (!input.IsEof)
            {
                var code = input.ReadU1();
                if (code == 0) // terminator
                    break;

                var option = (RcpTypes.PacketOptions)code;
				if (!Enum.IsDefined(typeof(RcpTypes.PacketOptions), option)) 
                	throw new RCPDataErrorException("Packet parsing: Unknown option: " + option.ToString());

                switch (option)
                {
                    case RcpTypes.PacketOptions.Data:
                        switch (command)
                        {
//	                        case RcpTypes.Command.Initialize:
//	                            // init - should not happen
//	                            throw new RCPDataErrorException();

                            case RcpTypes.Command.Add:
                            case RcpTypes.Command.Remove:
                            case RcpTypes.Command.Update:
                                // expect parameter
                                packet.Data = RCP.Parameter.Parameter.Parse(input);
                                break;

                            case RcpTypes.Command.Version:
                                throw new RCPUnsupportedFeatureException();
                        }

                        break;

                    case RcpTypes.PacketOptions.Timestamp:
                        packet.Timestamp = input.ReadU8be();
                        break;
                    
                	default:
                        throw new RCPUnsupportedFeatureException();
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
            	writer.Write((byte)RcpTypes.PacketOptions.Data);
            	Data.Write(writer);
        	}

            //terminate
            writer.Write((byte)0);
        }
    }
}