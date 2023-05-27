# NETW2

## Administratives

### Prüfung

Testat:

-   [x] Packet Tracer Aufgabe Inter VLAN
-   [ ] Packet Tracer Aufgabe 2
-   [ ] 25\.05\. lab on site

!!! warning "MEP Infos"

    -   Hilfsmittel: 16 A5 Seiten (single-sided, ausgedruckt)
    -   Abschlussprüfung zählt 100%

## Inhalt

### Important Commands

#### General Commands

```sh title="General Commands" linenums="1"
no shutdown
exit
```

#### Show commands

```sh title="show commands"  linenums="1"
show ip interface brief
show ipv6 interface brief
show interface trunk
show startup config
show running config
show version
show ip route
show interfaces port-channel
show etherchannel summary
show etherchannel port-channel
show vlan brief
show spanning-tree
show standby # show HSRP
show port-security interface # show port security
```

#### Configuration

```sh title="Common configuration" linenums="1"
enable secret <pw>
service password-encryption
copy running-config startup-config
ip default-gateway <ip-address>
```

### Switches

#### Layer 3 Switches

These are just switches (Layer 2) with an integrated router.

#### Micro Segmentation

-   bi-directional communication
-   automatic speed adjustment

#### Management

To prepare a switch for remote management access, the switch must be configured with an IP address.

#### Switch Virtual Interface (SVI)

All switches are per default in VLAN-1

#### Network Access Layer Issues

| Error Type      | Description                                                               |
| --------------- | ------------------------------------------------------------------------- |
| Input errors    | Total number of errors (runts, giants, no buffer, CRC, frame, overrun)    |
| Runts           | Packets that are discarded, because too small                             |
| Giants          | Packets that are discarded, because too big                               |
| CRC             | CRC Errors were generated                                                 |
| Output Errors   | Sum of all errors that prevented the final transmission                   |
| Collisions      | Number of messages retransmitted                                          |
| Late Collisions | A collision that occurs after 512 bits of the frame have been transmitted |

### Switching Concepts

#### Switching in Networking

Ingress is the term for when a frame is entering an interface and Egress the term for a frame leaving the interface

A switch uses its MAC address table to make forwarding decisions.

!!! note "Note"

    A switch will never allow traffic to be forwarded out the interface it received the traffic

**Switch Forwarding Methods:**

-   **Store-and-forward switching:** Receives the entire frame and ensures the frame is valid.
-   **Cut-Through switching:** Forwards the frame immediately after determining the destination MAC address of an incoming frame and the egress port.

!!! note "Note"

    The preferred Cisco method is store-and-forward switching

#### Collision Domains

Every port is a collision domain.
Switches elliminates collision domains and reduce congestion

-   When there is full duplex on the link the collision domains are eliminated.
-   When there is one or more devices in half-duplex there will now be a collision domain.
-   There will now be contention for the bandwidth.
-   Collisions are now possible
-   Most devices, including Cisco and Microsoft use auto-negotiation as the default setting for duplex and speed.

#### Link Aggregation

Sometimes it's useful to use multiple links for communicating between two end-devices. Per default STP prevents such communication.
EtherChannel is a technology which allows link aggregation which will not be blocked by STP

-   Fast Ethernat and Gbit Ethernet cannot be mixed
-   Can consist of up to eight configured Ethernet ports
-   Both end devices must be configured equally
-   EtherChannels can be formed through negotiation using one of two protocols, Port Aggregation Protocol (PAgP) or Link Aggregation Control Protocl (LACP). PAgP is only availabe on Cisco devices.

### VLAN

Logische Verbindung (Kreation von Netzen) zwischen unterschiedlichen physischen Verbindungen

VLAN configs sind im flash gespeichert (`show flash`)

#### create VLANs

```sh title="Create and use a VLAN on switch" linenums="1"
# create vlan
configure terminal
vlan <vlan_id>
name <id_name>
exit

# assign vlan to interface
configure terminal
interface fastEthernet 0/1
switchport mode access
switchport access vlan <vlan_id>
exit

# --- extra information ---
# If you want to assign multiple ports to one VLAN: (e.g. 11-17)
conf t
vlan 10
int range fastEthernet 0/11-17 // alle zusammenfassen
switchport mode access vlan 10
```

VLAN interface: virtuelles Interface -> bezieht sich auf alle Interfaces welche in diesem VLAN sind.  
Trunk: Verbindung von VLANs für nächste Switch

VLAN Tag: VLAN ID für Trunck

#### Inter-VLAN Routing

VLANs verbinden (Trunks?)

