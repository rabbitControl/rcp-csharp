using System;
using System.ComponentModel;
using System.IO;
using Kaitai;
using RCP.Protocol;
using RCP.Types;

namespace RCP.Parameters
{
    public class BangParameter: Parameter, INotifyPropertyChanged
    {
        bool FBangPending;

        public event Action OnBang;

        public BangParameter(Int16 id, IParameterManager manager)
            : base(id, manager, new BangDefinition())
        {
        }

        public void Bang()
        {
            SetChanged(ParameterChangedFlags.Value);
            FBangPending = true;
        }

        public override void ResetForInitialize()
        {
            base.ResetForInitialize();
            FBangPending = false;
        }

        protected override void WriteValue(BinaryWriter writer)
        {
            if (FBangPending)
            {
                writer.Write((byte)RcpTypes.ParameterOptions.Value);
                FBangPending = false;
            }
            base.WriteValue(writer);
        }

        protected override bool HandleOption(KaitaiStream input, RcpTypes.ParameterOptions option)
        {
            switch (option)
            {
                case RcpTypes.ParameterOptions.Value:
                    OnBang?.Invoke();
                    return true;
            }

            return false;
        }
    }
}
