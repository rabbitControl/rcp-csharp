using System;
using System.Linq;
using RCP.Protocol;
using System.IO;
using Kaitai;
using RCP.Exceptions;

namespace RCP.Protocol
{
    public class Widget
    {
        public RcpTypes.Widgettype Type { get; set; }

        public Widget(RcpTypes.Widgettype type)
        {
            Type = type;
        }

        public virtual void Write(BinaryWriter writer)
        {
            writer.Write((byte)Type);

            //write type specific stuff
            WriteOptions(writer);

            //terminate
            writer.Write((byte)0);
        }

        protected virtual void WriteOptions(BinaryWriter writer)
        { }

        protected virtual void ParseOptions(KaitaiStream input)
        {
            while (true)
            {
                var code = input.ReadU1();
                if (code == 0)
                    break;
            }
        }

        public static Widget Parse(KaitaiStream input)
        {
            var widgettype = (RcpTypes.Widgettype)input.ReadU1();
            if (!Enum.IsDefined(typeof(RcpTypes.Widgettype), widgettype))
                throw new RCPDataErrorException("Widget parsing: Unknown widget!");

            Widget widget = null;

            switch (widgettype)
            {
                case RcpTypes.Widgettype.Bang:
                    {
                        widget = new BangWidget();
                        break;
                    }

                case RcpTypes.Widgettype.Press:
                    {
                        widget = new PressWidget();
                        break;
                    }

                case RcpTypes.Widgettype.Toggle:
                    {
                        widget = new ToggleWidget();
                        break;
                    }

                case RcpTypes.Widgettype.Slider:
                    {
                        widget = new SliderWidget();
                        break;
                    }

                case RcpTypes.Widgettype.Endless:
                    {
                        widget = new EndlessWidget();
                        break;
                    }
            }

            widget.ParseOptions(input);
            return widget;
        }
    }

    public class BangWidget: Widget
    {
        public BangWidget()
        : base(RcpTypes.Widgettype.Bang)
        { }
    }

    public class PressWidget : Widget
    {
        public PressWidget()
        : base(RcpTypes.Widgettype.Press)
        { }
    }

    public class ToggleWidget : Widget
    {
        public ToggleWidget()
        : base(RcpTypes.Widgettype.Toggle)
        { }
    }

    public class SliderWidget : Widget
    {
        public SliderWidget()
        : base(RcpTypes.Widgettype.Slider)
        { }
    }

    public class EndlessWidget : Widget
    {
        public EndlessWidget()
        : base(RcpTypes.Widgettype.Endless)
        { }
    }
}