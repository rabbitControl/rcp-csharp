using Kaitai;
using System.IO;

using RCP.Protocol;

namespace RCP.Parameter
{
    public class GroupParameter : Parameter
    {
        public GroupParameter(int id): 
            base (id, new GroupDefinition())
        { }

        public void Write(BinaryWriter writer)
        {
            //mandatory
            writer.Write(Id, ByteOrder.BigEndian);

            //terminate
            writer.Write((byte)0);
        }

        protected override void WriteValue(BinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }
    }
}
