using System;
using System.IO;
using Kaitai;

using RCP.Protocol;
using RCP.Exceptions;
using System.Collections.Generic;
using RCP.Parameters;
using System.Linq;

namespace RCP.Types
{
    public class EnumDefinition : DefaultDefinition<string>, IEnumDefinition
    {
        private string[] FEntries = Array.Empty<string>();
        public string[] Entries
        {
            get { return FEntries; }
            set
            {
                if (!FEntries.SequenceEqual(value))
                {
                    FEntries = value;
                    SetChanged(TypeChangedFlags.EnumEntires);
                }
            }
        }

        private bool FMultiSelect;
        public bool MultiSelect
        {
            get { return FMultiSelect; }
            set
            {
                if (FMultiSelect != value)
                {
                    FMultiSelect = value;
                    SetChanged(TypeChangedFlags.EnumMultiSelect);
                }
            }
        }

        public EnumDefinition()
            : base(RcpTypes.Datatype.Enum, string.Empty)
        {
        }

        public override Parameter CreateParameter(short id, IParameterManager manager) => new EnumParameter(id, manager, this);

        public override void ResetForInitialize()
        {
            if (FEntries.Length != 0)
                SetChanged(TypeChangedFlags.EnumEntires);
            if (FMultiSelect != false)
                SetChanged(TypeChangedFlags.EnumMultiSelect);
            base.ResetForInitialize();
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
            if (IsChanged(TypeChangedFlags.EnumEntires))
            {
                writer.Write((byte)RcpTypes.EnumOptions.Entries);
                foreach (var entry in Entries)
                    RcpTypes.TinyString.Write(entry, writer);
                writer.Write((byte)0);
            }

            if (IsChanged(TypeChangedFlags.EnumMultiSelect))
            {
                writer.Write((byte)RcpTypes.EnumOptions.Multiselect);
                foreach (var entry in Entries)
                    RcpTypes.TinyString.Write(entry, writer);
                writer.Write((byte)0);
            }

            base.WriteOptions(writer);
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
    }
}