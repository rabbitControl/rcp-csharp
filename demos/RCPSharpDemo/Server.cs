using RCP;
using RCP.Parameter;
using RCP.Protocol;
using RCP.Transporter;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace RCPSharpDemo
{
    public partial class Server : Form
    {
        RCPServer Rabbit;
        Dictionary<string, IParameter> FParams = new Dictionary<string, IParameter>();

        Client FClient;

        public Server()
        {
            InitializeComponent();

            Rabbit = new RCPServer();

            //Rabbit.AddTransporter(new UDPServerTransporter("127.0.0.1", 4568, 4567));
            Rabbit.AddTransporter(new WebsocketServerTransporter("127.0.0.1", 10000));

            

            //update a parameter value
            //param.Value = 0.2f;
            //update label
            //param.Label = "My better Float";
            //update minimum
            //param.TypeDefinition.Minimum = 0f;

            //Rabbit.UpdateParameter(param); // this sends an update to all clients

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

            FClient = new Client();
            FClient.Show();
        }

        private void Server_FormClosing(object sender, FormClosingEventArgs e)
        {
            FClient.Dispose();
            Rabbit.Dispose();
        }
        
        private void button1_Click(object sender, System.EventArgs e)
        {
            string id = FParams.Count.ToString();
            var param = ParameterFactory.CreateParameter(id.ToRCPId(), RcpTypes.Datatype.Uri);
            //param now is of type NumberParameter<float>
            //holds param.TypeDefinition of type NumberDefinition<float>
            param.Label = "My Flöat: " + id;
            param.Description = "@€träöü";
            param.Order = 1;
            param.Userdata = Encoding.UTF8.GetBytes("öäüad");
            (param.TypeDefinition as IUriDefinition).Schema = "file";
            param.Widget = new SliderWidget();
            //param.Value = 0.5f;
            //param.TypeDefinition.Minimum = -1.0f;
            //param.TypeDefinition.Maximum = 1.0f;

            //var paramGroup = ParameterFactory.CreateParameterGroup(1);
            //paramGroup.addChild(param);

            FParams.Add(id, param);

            ////expose parameter
            Rabbit.AddParameter(param); //sends add to all clients
        }

        
    }
}
