using RCP;
using RCP.Parameter;
using RCP.Protocol;
using RCP.Transporter;
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

            //update a parameter value
            //param.Value = 0.2f;
            //update label
            //param.Label = "My better Float";
            //update minimum
            //param.TypeDefinition.Minimum = 0f;

            //Rabbit.UpdateParameter(param); // this sends an update to all clients

            //listen for value updates on the parameter
            FRabbit.ParameterValueUpdated = (p) =>
            {
                //Log(p.Value);
            };

            //listen for any changes on the parameter
            //some of the options of the parameter will be updated but since we don't know which ones, we'll have to take over all of them
            FRabbit.ParameterUpdated = (p) =>
            {
                //get application object from a list
                //var appObj = AppObjects[p.Id];

                //update application object
                //appObj.Label = p.Label;
                //...
            };

            //remove parameter
            //Rabbit.RemoveParameter(param.Id);

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
            //var group = FRabbit.CreateGroup();
            //group.Label = "foo";
            var param = (NumberParameter<float>)FRabbit.CreateParameter(RcpTypes.Datatype.Float32);
            //param now is of type NumberParameter<float>
            //holds param.TypeDefinition of type NumberDefinition<float>
            param.Label = "My Flöat: " + param.Id;
            param.Order = param.Id;
            param.Widget = new SliderWidget();
            param.Value = 0.5f;
            param.TypeDefinition.Minimum = -10.0f;
            param.TypeDefinition.Maximum = 10.0f;

            param.ValueUpdated += Param_ValueUpdated;
            
            //group.AddParameter(param);
            FRabbit.Root.AddParameter(param);

            // listen for value updates on the parameter
            //param.ValueUpdated = (p) => Log(p.Value);

            //var paramGroup = ParameterFactory.CreateParameterGroup(1);
            //paramGroup.addChild(param);

            FRabbit.Update();

            param.Label = "asdfasf";
            FRabbit.Update();
        }

        private void Param_ValueUpdated(object sender, float e)
        {
            //throw new System.NotImplementedException();
        }
    }
}
