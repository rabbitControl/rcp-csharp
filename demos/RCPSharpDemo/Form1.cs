using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using RCP;
using RCP.Model;
using System.IO;

namespace RCPSharpDemo
{
    public partial class Form1 : Form
    {
        RabbitServer Rabbit;
        RabbitClient Carrot;

        public Form1()
        {
            InitializeComponent();

            Carrot = new RabbitClient();
            Carrot.Transporter = new UDPClientTransporter("127.0.0.1", 9001, 9000);

            Carrot.ParameterAdded = (p) =>
            {
                //create UI matching p
                MessageBox.Show((Carrot.Params[p] as NumberParameter<float>).Value.ToString());
            };

            Carrot.ParameterRemoved = (p) =>
            {
                //remove UI matching p
            };

            //listen for any parameter changes
            Carrot.ParameterUpdated = (p) =>
            {
                MessageBox.Show((Carrot.Params[p] as NumberParameter<float>).Value.ToString());
                //var uip = UIParams[p.Id];
                //uip.Label = p.Label;
                //...
            };

            //listen for any value changes
            //Carrot.ParameterValueUpdated = (p, v) =>
            //{
            //    var uip = UIParams[p.Id];
            //    switch (p.TypeDefinition.Datatype)
            //    {
            //        case Datatype.Float32: uip.Value = (float)v; break;
            //            //...
            //    }
            //};

            ////listen for any typedefinition changes
            //Carrot.TypeDefinitionUpdated = (p) =>
            //{
            //    var uip = UIParams[p.Id];
            //    switch (p.TypeDefinition.Datatype)
            //    {
            //        case Datatype.Float32: uip.Minimum = ((NumberDefinition<float>)p.TypeDefinition).Minimum; break;
            //            //...
            //    }
            //};

            //ask for initial parameters from server
            //Carrot.Connected = () =>
            //{
            //    //clear UI
            //    Carrot.Initialize();
            //};

            //Carrot.Disconnected = () => { //};
            //}






            Rabbit = new RabbitServer();
            Rabbit.AddTransporter(new UDPServerTransporter("127.0.0.1", 9000, 9001));
            Rabbit.AddTransporter(new WebsocketServerTransporter("127.0.0.1", 10000));

            uint id = 123;
            var param = (NumberParameter<float>)ParameterFactory.CreateParameter(id, RcpTypes.Datatype.Float32);
            //param now is of type NumberParameter<float>
            //holds param.TypeDefinition of type NumberDefinition<float>
            param.Label = "My Float";
            param.Value = 0.5f;
            param.TypeDefinition.Minimum = -1.0f;
            param.TypeDefinition.Maximum = 1.0f;
            param.TypeDefinition.MultipleOf = 0.01f;

            //uint id = 1234;
            //var param = (BooleanParameter)ParameterFactory.CreateParameter(id, RcpTypes.Datatype.Boolean);
            ////param now is of type NumberParameter<float>
            ////holds param.TypeDefinition of type NumberDefinition<float>
            //param.Label = "My Bool";
            //param.Value = false;

            //expose parameter
            Rabbit.AddParameter(param); //sends add to all clients

            //var arrayParam = (ArrayParameter<float>)ParameterFactory.CreateArrayParameter(234, RcpTypes.Datatype.Float32, 3);
            //arrayParam.Label = "my array";

            //var floats = new float[3] { 3.2f, 4.5f, 6.8f };
            //arrayParam.Value = floats.ToList();

            ////using (var stream = new MemoryStream())
            ////using (var writer = new BinaryWriter(stream))
            ////{
            ////    arrayParam.Write(writer);
            ////    var bytes = stream.ToArray();
            ////}

            //Rabbit.AddParameter(arrayParam);

            ////update value
            //param.Value = 0.2f; //sends an update to all clients

            ////update label
            //param.Label = "My better Float"; //sends an update to all clients

            ////update typedefinition
            //param.TypeDefinition.Minimum = 0f; //sends an update to all clients

            ////listen for value changes on the parameter
            //param.ValueUpdated = (p) => Log(p.Value);

            ////listen for any changes on the parameter
            ////some of the options of the parameter will be updated but since we don't know which ones, we'll have to take over all of them
            //param.Updated = (p) =>
            //{
            //    //get application object from a list
            //    var appObj = AppObjects[p.Id];

            //    //update application object
            //    appObj.Label = p.Label;
            //    //...
            //};

            ////listen for changes on the typedefinition
            //param.TypeDefinition.Updated = (p) => param.TypeDefinition.Minimum = p.TypeDefinition.Minimum;

            ////remove parameter
            //Rabbit.RemoveParameter(param);


            Carrot.Initialize();
        }
    }
}