Neue Methode für viele VLANs verbinden: Router on a Stick oder Layer 3 Switch

switchport ist layer 2. Wenn man `no switchport` eingibt, wird der aktive Port auf 3 initialisiert (Port ist routed).

Schritte:

1. VLANs auf Switch kreieren
2. Sub-interfaces auf router kreieren und die Netz-IP's zuweisen.
3. Trunk von Switch auf Router machen

```sh title="Sub-Interface einrichten Beispiel" linenums="1"
# enable interface
enable
conf t
int gigabitEthernet 0/1
no shut
exit

# create sub-interface
int g0/0.10 # <int>.<sub-interface-number> whereas the sub-interface number is usually the vlan number
encapsulation dot1Q <vlan_id_number>
ip address 172.17.10.1 255.255.255.0

show interfaces | include Gig|802.1q # display

# enable trunking on switch
interface <int-id> # int-id is the interface connected to the router or to the next switch
switchport mode trunk # enable trunking
no shut
exit
```

!!! note "Note"

    The default gateway on the PC's has to be the IP address of the vlan on the router

### STP

Ethernet needs to be loop free. But for redundancy purpose, multiple paths must be present. Spanning Tree Protocol helps preventing circular communication.

1. Elect the root bridge
2. Elect the root ports
3. Elect designated ports
4. Elect alteernate (blocked) ports

Standard Cisco priority is 32769. Lower priority is preferable.

#### Root Bridge

Is the switch with the lowest priority. Path is determined by cost. Low bandwidth increases the 'cost' of the channel.

#### Root Port

Based on the cost, the algorithm chooses the port with the lowest cost (usually shortest path to root bridge). The port going to the bridge is the root port.

#### Designated port

The port where data income is preferred. All ports on the root bridge are designated ports. If one port isn't a root port it is a designated port.

#### Alternate ports

If a connection is between two designated ports (neither side is a root port) they become alternate (blocked) ports. Ports on the root bridge never get blocked. If all factors on the switches and ports are the same, decision making about blocked ports depends on the port ID **of the sender** (F0/1, F0/2 ...). If there are only two switches, the root bridge is the sender.

### DHCPv4

Cisco server can be configured to provide DHCPv4 services.

1. Client sends a DHCPDISCOVER message to the server
2. Server sends a DHCPOFFER message to the client
3. Client sends a DHCPREQUEST message to the server
4. Server sends a DHCPACK message to the client

If some clients have a static ip address they can be excluded from the DHCP pool with the `ip dhcp excluded-address` command.
If you want to exclude a range of ip addresses you can use # excluding range of addresses is `ip dhcp excluded-address <first_ip> <last_ip>`.

#### DHCPv4 Configuration

If no DHCP server is reachable and the client has the configuration "Obtain an IP address automatically" the client will use the APIPA address (169.254.xxx.xxx).

???+ example "DHCPv4 Configuration Example"

    ```sh title="DHCPv4 Configuration" linenums="1"
    enable
    conf t
    # create new pool
    ip dhcp pool <pool_name>
    # network address and subnet mask
    network <network_address> <subnet_mask>
    # default gateway of the LAN (ip address of router connected to the clients)
    default-router <ip_address>
    # dns server
    dns-server <ip_address>
    # domain name
    domain-name <domain_name>
    # exclude static addresses
    ip dhcp excluded-address <ip_address>
    exit
    # verify the configuration
    show running-config | include dhcp
    # verify that the DHCP server is running
    show ip dhcp server statistics
    ```

    !!! danger "Reminder"

        Don't forget to enable auto-configuration on the client

If hosts are supposed to connect to a DHCP server on a different network, the router in between must be configured as a DHCP relay agent.

This can be done with the `ip helper-address <ip_address>` command.

```sh title="DHCP Relay Agent Configuration" linenums="1"
int <interface> # interface to LAN with DHCP clients
ip helper-address <ip_address> # ip address of the DHCP server
```

For configuring a router as a DHCP client on a specified interface use the `ip address dhcp` command.

```sh title="DHCP Client Configuration" linenums="1"
int <interface>
ip address dhcp
no shutdown
show ip interface brief # verify the configuration
```

### IPv6

Enable IPv6 on Cisco devices with `ipv6 unicast-routing`.

Global unicast is manually configured using the `ipv6 address` command.

Dynamic IP Assignment

-   Stateless: No one is tracking which IP address is assigned to which device. The device itself generates the IP address.
    -   SLAAC only
    -   SLAAC with DHCPv6
-   Stateful: The DHCPv6 server tracks which IP address is assigned to which device.

