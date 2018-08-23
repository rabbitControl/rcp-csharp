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
            var group = FRabbit.CreateGroup("foo");

            var param = FRabbit.CreateNumberParameter<float>("my float", group);
            param.Order = param.Id;
            param.Widget = new SliderWidget();
            param.Default = 7.0f;
            param.Value = 2.0f;
            param.Minimum = -10.0f;
            param.Maximum = 10.0f;
            param.ValueUpdated += Param_ValueUpdated;

            var nt = FRabbit.CreateNumberParameter<int>("my int", group);
            nt.Value = 3;
            nt.Minimum = -5;
            nt.Maximum = 5;

            var enm = FRabbit.CreateEnumParameter("my enum", group);
            enm.Entries = new string[3] { "aber", "biber", "zebra" };
            enm.Default = "biber";
            enm.Value = "zebra";
            //enm.ValueUpdated += Enm_ValueUpdated;

            var str = FRabbit.CreateStringParameter("my string", group);
            str.Value = "foobar";

            var clr = FRabbit.CreateRGBAParameter("ma color", group);
            clr.Value = Color.Red;

            var strarr = FRabbit.CreateStringArrayParameter("my string array", 3);
            strarr.Default = new string[3] { "a", "b", "c" };
            strarr.Value = new string[3] { "aa", "bv", "cc" };

            var intarr = FRabbit.CreateNumberArrayParameter<int[], int>("my int array", 3);
            intarr.Default = new int[3] { 1, 2, 4 };
            intarr.Value = new int[3] { 4, 5, 6 };

            FRabbit.Update();

            //enm.Value = "biber";
            //FRabbit.Update();

            //enm.Value = "aber";
            //FRabbit.Update();
        }

        private void Enm_ValueUpdated(object sender, string e)
        {
            label1.Text = e.ToString();
        }

        private void Param_ValueUpdated(object sender, float e)
        {
            label1.Text = e.ToString();
        }
    }
}
