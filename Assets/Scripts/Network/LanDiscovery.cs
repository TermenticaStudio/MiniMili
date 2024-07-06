using System;
using System.Collections.Generic;
using System.Net;
using Mirror;
using Mirror.Discovery;
using UnityEngine;


public class IServerInfo
{
    public IServerInfo(Uri uri, string name)
    {
        this.uri = uri;
        this.serverName = name;
    }
    public Uri uri;
    public string serverName;
}
public struct ClientRequest : NetworkMessage
{
}

public struct ServerResponse : NetworkMessage
{
    public IPEndPoint EndPoint { get; set; }

    public Uri uri;

    public string serverName;

    public long serverId;

}

public class LanDiscovery : NetworkDiscoveryBase<ClientRequest, ServerResponse>
{
    readonly Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>();

    public bool DidFindAny => discoveredServers.Count > 0;
    public List<IServerInfo> GetDiscoveredServers()
    {
        List<IServerInfo> serversInfo = new List<IServerInfo>();
        foreach (var server in discoveredServers)
        {
            IServerInfo serverInfo = new IServerInfo(server.Value.uri, server.Value.serverName);
            serversInfo.Add(serverInfo);
        }
        return serversInfo;
    }
    private string serverName;
    public void SetServerName(string serverName)
    {
        this.serverName = serverName;
    }
    #region Unity Callbacks

#if UNITY_EDITOR
    public override void OnValidate()
    {
        base.OnValidate();
    }
#endif

    public override void Start()
    {
        base.Start();
    }

    #endregion

    #region Server

    /// <summary>
    /// Reply to the client to inform it of this server
    /// </summary>
    /// <remarks>
    /// Override if you wish to ignore server requests based on
    /// custom criteria such as language, full server game mode or difficulty
    /// </remarks>
    /// <param name="request">Request coming from client</param>
    /// <param name="endpoint">Address of the client that sent the request</param>
    protected override void ProcessClientRequest(ClientRequest request, IPEndPoint endpoint)
    {
        base.ProcessClientRequest(request, endpoint);
    }

    /// <summary>
    /// Process the request from a client
    /// </summary>
    /// <remarks>
    /// Override if you wish to provide more information to the clients
    /// such as the name of the host player
    /// </remarks>
    /// <param name="request">Request coming from client</param>
    /// <param name="endpoint">Address of the client that sent the request</param>
    /// <returns>A message containing information about this server</returns>
    protected override ServerResponse ProcessRequest(ClientRequest request, IPEndPoint endpoint) 
    {
        try
        {
            // this is an example reply message,  return your own
            // to include whatever is relevant for your game
            return new ServerResponse
            {
                serverId = ServerId,
                uri = transport.ServerUri(),
                serverName = serverName,
            };
        }
        catch (NotImplementedException)
        {
            Debug.LogError($"Transport {transport} does not support network discovery");
            throw;
        }
     /*   ServerResponse serverRes = new ServerResponse();
        serverRes.serverName = request.serverName;
        return serverRes;*/
    }

    #endregion

    #region Client

    /// <summary>
    /// Create a message that will be broadcasted on the network to discover servers
    /// </summary>
    /// <remarks>
    /// Override if you wish to include additional data in the discovery message
    /// such as desired game mode, language, difficulty, etc... </remarks>
    /// <returns>An instance of ServerRequest with data to be broadcasted</returns>
    protected override ClientRequest GetRequest()
    {
        return new ClientRequest();
    }

    /// <summary>
    /// Process the answer from a server
    /// </summary>
    /// <remarks>
    /// A client receives a reply from a server, this method processes the
    /// reply and raises an event
    /// </remarks>
    /// <param name="response">Response that came from the server</param>
    /// <param name="endpoint">Address of the server that replied</param>
    protected override void ProcessResponse(ServerResponse response, IPEndPoint endpoint) 
    {
        response.EndPoint = endpoint;

        // although we got a supposedly valid url, we may not be able to resolve
        // the provided host
        // However we know the real ip address of the server because we just
        // received a packet from it,  so use that as host.
        UriBuilder realUri = new UriBuilder(response.uri)
        {
            Host = response.EndPoint.Address.ToString()
        };
        response.uri = realUri.Uri;
        discoveredServers[response.serverId] = response;
        OnServerFound.Invoke(response);
    }

    #endregion
    public override void StartDiscovery()
    {
        base.StartDiscovery();
        discoveredServers.Clear();
    }

    public override void AdvertiseServer()
    {
        base.AdvertiseServer();
        discoveredServers.Clear();
    }
    public void AdvertiseServer(string serverName)
    {
        SetServerName(serverName);
        AdvertiseServer();
    }
}
