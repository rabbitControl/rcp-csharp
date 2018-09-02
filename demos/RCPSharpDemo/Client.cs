using System.Windows.Forms;

using RCP;
using RCP.Transporter;
using RCP.Parameters;
using System;
using System.Collections.Generic;

using RCP.Protocol;

namespace RCPSharpDemo
{
    public partial class Client : Form
    {
        RCPClient Carrot;
        Dictionary<Int16, IParameter> UIParams = new Dictionary<Int16, IParameter>();

        public Client()
        {
            InitializeComponent();

            var transporter = new WebsocketClientTransporter();
            Carrot = new RCPClient();
            Carrot.SetTransporter(transporter);

            Carrot.ParameterAdded += (s, p) =>
            {
                UIParams.Add(p.Id, p);
                label1.Text = UIParams.Count.ToString() + ": " + p.Label;

                p.Updated += P_Updated;
            };

            Carrot.ParameterRemoved += (s, p) =>
            {
                //remove UI matching p
                UIParams.Remove(p.Id);
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
        }

        private void P_Updated(object sender, EventArgs e)
        {
            if (sender is NumberParameter<int>)
                label1.Text = UIParams.Count.ToString() + ": " + (sender as NumberParameter<int>).Value.ToString();
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