#### Flags

A flag - if set, SLAAC is used (`ipv6 nd autonomous-flag`)
O flag - if set, additional information is provided by DHCPv6 (`ipv6 nd other-config-flag`)
M flag - if set, stateful DHCPv6 is used (`ipv6 nd managed-config-flag`)

if m is set, o is set automatically

#### DHCPv6

Enable DHCPv6 on Cisco Router with `ipv6 address dhcp`.

If the DHCP Server is located on a different network than the client, then the IPv6 router can be configured as a DHCPv6 relay agent.

```sh title="DHCPv6 Relay Agent" linenums="1"
interface <int-id>
ipv6 dhcp relay destination <ip_address>
```

### Router redundancy

One way to prevent a single point of failure at the default gateway is to implement a virtual router. By sharing one IP and MAC address between two routers, the routers can be configured to share the load.

FHRP (First Hop Redundancy Protocols):

-   Cisco Protocol: HSRP (Hot Standby Router Protocol)
-   General Protocol: VRRP (Virtual Router Redundancy Protocol)

#### Hello messages

To check if router is alive every x-seconds a hello message is sent. If the router doesn't receive a hello message within a certain time, it will take over the virtual IP address.

#### Configure HSRP

```sh title="HSRP Configuration" linenums="1"
# enable HSRP on interface
interface <int-id>
standby version <version> # version is 1 or 2
standby <group-id> ip <ip_address> # virtual ip address, group represents the group of routers
```

### Switch Attack Categories

-   MAC Table Attacks (MAC Flooding)
-   VLAN Attacks - Includes VLAN hopping and VLAN spoofing.
-   DHCP Attacks - Includes DHCP spoofing and DHCP starvation.
-   ARP Attacks - Includes ARP spoofing and ARP poisoning.
-   STP Attacks - Includes STP spoofing and STP poisoning.
-   Address Spoofing - Includes MAC spoofing and IP spoofing.

1. MAC Table Attacks (MAC Flooding)

Limit the amount of MAC address registering per port.

```sh title="MAC Table Attacks" linenums="1"
# limit the amount of MAC addresses per port
interface <int-id>
switchport port-security maximum <number>
```

### Switch Security Configuration

#### Port Security

Limit the amount of MAC addresses per port

```sh title="Port Security" linenums="1"
# limit the amount of MAC addresses per port
interface <int-id>
switchport port-security maximum <number>
```

#### Port Security Aging

-   Absolute - The secure addresses on the port are deleted after the specified aging time.
-   Inactivity - The secure addresses on the port are deleted if they are incomplete for the specified time.

```sh title="Port Security Aging" linenums="1"
# set the aging time
interface <int-id>
switchport port-security aging time <time>
switchport port-security aging type <absolute|inactivity>
```

#### Port Security Violation

-   Shutdown - The port shuts down and needs to be reenabled by an administrator. **A syslog message is sent**.
-   Restrict - The port drops packets with unknown source addresses until you remove a sufficient number of secure MAC addresses. **A syslog message is sent**.
-   Protect - Least secure, the port drops packets with unknown source addresses until you remove a sufficient number of secure MAC addresses. **No syslog message is sent**.

```sh title="Port Security Violation" linenums="1"
# set the violation action
interface <int-id>
switchport port-security violation <shutdown|restrict|protect>
```

### WLAN Concepts

#### Types of Wireless Networks

-   Wireless Personal-Area Network - Bluetooth
-   Wireless LAN - Medium sized networks up to about 300 feet.
-   Wireless MAN - Large geographical area such as city or district
-   Wireless WAN - Extensive geographical area for national or global communication
-   Cellular Broadband - Mobile communication
-   Satellite Broadband - Typically used in rural locations where cable and DSL are unavailable.

#### Wireless Topology Modes

-   **Ad hoc mode** - Peer-to-peer network without an access point.
-   **Infrastructure mode** - Wireless clients connect to an access point.
-   **Tethering** - A mobile device shares its internet connection with other devices.

#### Wireless Clieent and AP Association

To achieve successful association, the following parameters must match:

-   **SSID** - Service Set Identifier, name of the wireless network
-   **Password** - Password to connect to the wireless network
-   **Network mode** - The 802.11 standard in use
-   **Security mode** - The security protocol in use e.g. WPA2
-   **Channel** - The channel the AP is using / Frequenzbereich

### Access Points

#### Basic Network configuration

1. Login to the AP
2. Change the default administrator password
3. Login to the AP again
4. Change the default DHCP IPv4 address.
5. Renew the IP address
6. Log in to the router with the new IP address
