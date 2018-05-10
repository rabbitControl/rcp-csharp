using System;

using Kaitai;
using RCP.Protocol;
using System.IO;
using RCP.Exceptions;
using System.Collections.Generic;

namespace RCP.Parameter
{
    internal class EnumParameter : ValueParameter<string>, IEnumParameter
    {
        private bool FEntriesChanged;
        private string[] FEntries;
        public string[] Entries { get { return FEntries; } set { FEntries = value; FEntriesChanged = true; SetDirty(); } }

        public EnumParameter(Int16 id, IParameterManager manager) : 
            base (id, RcpTypes.Datatype.Enum, manager)
        { }

        protected override bool AnyChanged()
        {
            return base.AnyChanged() || FEntriesChanged;
        }

        public override void ResetForInitialize()
        {
            base.ResetForInitialize();

            FValueChanged = Value != "";
            FDefaultChanged = Default != "";
            FEntriesChanged = FEntries.Length != 0;
        }

        public override string ReadValue(KaitaiStream input)
        {
            return new RcpTypes.TinyString(input).Data;
        }

        public override void WriteValue(BinaryWriter writer, string value)
        {
            RcpTypes.TinyString.Write(value, writer);
        }

        protected override void WriteTypeDefinitionOptions(BinaryWriter writer)
        {
            base.WriteTypeDefinitionOptions(writer);

            if (FEntriesChanged)
            {
                writer.Write((byte)RcpTypes.EnumOptions.Entries);
                foreach (var entry in Entries)
                    RcpTypes.TinyString.Write(entry, writer);
                writer.Write((byte)0);

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
