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
        public bool EntriesChanged { get; private set; }
        private string[] FEntries;
        public string[] Entries { get { return FEntries; } set { if (FEntries != value) { EntriesChanged = true; FEntries = value; } } }

        public bool MultiSelectChanged { get; private set; }
        private bool FMultiSelect;
        public bool MultiSelect { get { return FMultiSelect; } set { if (FMultiSelect != value) { MultiSelectChanged = true; FMultiSelect = value; } } }

        public EnumDefinition()
        : base(RcpTypes.Datatype.Enum)
        { }

        public override bool AnyChanged()
        {
            return base.AnyChanged() || EntriesChanged || MultiSelectChanged;
        }

        public override void ResetForInitialize()
        {
            EntriesChanged = FEntries.Length != 0;
            MultiSelectChanged = FMultiSelect != false;
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

            if (EntriesChanged)
            {
                writer.Write((byte)RcpTypes.EnumOptions.Entries);
                foreach (var entry in Entries)
                    RcpTypes.TinyString.Write(entry, writer);
                writer.Write((byte)0);

                EntriesChanged = false;
            }

            if (MultiSelectChanged)
            {
                writer.Write((byte)RcpTypes.EnumOptions.Multiselect);
                foreach (var entry in Entries)
                    RcpTypes.TinyString.Write(entry, writer);
                writer.Write((byte)0);

                MultiSelectChanged = false;
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

        public override void CopyFrom(ITypeDefinition other)
        {
            base.CopyFrom(other);

            var otherEnum = other as IEnumDefinition;

            if (otherEnum.EntriesChanged)
                FEntries = otherEnum.Entries;

            if (otherEnum.MultiSelectChanged)
                FMultiSelect = otherEnum.MultiSelect;
        }
    }
}