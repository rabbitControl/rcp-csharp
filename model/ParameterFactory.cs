using RCP.Model;
using System.Drawing;
using System.Numerics;

using RCP.Protocol;
using RCP.Exceptions;
using RCP.Parameter;
using System;

namespace RCP
{
    internal static class ParameterFactory
    {
        public static IParameter CreateParameter(Int16 id, RcpTypes.Datatype datatype, IManager manager)
        {
            switch (datatype)
            {
                case RcpTypes.Datatype.Boolean:
                    return new BooleanParameter(id, manager);

                case RcpTypes.Datatype.Enum:
                    return new EnumParameter(id, manager);

                case RcpTypes.Datatype.Int32:
                    return new NumberParameter<int>(id, new Integer32Definition(), manager);

                case RcpTypes.Datatype.Float32:
                    return new NumberParameter<float>(id, new Float32Definition(), manager);

                case RcpTypes.Datatype.String:
                    return new StringParameter(id, manager);

                case RcpTypes.Datatype.Uri:
                    return new UriParameter(id, manager);

                case RcpTypes.Datatype.Rgba:
                    return new RGBAParameter(id, manager);

                case RcpTypes.Datatype.Vector2f32:
                    return new NumberParameter<Vector2>(id, new Vector2f32Definition(), manager);

                case RcpTypes.Datatype.Vector3f32:
                    return new NumberParameter<Vector3>(id, new Vector3f32Definition(), manager);

                case RcpTypes.Datatype.Group:
                    return new GroupParameter(id, manager);

                default: throw new RCPUnsupportedFeatureException();
                //group
                //array
            }
        }
    	
    	public static IParameter CreateParameter(Int16 id, IManager manager, ITypeDefinition typeDefinition)
        {
            switch (typeDefinition.Datatype)
            {
                case RcpTypes.Datatype.Boolean:
                    return new BooleanParameter(id, typeDefinition as IBooleanDefinition, manager);

                case RcpTypes.Datatype.Enum:
                    return new EnumParameter(id, typeDefinition as IEnumDefinition, manager);

                case RcpTypes.Datatype.Int32:
                    return new NumberParameter<int>(id, typeDefinition as INumberDefinition<int>, manager);

                case RcpTypes.Datatype.Float32:
                    return new NumberParameter<float>(id, typeDefinition as INumberDefinition<float>, manager);

                case RcpTypes.Datatype.String:
                    return new StringParameter(id, typeDefinition as IStringDefinition, manager);

                case RcpTypes.Datatype.Uri:
                    return new UriParameter(id, typeDefinition as IUriDefinition, manager);

                case RcpTypes.Datatype.Rgba:
                    return new RGBAParameter(id, typeDefinition as IRGBADefinition, manager);

                case RcpTypes.Datatype.Vector2f32:
                    return new NumberParameter<Vector2>(id, typeDefinition as INumberDefinition<Vector2>, manager);

                case RcpTypes.Datatype.Vector3f32:
                    return new NumberParameter<Vector3>(id, typeDefinition as INumberDefinition<Vector3>, manager);

                default: throw new RCPUnsupportedFeatureException();
                //group
                //array
            }
        }

     //   public static IParameter CreateArrayParameter(Int16 id, RcpTypes.Datatype datatype, uint length)
     //   {
     //       switch (datatype)
     //       {
     //           case RcpTypes.Datatype.Boolean:
     //               return new ArrayParameter<bool>(id, new ArrayDefinition<bool>(new BooleanDefinition(), length));

     //           case RcpTypes.Datatype.Enum:
     //               return new ArrayParameter<ushort>(id, new ArrayDefinition<ushort>(new EnumDefinition(), length));

     //           case RcpTypes.Datatype.Int32:
     //               return new ArrayParameter<int>(id, new ArrayDefinition<int>(new Integer32Definition(), length));

     //           case RcpTypes.Datatype.Float32:
     //               return new ArrayParameter<float>(id, new ArrayDefinition<float>(new Float32Definition(), length));

     //           case RcpTypes.Datatype.String:
     //               return new ArrayParameter<string>(id, new ArrayDefinition<string>(new StringDefinition(), length));

     //           case RcpTypes.Datatype.Uri:
     //               return new ArrayParameter<string>(id, new ArrayDefinition<string>(new UriDefinition(), length));

     //           case RcpTypes.Datatype.Rgba:
     //               return new ArrayParameter<Color>(id, new ArrayDefinition<Color>(new RGBADefinition(), length));

     //           case RcpTypes.Datatype.Vector2f32:
     //               return new ArrayParameter<Vector2>(id, new ArrayDefinition<Vector2>(new Vector2f32Definition(), length));

     //           case RcpTypes.Datatype.Vector3f32:
     //               return new ArrayParameter<Vector3>(id, new ArrayDefinition<Vector3>(new Vector3f32Definition(), length));

     //           default: throw new RCPUnsupportedFeatureException();
     //       }
     //   }
    	
    	//public static IParameter CreateArrayParameter<T>(Int16 id, ArrayDefinition<T> definition)
     //   {
     //       return new ArrayParameter<T>(id, definition);
     //   }
    }
}
