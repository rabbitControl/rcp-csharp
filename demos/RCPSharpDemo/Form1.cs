using System.Windows.Forms;

using RCP;
using RCP.Transporter;
using RCP.Parameter;
using RCP.Protocol;
using System.Collections.Generic;

namespace RCPSharpDemo
{
    public partial class Form1 : Form
    {
        RCPServer Rabbit;
        RCPClient Carrot;
        Dictionary<int, IParameter> UIParams = new Dictionary<int, IParameter>();

        public Form1()
        {
            InitializeComponent();

            Rabbit = new RCPServer();

            Rabbit.AddTransporter(new UDPServerTransporter("127.0.0.1", 9000, 9001));
            Rabbit.AddTransporter(new WebsocketServerTransporter("127.0.0.1", 10000));

            int id = 123;
            var param = ParameterFactory.CreateParameter(id, RcpTypes.Datatype.Float32);
            //param now is of type NumberParameter<float>
            //holds param.TypeDefinition of type NumberDefinition<float>
            param.Label = "My Float";
            //param.Value = 0.5f;
            //param.TypeDefinition.Minimum = -1.0f;
            //param.TypeDefinition.Maximum = 1.0f;

            //var paramGroup = ParameterFactory.CreateParameterGroup(1);
            //paramGroup.addChild(param);

            ////expose parameter
            Rabbit.AddParameter(param); //sends add to all clients

            //update a parameter value
            //param.Value = 0.2f;
            //update label
            param.Label = "My better Float";
            //update minimum
            //param.TypeDefinition.Minimum = 0f;

            Rabbit.UpdateParameter(param); // this sends an update to all clients

            //listen for value updates on the parameter
            Rabbit.ParameterValueUpdated = (p) =>
            {
                //Log(p.Value);
            };

            //listen for any changes on the parameter
            //some of the options of the parameter will be updated but since we don't know which ones, we'll have to take over all of them
            Rabbit.ParameterUpdated = (p) =>
            {
                //get application object from a list
                //var appObj = AppObjects[p.Id];

                //update application object
                //appObj.Label = p.Label;
                //...
            };

            //remove parameter
            //Rabbit.RemoveParameter(param.Id);

            //----------------------------------------------------

            Carrot = new RCPClient();
            Carrot.SetTransporter(new UDPClientTransporter("127.0.0.1", 9001, 9000));

            Carrot.ParameterAdded = (p) =>
            {
                UIParams.Add(p.Id, p);
            };

            //listen for any parameter changes
            Carrot.ParameterUpdated = (p) =>
            {
                var uip = UIParams[p.Id];
                uip.Label = p.Label;
                //...
            };

            //listen for any value changes
            Carrot.ParameterValueUpdated = (p) =>
            {
                var uip = UIParams[p.Id];
                switch (p.TypeDefinition.Datatype)
                {
                    //case RcpTypes.Datatype.Float32: uip.Value = (float)p.Value; break;
                        //...
                }
            };

            Carrot.ParameterRemoved = (p) =>
            {
                //remove UI matching p
            };

            Carrot.StatusChanged = (status, message) =>
            {
                // e.g.:
                switch (status)
                {
                    case RcpTypes.Status.NoTransporter: /*show something*/ break;
                    case RcpTypes.Status.NotConnected: /*show something*/ break;
                    case RcpTypes.Status.Connected: Carrot.Initialize(); break;
                    case RcpTypes.Status.VersionMissmatch: MessageBox.Show(message); break;
                    case RcpTypes.Status.Ok: /*nop*/ break;
                }
            };

            Carrot.Initialize();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // cleanup
            Carrot.Dispose();
            Rabbit.Dispose();
        }
    }
}
