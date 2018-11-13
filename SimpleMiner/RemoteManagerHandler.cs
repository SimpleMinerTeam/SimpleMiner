using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using SimpleCPUMiner.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Windows.System.RemoteSystems;

namespace SimpleCPUMiner
{
    public static class RemoteManagerHandler
    {

        public static async void SendMessageAsync(SimpleMinerManager.Model.MessageBase msg, string url)
        {
            try
            {
                string msgType = string.Empty;
                string data = string.Empty;

                if (msg is SimpleMinerManager.Model.RegisterMessage)
                {
                    var origMsg = msg as SimpleMinerManager.Model.RegisterMessage;
                    msgType = "Register";
                    data = JsonConvert.SerializeObject(origMsg);
                }
                else if (msg is SimpleMinerManager.Model.StatusMessage)
                {
                    var origMsg = msg as SimpleMinerManager.Model.StatusMessage;
                    msgType = "Status";
                    data = JsonConvert.SerializeObject(origMsg);
                }

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Type", msgType);
                    var httpContent = new StringContent(data, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync($"http://{url}/web/", httpContent);
                    IEnumerable<string> respType;
                    string type = string.Empty;
                    if (response.Headers.TryGetValues("Type", out respType))
                    {
                        
                        switch (type)
                        {
                            case "RegResp":
                                SimpleMinerManager.Model.Responses.RegisterMessageResponse respMsg;
                                respMsg = JsonConvert.DeserializeObject<SimpleMinerManager.Model.Responses.RegisterMessageResponse>(response.Content.ToString());

                                break;
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
                //Messenger.Default.Send<MinerOutputMessage>(new MinerOutputMessage() { OutputText = $"Failed to send message to remote host {ex.Message} {ex.InnerException}" });
            }
        }

    }
}
