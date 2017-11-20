using RCP.Model;
using System.Drawing;
using System.Numerics;

namespace RCP
{
    public static class ParameterFactory
    {
        public static IParameter CreateParameter(uint id, RcpTypes.Datatype datatype)
        {
            switch (datatype)
            {
                case RcpTypes.Datatype.Boolean:
                    return new BooleanParameter(id);

                case RcpTypes.Datatype.Int32:
                    return new NumberParameter<int>(id, new Integer32Definition());

                case RcpTypes.Datatype.Float32:
                    return new NumberParameter<float>(id, new Float32Definition());

                case RcpTypes.Datatype.String:
                    return new StringParameter(id);

                case RcpTypes.Datatype.Rgba:
                    return new RGBAParameter(id);

                //group
                //array
            }

            return null;
        }

        public static IParameter CreateArrayParameter(uint id, RcpTypes.Datatype datatype, uint length)
        {
            return new ArrayParameter<float>(id, new ArrayDefinition<float>(new Float32Definition(), length));
        }
    	
    	public static IParameter CreateArrayParameter<T>(uint id, ArrayDefinition<T> definition)
        {
            return new ArrayParameter<T>(id, definition);
        }
    }
}
