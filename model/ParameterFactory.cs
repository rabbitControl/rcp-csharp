using RCP.Model;
using System.Drawing;
using System.Numerics;

using RCP.Protocol;
using RCP.Exceptions;
using RCP.Parameter;

namespace RCP
{
    public static class ParameterFactory
    {
        public static IParameter CreateParameter(byte[] id, RcpTypes.Datatype datatype)
        {
            switch (datatype)
            {
                case RcpTypes.Datatype.Boolean:
                    return new BooleanParameter(id);

                case RcpTypes.Datatype.Enum:
                    return new EnumParameter(id);

                case RcpTypes.Datatype.Int32:
                    return new NumberParameter<int>(id, new Integer32Definition());

                case RcpTypes.Datatype.Float32:
                    return new NumberParameter<float>(id, new Float32Definition());

                case RcpTypes.Datatype.String:
                    return new StringParameter(id);

                case RcpTypes.Datatype.Uri:
                    return new UriParameter(id);

                case RcpTypes.Datatype.Rgba:
                    return new RGBAParameter(id);

                case RcpTypes.Datatype.Vector2f32:
                    return new NumberParameter<Vector2>(id, new Vector2f32Definition());

                case RcpTypes.Datatype.Vector3f32:
                    return new NumberParameter<Vector3>(id, new Vector3f32Definition());

                default: throw new RCPUnsupportedFeatureException();
                //group
                //array
            }
        }
    	
    	public static IParameter CreateParameter(byte[] id, ITypeDefinition typeDefinition)
        {
            switch (typeDefinition.Datatype)
            {
                case RcpTypes.Datatype.Boolean:
                    return new BooleanParameter(id, typeDefinition as IBooleanDefinition);

                case RcpTypes.Datatype.Enum:
                    return new EnumParameter(id, typeDefinition as IEnumDefinition);

                case RcpTypes.Datatype.Int32:
                    return new NumberParameter<int>(id, typeDefinition as INumberDefinition<int>);

                case RcpTypes.Datatype.Float32:
                    return new NumberParameter<float>(id, typeDefinition as INumberDefinition<float>);

                case RcpTypes.Datatype.String:
                    return new StringParameter(id, typeDefinition as IStringDefinition);

                case RcpTypes.Datatype.Uri:
                    return new UriParameter(id, typeDefinition as IUriDefinition);

                case RcpTypes.Datatype.Rgba:
                    return new RGBAParameter(id, typeDefinition as IRGBADefinition);

                case RcpTypes.Datatype.Vector2f32:
                    return new NumberParameter<Vector2>(id, typeDefinition as INumberDefinition<Vector2>);

                case RcpTypes.Datatype.Vector3f32:
                    return new NumberParameter<Vector3>(id, typeDefinition as INumberDefinition<Vector3>);

                default: throw new RCPUnsupportedFeatureException();
                //group
                //array
            }
        }

        public static IParameter CreateArrayParameter(byte[] id, RcpTypes.Datatype datatype, uint length)
        {
            switch (datatype)
            {
                case RcpTypes.Datatype.Boolean:
                    return new ArrayParameter<bool>(id, new ArrayDefinition<bool>(new BooleanDefinition(), length));

                case RcpTypes.Datatype.Enum:
                    return new ArrayParameter<ushort>(id, new ArrayDefinition<ushort>(new EnumDefinition(), length));

                case RcpTypes.Datatype.Int32:
                    return new ArrayParameter<int>(id, new ArrayDefinition<int>(new Integer32Definition(), length));

                case RcpTypes.Datatype.Float32:
                    return new ArrayParameter<float>(id, new ArrayDefinition<float>(new Float32Definition(), length));

                case RcpTypes.Datatype.String:
                    return new ArrayParameter<string>(id, new ArrayDefinition<string>(new StringDefinition(), length));

                case RcpTypes.Datatype.Uri:
                    return new ArrayParameter<string>(id, new ArrayDefinition<string>(new UriDefinition(), length));

                case RcpTypes.Datatype.Rgba:
                    return new ArrayParameter<Color>(id, new ArrayDefinition<Color>(new RGBADefinition(), length));

                case RcpTypes.Datatype.Vector2f32:
                    return new ArrayParameter<Vector2>(id, new ArrayDefinition<Vector2>(new Vector2f32Definition(), length));

                case RcpTypes.Datatype.Vector3f32:
                    return new ArrayParameter<Vector3>(id, new ArrayDefinition<Vector3>(new Vector3f32Definition(), length));

                default: throw new RCPUnsupportedFeatureException();
            }
        }
    	
    	public static IParameter CreateArrayParameter<T>(byte[] id, ArrayDefinition<T> definition)
        {
            return new ArrayParameter<T>(id, definition);
        }
    }
}
