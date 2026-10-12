Startup instructions for RavenDB on Linux
=========================================

* RavenDB as a Console Application
Open bash terminal
Type:
    chmod +x run.sh
    ./run.sh

* RavenDB as Daemon (systemd - applies to Ubuntu 16.04)
Open bash terminal, and create file /etc/systemd/system/ravendb.service, using super user permissions, containing:
    [Unit]
    Description=RavenDB v7.2
    After=network.target

    [Service]
    LimitCORE=infinity
    LimitNOFILE=65535
    LimitRSS=infinity
    LimitAS=infinity
    LimitMEMLOCK=infinity
    TasksMax=infinity
    StartLimitBurst=0
    AmbientCapabilities=CAP_NET_BIND_SERVICE  
    Restart=on-failure
    Type=exec
    TimeoutStopSec=300
    User=<desired user>
    ExecStart=/path/to/RavenDB/run.sh

    [Install]
    WantedBy=multi-user.target

Note: Replace in the above text the username "User=<desired user>" and set path in "ExecStart"

Then register the service and enable it on startup by typing:
    systemctl daemon-reload
    systemctl enable ravendb.service

Start the service:
    systemctl start ravendb.service

View its status using:
    systemctl status ravendb.service
    or
    journalctl -f -u ravendb.service

* Setup
Open browser, if not opened automatically, at url printed in "Server available on: <url>"
Follow the web setup instructions at: https://docs.ravendb.net/7.2/start/installation/setup-wizard/overview

* Separate Cluster and Client Addresses
ServerUrl controls the HTTP listening addresses. PublicServerUrl remains the client-facing
HTTP address and the address used by the local startup self-check.
Set ServerUrl.Cluster to the HTTP URL reachable by the other cluster nodes when those
nodes use a private network. Cluster bootstrap, reverse connection checks and internal
replication advertise this URL without requiring it to be reachable during startup.
The default cluster TCP address uses its host and the bound ServerUrl.Tcp port.
Set PublicServerUrl.Tcp.Cluster explicitly when the reachable TCP port differs from the
bound port. PublicServerUrl.Tcp and PublicServerUrl.Tcp.External retain their client and
external replication settings. Without ServerUrl.Cluster, existing defaults apply.
Use the cluster HTTP URL when adding a node to the cluster. Existing multi-node cluster
topology URLs must already use reachable cluster addresses; startup only updates a
single-node topology automatically.

* Fork Cluster Capacity
This fork does not enforce license limits on distributed cluster membership, cluster
size, CPU count or process working-set memory. Explicit node resource settings and
physical capacity still apply. Other licensed feature checks are unchanged.


* Upgrading to a New Version
Follow the upgrade instructions available at: https://docs.ravendb.net/7.2/start/installation/upgrading-to-new-version



