# pid
### Raspberry Pi Identification Utility 
##### written in C# for Mono

#### What is it?
It's a small command line tool to tell you what model your raspberry Pi is.

It also has the bonus that it can report the information from */proc/cpuinfo* in either XML or JSON formats, which makes it usefull as a tool to be called from web scripts such as PHP, allowing you to format the data for display in a web page.

The primary use (or at least the main use I wrote it for) was in shell scripts.  I have a number of different Pi's and when writing shell scripts for them, it's usually a good idea to be able to customise the scripts for specific devices.

I got fed up of having to look up the revision number values each time, and so I broke out my copy of Visual Studio and came up with this.

#### How does it work?
Quite simply really.

As with all Linux distros, the Raspberry Pi's distro of Debian (aka Raspbian) has a file in the *proc* file system called *cpuinfo*

You can read this file yourself by typing *cat /proc/cpuinfo* at your raspberry Pi command line.  When you do, you should see something similar to the following:

```
pi@raspberrypi:~/$ cat /proc/cpuinfo
processor       : 0
model name      : ARMv6-compatible processor rev 7 (v6l)
BogoMIPS        : 697.95
Features        : half thumb fastmult vfp edsp java tls
CPU implementer : 0x41
CPU architecture: 7
CPU variant     : 0x0
CPU part        : 0xb76
CPU revision    : 7

Hardware        : BCM2708
Revision        : 000f
Serial          : 00000000abe53a55
pi@raspberrypi:~/$
```

The second line from the bottom listed as *Revision* is the magic value, that tells you what type of RPi you have.

There are a number of lists scattered around the internet, the following one is just a starting example:

[Revision info at Raspberry Pi Spy](http://www.raspberrypi-spy.co.uk/2012/09/checking-your-raspberry-pi-board-version/)

Every time I needed to detect my device, I had to go here to find the values, then use those values, eventually *pid* was born.

#### How do I use it?
It's really simple, it takes one option at a time, if you try to give it more, or less, then it'll show you it's help text, which looks like this:

```
PID V1.00 (Raspberry Pi Identification Utility) written by Shawty/DS

Usage is as follows:
pid <option>

Where option is one of the following, NOTE: ONLY one option can be used at a time, if you use multiple only the first one will be acted upon.

        -CJ             Display entire /proc/cpuinfo file as JSON formatted data
        -CX             Display entire /proc/cpuinfo file as XML formatted data
        -PJ             Display platform information as JSON formatted data
        -PX             Display platform information as XML formatted data

The following options are used to get single responses suitable for use in shell scripts and other utilities:
        -H              Display platform hardware
        -S              Display platform serial number
        -V              Display Raspberry Pi Version (1,2 or 3)
        -M              Display Raspberry Pi Model
        -R              Display Raspberry Pi Model Revision
        -C              Display Raspberry Pi Memory Capacity
```

If you use any of the 4 double options, CJ, CX, PJ or PX you'll get the *cpuinfo* direct from proc or the *platform info* decoded from the revision value, in either JSON or XML respectively.

These options are mostly of use by web server scripts, or by other applications who just need bulk output that they will then parse to decide how to display.

The 6 single char options, simply just report the requested variable to stdout followed by a carrige return, then exit.

Beacuse the value is sent to stdout, you can easily consume this value in shell scripts using backticks and many other places where knowing this info could prove usefull.

#### What info does it report?
* **Platform Hardware** - This is the base SOC that the entire system is based on, available in the *Hardware* tag in cpu info
* **Platform Serial Number** - This is the unique CPU based serial number that each ARM based SOC has embedded in it
* **Raspberry Pi Version** - This is the version of the RPi you have, Pi 1, 2 or 3
* **Raspberry Pi Model** - This is the model you have, EG: *Model B*
* **Raspberry Pi Model Revision** - This is the PCB Revision of the board your running, EG: if you have a Model B Rev 2, this will be *2*
* **Raspberry Pi Memory Capacity** - This is the total amount of physical memory your RPi has

If you run this on a board that produces a revision the utility doesnt recognise, then most of the platform values (Version, Model, Revision, Memory) will either show *Unknown* or a *0*

If you come accross a board that this utility doesnt recognise, then please open an issue, and provide me with the details of the board, and a copy and paste output from /proc/cpuinfo

If you have coding skills, then feel free to clone, make the required changes and submit it back as a pull request.

I wont be expending a huge amount of energy maintaining this, as it generally works for what I use it for (and what I reckon most others will use it for).

#### How do I build it?
The entire Visual Studio solution is provided, so if you have a windows machine with a copy of Visual Studio 2013 or higher, you can simply clone the repo, build it, then copy the resulting exe file to your Raspberry Pi.

You **WILL NEED** mono installing on your Raspberry Pi in order to run it.  I was going to write this as a Native C application, but to be honest, it's been so long since I did anything in plain old C, that I really couldn't be bothered. :-) (and I already had a copy of Visual Studio open on my desktop PC anyway)

Running *sudo apt-get install mono-complete* should be all you need to do on the latest versions of Raspbian.  Iv'e not tried to compile or build this using the newer *dot net core* stuff, but to be honest it uses only the very base XML and Linq libraries in .NET, so there's no reason why it shouldn't just compile.  You'll need to convert the project however, so that it's in the correct format for dot net core.

If you want to build the program using mono directly on your Raspberry Pi, then it's simply just a case of typing:

*dmcs -r:System.Xml.Linq.dll pid.cs*

at your command line, then assuming mono is installed correctly, you can run it just by running *./pid.exe*

**Please note this has not been tested with dotnet core, however since it's fairly standard c# code (Dealing with files and general syntax) then it should work ok without too much of a problem**

