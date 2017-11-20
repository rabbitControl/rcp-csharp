using System.IO;
using Kaitai;

namespace RCP.Model
{
    public class GroupDefinition : TypeDefinition
    {
        public GroupDefinition()
        : base(RcpTypes.Datatype.Group)
        { }
    }
}
