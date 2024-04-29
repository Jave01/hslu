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

- Binary Ninja for the win



## Anti-Analyse Techniken

- Make analysis more time consuming
- can often be bypassed given enough time and skills


### Packing and Multiple Stages

Hide real malicious code in multi-staged layers
- download payload from the internet
- extract (encrypted / encoded) parts from the same file
- build malicious code at runtime

>[!hint]
>Can often be resolved dynamically.


### Anti-Debug techniques

Detect if debugger is present and execute non-suspicious code if so.

- insertion of garbage bytes to confuse debuggers
- detection via timing
- detection via debugger APIs (`IsDebuggerPresent` or `Ptrace`)



### VM Detection

- searching for VM artifacts (network devices, CPU information, special instructions)
- timing checks (doesn't work well)



>[!note]
>VM Detection and Anti-Debug techniques can be patched easily
>For example in binary ninja always follow the branch as if no debugger is present



### Checksumming

**Techniques to detect code modifications**

- calculation of hashsums over code or 
- #TODO 




### Code obfuscation

- encrypt/encode code
- make code harder to read/understand
- looks different, computes the same
- e.g

```python
def f1(x, y):
	return x + y

def f2(x, y):
	return (x ^ y) + 2 * (x & y)
```

or stuff like this (opaque predicate, control flow gets complicated):

```python
int Foo(x, y):
	if 1 == 2:
		return x - y
	elif 2x % 2 == 1:
		return 2x
	else:
		return x + y

```



- Control flow flattening:
	- That weird stuff in the while loop instead of direct if-statements
- Mixed boolean algebra


### Patching

- overwrite instruction bytes with different assembly instructions



## Android

- Good for reverse engineering because most applications are written in java or Kotlin.
	- Bytecode can be (almost) perfectly be decompiled to source code
- APK is just a zip file


>[!note]
>Analysis is mostly done statically


Disassemble `.class` files:
```bash
javap -c HelloWorld.class
```


Good decompilers:
- CFR
- Procyon
- FernFlower


>[!warning]
>JD(-GUI) apparently gets recommended a lot but seems to be crap


>[!note] Nice to know
>Go to [apkmirror.com](apkmirror.com) for Android APK's


- convert dex files to java files
```bash
dex2jar holy.dex
java -jar holy.jar --outputdir yay
```



>[!tip] Tools
>- `dex2jar`
>- `apktool` - `dex2jar` integrated

```bash
apktool d app.apk
```

