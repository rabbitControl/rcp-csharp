using System;
using System.IO;
using Kaitai;

using RCP.Protocol;
using RCP.Exceptions;
using System.Collections.Generic;

namespace RCP.Parameter
{
    public class EnumDefinition : DefaultDefinition<string>, IEnumDefinition
    {
        private bool FEntriesChanged;
        private string[] FEntries;
        public string[] Entries { get { return FEntries; } set { FEntriesChanged = FEntries != value;  FEntries = value; } }

        private bool FMultiSelectChanged;
        private bool FMultiSelect;
        public bool MultiSelect { get { return FMultiSelect; } set { FMultiSelectChanged = FMultiSelect != value; FMultiSelect = value; } }

        public EnumDefinition()
        : base(RcpTypes.Datatype.Enum)
        { }

        public override bool AnyChanged()
        {
            return FEntriesChanged || FMultiSelectChanged;
        }

        public override void ResetForInitialize()
        {
            FEntriesChanged = FEntries.Length != 0;
            FMultiSelectChanged = FMultiSelect != false;
        }

        public override string ReadValue(KaitaiStream input)
        {
            return new RcpTypes.TinyString(input).Data;
        }

        public override void WriteValue(BinaryWriter writer, string value)
        {
            RcpTypes.TinyString.Write(value, writer);
        }

        protected override void WriteOptions(BinaryWriter writer)
        {
            base.WriteOptions(writer);

            if (FEntriesChanged)
            {
                writer.Write((byte)RcpTypes.EnumOptions.Entries);
                foreach (var entry in Entries)
                    RcpTypes.TinyString.Write(entry, writer);
                writer.Write((byte)0);

                FEntriesChanged = false;
            }

            if (FMultiSelectChanged)
            {
                writer.Write((byte)RcpTypes.EnumOptions.Multiselect);
                foreach (var entry in Entries)
                    RcpTypes.TinyString.Write(entry, writer);
                writer.Write((byte)0);

                FMultiSelectChanged = false;
            }
        }

        protected override bool HandleOption(KaitaiStream input, byte code)
        {
            var option = (RcpTypes.EnumOptions)code;
            if (!Enum.IsDefined(typeof(RcpTypes.EnumOptions), option))
                throw new RCPDataErrorException("EnumDefinition parsing: Unknown option: " + option.ToString());

            switch (option)
            {
                case RcpTypes.EnumOptions.Default:
                    Default = ReadValue(input);
                    return true;

                case RcpTypes.EnumOptions.Multiselect:
                    MultiSelect = input.ReadU1() > 0;
                    return true;

                case RcpTypes.EnumOptions.Entries:
                    var entries = new List<string>();
                    while (input.PeekChar() > 0)
                        entries.Add(new RcpTypes.TinyString(input).Data);
                    input.ReadByte(); //0 terminator
                    Entries = entries.ToArray();
                    return true;
            }

            return false;
        }

        public override void CopyTo(ITypeDefinition other)
        {
            base.CopyTo(other);

            var otherUri = other as EnumDefinition;

            if (FEntriesChanged)
                otherUri.Entries = FEntries;

            if (FMultiSelectChanged)
                otherUri.MultiSelect = FMultiSelect;
        }
    }
}