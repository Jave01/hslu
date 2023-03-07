# NETW2

## Administratives

### Prüfung

!!! warning "MEP Infos"

    -   Hilfsmittel: 16 A5 Seiten (single-sided, ausgedruckt)
    -   Abschlussprüfung zählt 100%

## Inhalt

### Important Commands

!!! abstract "General commands"

    ```sh title=""
    no shutdown
    exit
    ```

!!! abstract "show commands"

    ```sh title=""
    $ show ip interface brief
    $ show ipv6 interface brief
    $ show startup config
    $ show running config
    $ show version
    $ show ip route
    $ show interfaces port-channel
    $ show etherchannel summary
    $ show etherchannel port-channel

    ```

#### Configuration

$ enable secret <pw>
$ service password-encryption
$ copy running-config startup-config

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
| Input errors    | Total number of errors (runts, giants, no buffer, CRC, frame, overrun     |
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

```

```
