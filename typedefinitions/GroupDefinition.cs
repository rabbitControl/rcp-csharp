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

        public override void CopyFrom(ITypeDefinition other)
        {
            //throw new NotImplementedException();
        }
    }
}
