using System;

using Kaitai;
using RCP.Protocol;
using System.IO;
using RCP.Exceptions;
using System.Collections.Generic;

namespace RCP.Parameter
{
    public class EnumParameter : ValueParameter<ushort>
    {
        private bool FEntriesChanged;
        private string[] FEntries;
        public string[] Entries { get { return FEntries; } set { FEntries = value; FEntriesChanged = true; } }

        public EnumParameter(Int16 id, IManager manager) : 
            base (id, RcpTypes.Datatype.Enum, manager)
        { }

        public override ushort ReadValue(KaitaiStream input)
        {
            return input.ReadU2be();
        }

        public override void WriteValue(BinaryWriter writer, ushort value)
        {
            writer.Write(value, ByteOrder.BigEndian);
        }

        protected override void WriteTypeDefinitionOptions(BinaryWriter writer)
        {
            base.WriteTypeDefinitionOptions(writer);

            if (FEntriesChanged)
            {
                writer.Write((byte)RcpTypes.EnumOptions.Entries);
                ushort entryCount = (ushort)Entries.Length;
                writer.Write(entryCount, ByteOrder.BigEndian);

                foreach (var entry in Entries)
                    RcpTypes.TinyString.Write(entry, writer);

                FEntriesChanged = false;
            }
        }

        protected override bool HandleTypeDefinitionOption(KaitaiStream input, byte code)
        {
            var option = (RcpTypes.EnumOptions)code;
            if (!Enum.IsDefined(typeof(RcpTypes.EnumOptions), option))
                throw new RCPDataErrorException("EnumDefinition parsing: Unknown option: " + option.ToString());

            switch (option)
            {
                case RcpTypes.EnumOptions.Default:
                    Default = ReadValue(input);
                    return true;

                case RcpTypes.EnumOptions.Entries:
                    var entries = new List<string>();
                    var entryCount = input.ReadU2be();
                    for (int i = 0; i < entryCount; i++)
                        entries.Add(new RcpTypes.TinyString(input).Data);
                    Entries = entries.ToArray();
                    return true;
            }

            return false;
        }
    }
}
