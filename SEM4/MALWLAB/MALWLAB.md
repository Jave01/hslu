# Malware lab

- 5 topics
- assessment: average of the best 4 reports
- one lecture/practical exercise per topic
- **team work** in groups of 1-3 people
	- 2-3 weeks per task
	- handing in analysis report, maybe some code


## Labs

How to:
- Describe the behaviour of ...
- Find ... and analyze how it works.


- Focus on **how** you got there.




## Malware in general

Many different types, in practice often no clear separation. 
Separate on what the malware intends to do.
- **launcher**
- **backdoor**
- **botnet:** 
- **virus/worm:** copies itself and infects other computers
- **rootkit:** conceals the existence of other code
- **ransomware:** encrypts files on the computer


### Can be hard to analyze

- complex binaries, limited time frame
- often only certain code fragments available for analysis
- sample runs in specific environments
- anti-analysis techniques



## Dynamic Analysis

- `file`
- `string`
- VirusTotal (hash or file upload)
- entropy analysis (packed binary e.g. with `utx`)
	- Detect it easy: `diec` / `diel`



### Methods

- run samples in sandboxes
- look for
	- file writes/reads
	- dll injection
	- network scanning
	- DNS resolutions

VMI - virtual machine introspection


Free sandbox analysis:
- hybrid sandbox
- any.run (maybe free)
- joesandbox
- cuckoo sandbox (local running)


## Static analysis

