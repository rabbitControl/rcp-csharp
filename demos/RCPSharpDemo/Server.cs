using RCP;
using RCP.Parameter;
using RCP.Protocol;
using RCP.Transporter;
using System.Drawing;
using System.Windows.Forms;

namespace RCPSharpDemo
{
    public partial class Server : Form
    {
        RCPServer FRabbit;
        Client FClient;

        public Server()
        {
            InitializeComponent();

            FRabbit = new RCPServer();

            //Rabbit.AddTransporter(new UDPServerTransporter("127.0.0.1", 4568, 4567));
            FRabbit.AddTransporter(new WebsocketServerTransporter("127.0.0.1", 10000));

            //the client
            FClient = new Client();
            FClient.Show();
        }

        private void Server_FormClosing(object sender, FormClosingEventArgs e)
        {
            FClient.Dispose();
            FRabbit.Dispose();
        }
        
        private void button1_Click(object sender, System.EventArgs e)
        {
            var group = FRabbit.CreateGroup();
            group.Label = "foo";

            var param = FRabbit.CreateNumberParameter<float>();
            param.Label = "My Flöat: " + param.Id;
            param.Order = param.Id;
            param.Widget = new SliderWidget();
            param.Value = 2.5f;
            param.TypeDefinition.Minimum = -10.0f;
            param.TypeDefinition.Maximum = 10.0f;
            param.ValueUpdated += Param_ValueUpdated;
            group.AddParameter(param);

            var nt = FRabbit.CreateNumberParameter<int>();
            nt.Label = "integer";
            nt.Value = 7;
            nt.TypeDefinition.Minimum = -10;
            nt.TypeDefinition.Maximum = 10;
            group.AddParameter(nt);

            var str = FRabbit.CreateStringParameter();
            str.Label = "string";
            str.Value = "foobar";
            group.AddParameter(str);

            var clr = FRabbit.CreateRGBAParameter();
            clr.Label = "color";
            clr.Value = Color.Red;
            group.AddParameter(clr);

            FRabbit.Root.AddParameter(group);

            FRabbit.Update();
        }

        private void Param_ValueUpdated(object sender, float e)
        {
            label1.Text = e.ToString();
        }
    }
}
