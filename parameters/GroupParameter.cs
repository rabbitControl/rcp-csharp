using System;
using System.Collections.Generic;
using System.IO;

namespace RCP.Parameter
{
    public class GroupParameter : Parameter, IGroupParameter
    {
        private List<IParameter> FParams = new List<IParameter>();
        private List<IParameter> FAddedParams = new List<IParameter>();
        private List<IParameter> FRemovedParams = new List<IParameter>();

        public GroupParameter(Int16 id, IManager manager): 
            base (id, new GroupDefinition(), manager)
        { }

        protected override void WriteValue(BinaryWriter writer)
        {
            //groups have no value!
        }

        public void AddParameter(IParameter param)
        {
            (param as Parameter).SetParent(this);
        }

        public void RemoveParameter(IParameter param)
        {
            (param as Parameter).Destroy();
        }
    }
}
