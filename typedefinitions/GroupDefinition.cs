using System;
using System.IO;
using Kaitai;

using RCP.Protocol;

namespace RCP.Parameter
{                           
    public class GroupDefinition : TypeDefinition
    {
        public GroupDefinition()
        : base(RcpTypes.Datatype.Group)
        {
            
        }

        public override Type ClrType => null;

        public override Parameter CreateParameter(short id, IParameterManager manager) => new GroupParameter(id, manager, this);

        public override void CopyFrom(ITypeDefinition other)
        {
            //throw new NotImplementedException();
        }

        public override TypeDefinition CreateArray(int[] structure)
        {
            throw new NotSupportedException();
        }

        public override TypeDefinition CreateRange()
        {
            throw new NotSupportedException();
        }
    }
}
