using Kaitai;
using RCP.Protocol;
using System;
using System.Collections.Generic;

namespace RCP.Parameter
{
    internal class GroupParameter : Parameter, IGroupParameter
    {
        private List<IParameter> FParams = new List<IParameter>();
        private List<IParameter> FAddedParams = new List<IParameter>();
        private List<IParameter> FRemovedParams = new List<IParameter>();

        public GroupParameter(Int16 id, IParameterManager manager): 
            base (id, manager)
        {
            TypeDefinition = new GroupDefinition();
        }

        public void AddParameter(IParameter param)
        {
            (param as Parameter).SetParent(this);
        }

        //protected override void ParseTypeDefinitionOptions(KaitaiStream input)
        //{
        //    //read the terminator
        //    input.ReadU1();
        //}

        protected override bool HandleOption(KaitaiStream input, RcpTypes.ParameterOptions option)
        {
            return false;
        }
    }
}
