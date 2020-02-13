using System;
using System.IO;
using Kaitai;

using RCP.Exceptions;
using RCP.IO;
using RCP.Parameters;

namespace RCP.Protocol
{
    public class Packet : IWriteable
    {
        public RcpTypes.Command Command { get; set; }
        public ulong Timestamp { get; set; }
        public object Data { get; set; }

        public Packet(RcpTypes.Command command)
        {
            Command = command;
        }

        public static Packet Parse(KaitaiStream input, IParameterManager manager)
        {
            // get command
            var command = (RcpTypes.Command)input.ReadU1();
            if (!Enum.IsDefined(typeof(RcpTypes.Command), command)) 
                throw new RCPDataErrorException("Packet parsing: Unknown command: " + command.ToString());

            var packet = new Packet(command);

            if (command == RcpTypes.Command.Updatevalue)
            {
                // read id
                var id = input.ReadS2be();
                // read datatype
                var datatype = (RcpTypes.Datatype)input.ReadU1();
                //get parameter
                var parameter = manager.GetParameter(id);
                //read value
                parameter.ReadValue(input);
                packet.Data = parameter;
                return packet;
            }

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

                            case RcpTypes.Command.Remove:
                                // expect int16
                                packet.Data = input.ReadS2be();
                                break;
                            case RcpTypes.Command.Update:
                                // expect parameter
                                packet.Data = Parameter.Parse(input, manager);
                                break;
                            

                            case RcpTypes.Command.Info:
                                if (input.PeekChar() > 0)
                                    packet.Data = InfoData.Parse(input);
                                else
                                    packet.Data = null;
                                break;
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

            //timestamp

            var needsTerminator = true;
            //data
        	if (Data != null)
        	{
                if (Command == RcpTypes.Command.Updatevalue)
                {
                    var param = (Data as Parameter);
                    //id
                    writer.Write(param.Id, ByteOrder.BigEndian);
                    //datatype
                    writer.Write((byte)param.TypeDefinition.Datatype);
                    //value
                    param.WriteValue(writer, false);
                    needsTerminator = false;
                }
                else
                {
                    writer.Write((byte)RcpTypes.PacketOptions.Data);
                    if (Data is Parameter)
                        (Data as Parameter).Write(writer);
                    else if (Data is InfoData)
                        (Data as InfoData).Write(writer);
                    else if (Data is short)
                        writer.Write((short)Data, ByteOrder.BigEndian);
                }
            }

            //terminate
            if (needsTerminator)
                writer.Write((byte)0);
        }
    }
}