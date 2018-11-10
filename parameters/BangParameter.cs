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

        public event EventHandler OnBang;

        public BangParameter(Int16 id, IParameterManager manager)
            : base(id, manager, new BangDefinition())
        {
            (TypeDefinition as BangDefinition).OnBang += ForwardBang;
        }

        private void ForwardBang()
        {
            OnBang?.Invoke(this, null);
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

        //protected override void WriteValue(BinaryWriter writer)
        //{
        //    if (FBangPending)
        //    {
        //        writer.Write((byte)RcpTypes.ParameterOptions.Value);
        //        FBangPending = false;
        //    }
        //    base.WriteValue(writer);
        //}
    }
}
