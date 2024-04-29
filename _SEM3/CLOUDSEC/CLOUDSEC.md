# CLOUDSEC
## General information

>[!tip] Terminology
>
>| Term | Description |
>|: - | - |
>| VPC | Virtual Private Cloud |
>| RBAC | role-based access control |
>| SAML | Security assertion Markup Language - open standard that attempts to bridge the divide between authentication and authorization |
>| SIEM | Security Information and Event Management |
>| KEK | Key encryption key |
>| DEK | Data encryption key |
>| EKMS | External Key Management System |
>| GC cheatsheet | cheatsheet for all services [GCP cheat sheet](https://googlecloudcheatsheet.withgoogle.com/)|
>| SLO / SLA | Service Level Operation / Agreement |


## Terms explained

- **Middleware:** Software that provides common services and capabilities to applications additional to the OS. Connection tissue between applications, data and users, enabling communication and data management for distributed applications
- **Micro services:** a collection of small independent services, organized around specific capabilities. Runs its own process and communicates with others through APIs. 
	- Examples:
	- User service
	- Product service
	- Order service
	- Payment service
	- Notification service
- **Web application firewall:** Also WAF, is designed to protect web applications, APIs, and serverless workloads. It helps defend against various threats, including DoS and provides web access protection by filtering and monitoring HTTP traffice between a web application and the Internet.
- **Secrets manager:** Storing database credentials in it is best practice. Provides centralized and secure storage solution for sensitive data, with encryption access control and automated credential rotation.
- **X.509 Certificates:** #TODO
- **Cloud DLP unredacted text:** refers to the original, unaltered text that may contain sensitive information. 
- **DLP:** Cloud data loss prevention
- **Container orchestration (Kubernetes):** Automated process of managing, deploying and scaling containerised applications. Kubernetes does:
	- Automated rollouts and rollbacks
	- Automated service up and down scaling 
	- Provides API to control how and where containers will run
- **KMS and EKMS:** KMS is a centralized approach for generating distributing and managing cryptographic keys for devices and applications. EKMS is an approach that allows organizations to use their own external key manager to manage their keys outside of a cloud service provider's environment. EKMS acts as an intermediary between the application and the KMS to ensure that the cryptographic operations are perforemd using the key material managed by the EKMS.

## Remember
#TODO
>[!warning] ToDo
>- SIEM

### Key rotation

**What:** regularly update cryptographic keys used for encryption to maintain data security. Create a new key to replace an old one.

**Why:** 
- The amount of data encrypted with the same key is reduced
- Limit the number of actual messages vulnerable to compromise


### VPC

Virtual private cloud. on-demand, configurable pool of shared resources allocated within a public cloud environment.
A big cloud and you can get a portion of it on-demand. Offers a private cloud environment within a public cloud.

- cost-effective
- flexible and scalable
- only pay for what you use
- adapt to changing business needs.


**VPC Service Control:** in GCP - defines a security perimeter around Google Cloud resources. It helps control the movement of data across a service perimeter through supported services. Extra layer of security independent of Identity and Access Management.


**VPC Flows:** capture information about the IP traffic going to and from network interfaces on Compute Engine. Logs that provide real-time visibility and maintain an ongoing log of access denials to spot potential malicious activity on Google Cloud resources.

## Exam Questions

### Part 1

1. Public cloud computing typically presents which one of the following characteristics:
	1. Pay as you go model
2. Tasks of PaaS users include:
	1. Deployment and management of applications
3. In a cloud "shared responsibility" model:
	1. The cloud service provider manages web application security in an IaaS model
4. If an organisation already has its users' setup in an Active Directory then for Cloud Identity and Access Management, which of the following would be best practice:
	1. Cloud Directory Sync should be used to import and manage users in Cloud Identity
	2. **Careful:** He wasn't so sure about this one
5. Generally long-term stored data benefits from key rotation. If cloud keys are rotated every 180 days and 10 versions are stored, after +/- what maximum period is data re-encryption required?
	1. 5 years
6. With which solution could a company achieve web access protection and defend against denial of service attacks, also working with a global load balancer?
	1. Web application firewall
7. You are designing a large cloud application with microservices. Each microservice needs to connect to a backend database. You want to securely store the database credentials. Where should you store the credentials?
	1. A secrets manager
8. To protect data at rest, a key hierarchy is typically implemented. What is the encryption sequence?
	1. The DEK is encrypted with a KEK and stored as a wrapped entitiy in the cloud key management service
9. Which of the following is usually **not** stored in a Secret Manager?
	1. Cloud DLP unredacted text
10. In setting up a cloud resource hierarchy the scope of different objects differs. In the Google Cloud Platform which of the following is true?
	1. VPC networks are regional resources

### Part 2

1. Kubernetes provides automated container orchestration. Which statement is incorrect about Kubernetes?
	1. Vulnerability scans on containers
2. If an EKMS is used, then when the KMS is called which of the following happens?
	1. Application call is passed to the KMS by the EKMS
3. In a Confidential Compute setup, the memory of the virtual machine is encrypted with a key. Where is this key generated and managed?
	1. In the processor
4. Most cloud providers have set up a global deployment in which of the following ways:
	1. Multiple regions made up of zones
5. What is **NOT** part of the concept of a firewall tag?
	1. Tags can be assigned priority values to determine the precedence of execution
	2. What tags SHOULD be used for:
		1. Firewall rules can match IP addresses, ranges or tags
		2. Tags are user-defined strings that help organize firewall policies for standards-based policy approach
		3. Tags can be associated with functions like a "web-server", with a firewall policy that says any VM with the tag web-server should have ports HTTP, HTTPS, and SSH opened
6. SAML is an open standard that allows identity providers (IdP) to pass authorization credentials to service providers. Which of the following statements about SAML is true
	1. An authorization decision assertion indicates if a user is authorized to use a service
7. Which of the following firewall rules will have priority according to GCP firewall rule precedence:
	1. deny rule with 1000 (lower number is higher priority, deny is stronger than allow)
8. Functions of AWS Cloud Watch do NOT include which one of the following:
	1. SIEM functionality including event collection and analysis.
9. What is the DLP process whereby PII data can be substituted with surrogate values for privacy/CLP in such a way that this substitution can be reversed?
	1. Tokenization (pseudonymization)
10. AWS Security Services are built around which of the following approaches:
	1. Prevent, Detect, Respond, Remediate



## Cloud Computing Models

![[shared_responsibility.png]]


### IaaS

Mostly only hardware provided. You need to maintain and operate the OS and other Software as well as deployment of Software by yourself.

- More cost-efficient
- Good for hosting, data storage, back-ups

Provider:
- Manages Storage, server and networking resources and delivers them
- no OS delivered

User:
- Takes care of software installed and running
- Scale the infrastructure 


### PaaS

Provides hardware and software for application creation, delivered via the web. Users don't need to worry about software updates or hardware failures.
Heroku, Google App Engine. Provides a platform for application development.

- cost effective development
- scalability
- significant reduction in the amount of coding needed.

Provider
- **Operating system management:**
- **Storage capacity planning:**
- **Software patching**
- Handles access and authentication

User:
- **Deployment and management of applications** done by the user


### SaaS

System as a Service
Dropbox.


**Provider:**
- Physical Infrastructure
- provide security controls
- mechanisms for data protection
- restore deleted data


**User:**
- appropriate access to application
- data protection
- data backup




## Digitization Transforming Industries

- Smart logistics
- Predictive maintenance
- Smart Factory / IoT
- Smart Data
- Rail & Public Transport
- Digital Store
- Financial Services


## Cloud itself
General:
- Capable of doing Analytics / AI / ML
- Businesses shifting to not buy own computing power, but to rent from a provider (e.g. dynamic computing power for shops on Black Friday)
- [Google cloud cheat sheet](https://googlecloudcheatsheet.withgoogle.com/)

What is Cloud Computing?
- On demand delivery of compute power
- fast access

capex vs opex:
- operation expenses or
- long time commitment

### Security:
- Business risk
- Reputation
- Availability

### Hybrid vs Multi-Cloud
Hybrid:
- Multiple-cloud is multiple Hyperscalars
- Doing Jobs with multiple Cloud distributors

### Shared responsibility model
I think it's the table with Who's responsibility it is, user or cloud provider.

## Cloud Computing Models
- Infrastructure as a Service
- Platform as a Service
- Software as a Service


## Cloud Infrastructure Layers
See :
![[cloud infrastructure.canvas]]


## Encryption
- Encryption data is stored a long time)
- Encryption in transit 
- Encryption while processing (homomorphic encryption)

## Cloud Identity Access Management (IAM)
- Role based access control
- Property based access control

**Questions:**
- Who
	- Owner
	- Reader
	- Writer
- Can do What
	- Edit
	- Delete
	- Move
	- Read
- On Which Resourcesn at rest (wh
	- Organisation (top level)
	- Folder (mid level)
	- Project (lower level)


## SAML
3 Parties:
- Principal = the user
- Service provider = owner of a web resource
- Identity provider = Performs identity access management services.

SAML is used to exchange authentication information between SP and IP


## OAuth
open standard for authorization that grants secure delegated access to applications, devices, applications...

![[OAuth protocol flow.png]]



## OpenID Connect
Provides a decentralized authentication protocol layer atop OAuth



## Containers
- logical packaging mechanism in which applications can be abstracted from thee environment.
- Allows easy and consistent deployment regardless of the target environment.
- Consistent Environment
	- Helps reducing debug time
	- Predictability
- Run Anywhere
- Isolation (virtualize CPU, memory, storage...)

Example: Docker



## What is Strategy?

- You have long-term goals
- Goals are driven by a vision statements
- How you achieve these goals over a long period of time is a strategy


Vision -> Goal -> Strategy


## Why Cloud

- Scalability
- Availability
- Less responsibility
- Less investment


## What need to be protected 

- Client Data
- research data
- Product data
- Employee information


## Order

Risk register $\rightarrow$ control objective $\rightarrow$ control

- **Example Risk Register:**
	- Data leak or non-compliance due to non-authorized access to sensitive data
- **Example Control Objective:**
	- Only authorized access to sensitive date
- **Control/Safeguard:**
	- IAM system must be in place
	- Authorization process prior to access, e.g. approval
	- Log of any access to sensitive data 




