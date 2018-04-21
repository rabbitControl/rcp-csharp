using System;
using System.IO;

namespace RCP.Parameter
{
    public class GroupParameter : Parameter
    {
        public GroupParameter(Int16 id): 
            base (id, new GroupDefinition())
        { }

        public void Write(BinaryWriter writer)
        {
            //mandatory
            writer.Write(Id);

            //terminate
            writer.Write((byte)0);
        }

        protected override void WriteValue(BinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }
    }
}
