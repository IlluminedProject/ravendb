using Microsoft.AspNetCore.Http;
using Raven.Client.Http;
using Raven.Server.ServerWide;

namespace Raven.Server.Extensions
{
    public static class ClusterTopologyExtensions
    {
        public static void ReplaceCurrentNodeUrlWithClientRequestedNodeUrlIfNecessary(this ClusterTopology topology, ServerStore serverStore, HttpContext httpContext)
        {
            if (serverStore.PublishedServerUrls != null)
            {
                foreach (var (tag, urls) in serverStore.PublishedServerUrls.Urls)
                {
                    if (urls.PublicUrl != null)
                        topology.ReplaceCurrentNodeUrlWithClientRequestedUrl(tag, urls.PublicUrl);
                }
            }

            var currentNodeUrlAsSeenByTheClient = serverStore.GetNodeHttpServerUrl(httpContext.Request.GetClientRequestedNodeUrl());
            topology.ReplaceCurrentNodeUrlWithClientRequestedUrl(serverStore.NodeTag, currentNodeUrlAsSeenByTheClient);
        }
    }
}
