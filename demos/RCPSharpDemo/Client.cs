using System.Windows.Forms;

using RCP;
using RCP.Transporter;
using RCP.Parameter;
using RCP.Protocol;
using System.Collections.Generic;

namespace RCPSharpDemo
{
    public partial class Client : Form
    {
        RCPClient Carrot;
        Dictionary<byte[], IParameter> UIParams = new Dictionary<byte[], IParameter>(new StructuralEqualityComparer<byte[]>());

        public Client()
        {
            InitializeComponent();

            var transporter = new WebsocketClientTransporter();
            Carrot = new RCPClient();
            Carrot.SetTransporter(transporter);

            Carrot.ParameterAdded = (p) =>
            {
                UIParams.Add(p.Id, p);
                label1.Text = UIParams.Count.ToString() + ": " + p.Label;
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
                    case RcpTypes.ClientStatus.Disconnected: /*show something*/ break;
                    case RcpTypes.ClientStatus.Connected: Carrot.Initialize(); break;
                    case RcpTypes.ClientStatus.VersionMissmatch: MessageBox.Show(message); break;
                    case RcpTypes.ClientStatus.Ok: /*nop*/ break;
                }
            };

            transporter.Connect("127.0.0.1", 10000);
            //Carrot.Initialize();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // cleanup
            Carrot.Dispose();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            UIParams.Clear();
            label1.Text = "";
            Carrot.Initialize();
        }
    }
}
