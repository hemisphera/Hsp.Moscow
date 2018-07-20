using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Hsp.Moscow.Plugins
{

  class VlcClient
  {

    public string VlcHost { get; private set; }

    public string Password { get; set; }


    public VlcClient()
      : this("localhost:8080")
    {
    }

    public VlcClient(string host)
    {
      VlcHost = host;
    }


    public async Task InitPlaylist()
    {
      await ClearPlaylist();
      await EnqueueItem(@"file:///C:/Users/thomas/Desktop/Projektionen.xspf");
    }

    public async Task ClearPlaylist()
    {
      await SendRequest("pl_empty");
    }

    public async Task EnqueueItem(string url)
    {
      var parameters = new Dictionary<string, string>
      {
        { "input", url }
      };
      await SendRequest("in_enqueue", parameters);
    }

    private async Task SendRequest(string command, IDictionary<string, string> parameters = null)
    {
      await Task.Run(() =>
      {
        if (parameters == null)
          parameters = new Dictionary<string, string>();
        parameters.Add("command", command);

        var paramStr = String.Join("&", parameters.Select(e => $"{e.Key}={HttpUtility.UrlEncode(e.Value)}"));
        var requestUrl = $"http://{VlcHost}/requests/status.xml?{paramStr}";

        var req = WebRequest.Create(requestUrl);

        if (!String.IsNullOrEmpty(Password))
        {
          var credential = Convert.ToBase64String(Encoding.Default.GetBytes(":" + Password));
          req.Headers[HttpRequestHeader.Authorization] = "Basic " + credential;
        }
        
        req.UseDefaultCredentials = false;
        var resp = req.GetResponse();
      });
    }

  }

}
