using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace pid
{
  class Program
  {
    private static readonly Dictionary<string, string> _cpuInfo = new Dictionary<string, string>();
    private static readonly Dictionary<string, string> _platformInfo = new Dictionary<string, string>();

    private static void LoadCpuInfo()
    {
      //List<string> fileLines = File.ReadAllLines("/proc/cpuinfo").ToList();
      List<string> fileLines = File.ReadAllLines("cpuinfo").ToList();
      foreach (string line in fileLines)
      {
        if (String.IsNullOrEmpty(line)) continue;

        string[] parts = line.Split(':');
        if (parts.Length != 2) continue;

        string keyName = parts[0].Replace(" ", String.Empty).Trim();
        string value = parts[1].Trim();

        _cpuInfo.Add(keyName, value);
      }
    }

    private static void DisplayCpuInfoAsJson()
    {
      string concatenatedItems = _cpuInfo.Aggregate(String.Empty, (current, item) => current + String.Format("\t\"{0}\": \"{1}\",", item.Key, item.Value));
      concatenatedItems = concatenatedItems.Trim(',');
      concatenatedItems = concatenatedItems.Replace(",", ",\n");
      Console.WriteLine("{");
      Console.WriteLine(concatenatedItems);
      Console.WriteLine("}");
    }

    private static void DisplayCpuInfoAsXml()
    {
      XElement doc = new XElement("picpuinfo");
      foreach (KeyValuePair<string, string> item in _cpuInfo)
      {
        doc.Add(new XElement(item.Key, item.Value));
      }
      Console.WriteLine(doc.ToString());
    }

    private static void DecodeRevision(string revisionInput)
    {
      string piVersion = "0"; // defaults
      string modelName = "Unknown";
      string modelRevision = "Unknown";
      string modelMemory = "Unknown";
      string notes = String.Empty;

      switch (revisionInput)
      {
        case "0002":
          piVersion = "1";
          modelName = "Model B";
          modelRevision = "1";
          modelMemory = "256MB";
          break;

        case "0003":
          piVersion = "1";
          modelName = "Model B ECN0001";
          modelRevision = "1";
          modelMemory = "256MB";
          notes = "No Fuses, D14 Removed";
          break;

        case "0004":
        case "0005":
        case "0006":
          piVersion = "1";
          modelName = "Model B";
          modelRevision = "2";
          modelMemory = "256MB";
          break;

        case "0007":
        case "0008":
        case "0009":
          piVersion = "1";
          modelName = "Model A";
          modelRevision = "1";
          modelMemory = "256MB";
          break;

        case "000d":
        case "000e":
        case "000f":
          piVersion = "1";
          modelName = "Model B";
          modelRevision = "2";
          modelMemory = "512MB";
          break;

        case "0010":
        case "0013":
        case "900032":
          piVersion = "1";
          modelName = "Model B+";
          modelRevision = "1";
          modelMemory = "512MB";
          break;

        case "0011":
          piVersion = "1";
          modelName = "Compute Module";
          modelRevision = "1";
          modelMemory = "512MB";
          break;

        case "0014":
          piVersion = "1";
          modelName = "Compute Module";
          modelRevision = "1";
          modelMemory = "512MB";
          notes = "Made in China by Embest";
          break;

        case "0012":
          piVersion = "1";
          modelName = "Model A+";
          modelRevision = "1";
          modelMemory = "256MB";
          break;

        case "0015":
          piVersion = "1";
          modelName = "Model A+";
          modelRevision = "1";
          modelMemory = "256MB or 512MB";
          notes = "Made in China by Embest";
          break;

        case "a01041":
          piVersion = "2";
          modelName = "Model B v1.1";
          modelRevision = "1";
          modelMemory = "1GB";
          notes = "Made in the UK by Sony";
          break;

        case "a21041":
          piVersion = "2";
          modelName = "Model B v1.1";
          modelRevision = "1";
          modelMemory = "1GB";
          notes = "Made in China by Embest";
          break;

        case "a22042":
          piVersion = "2";
          modelName = "Model B v1.2";
          modelRevision = "1";
          modelMemory = "1GB";
          break;

        case "900092":
          piVersion = "2";
          modelName = "Zero v1.2";
          modelRevision = "1";
          modelMemory = "512MB";
          break;

        case "900093":
          piVersion = "2";
          modelName = "Zero v1.3";
          modelRevision = "1";
          modelMemory = "512MB";
          break;

        case "0x9000C1":
          piVersion = "2";
          modelName = "Zero W";
          modelRevision = "1";
          modelMemory = "512MB";
          break;

        case "a02082":
          piVersion = "3";
          modelName = "Model B";
          modelRevision = "1";
          modelMemory = "1GB";
          notes = "Made in the UK by Sony";
          break;

        case "a22082":
          piVersion = "3";
          modelName = "Model B";
          modelRevision = "1";
          modelMemory = "1GB";
          notes = "Made in China by Embest";
          break;

      }

      _platformInfo.Add("piVersion", piVersion);
      _platformInfo.Add("modelName", modelName);
      _platformInfo.Add("modelRevision", modelRevision);
      _platformInfo.Add("modelMemory", modelMemory);
      _platformInfo.Add("notes", notes);
    }

    private static void DisplayPlatformAsJson()
    {
      if (!_cpuInfo.ContainsKey("Revision"))
      {
        Console.WriteLine("{");
        Console.WriteLine("\t\"error\":\"No revision present in cpu info, cannot determine pi model\"");
        Console.WriteLine("}");
        return;
      }

      Console.WriteLine("{");
      Console.WriteLine("\t\"piVersion\": {0}", _platformInfo["piVersion"]);
      Console.WriteLine("\t\"piModel\": \"{0}\"", _platformInfo["modelName"]);
      Console.WriteLine("\t\"piModelRevision\": {0}", _platformInfo["modelRevision"]);
      Console.WriteLine("\t\"piMemory\": \"{0}\"", _platformInfo["modelMemory"]);
      Console.WriteLine("\t\"notes\": \"{0}\"", _platformInfo["notes"]);
      Console.WriteLine("}");
    }

    private static void DisplayPlatformAsXml()
    {
      XElement doc = new XElement("piplatform");

      if (!_cpuInfo.ContainsKey("Revision"))
      {
        doc.Add(new XElement("error", "No revision present in cpu info, cannot determine pi model"));
        Console.WriteLine(doc.ToString());
        return;
      }

      doc.Add(new XElement("version", _platformInfo["piVersion"]));
      doc.Add(new XElement("model", _platformInfo["modelName"]));
      doc.Add(new XElement("revision", _platformInfo["modelRevision"]));
      doc.Add(new XElement("memory", _platformInfo["modelMemory"]));
      doc.Add(new XElement("notes", _platformInfo["notes"]));

      Console.WriteLine(doc.ToString());
    }

    private static void DisplayCpuItem(string itemName, string defaultText = "Unknown")
    {
      Console.WriteLine(_cpuInfo.ContainsKey(itemName) ? _cpuInfo[itemName] : defaultText);
    }

    private static void DisplayPlatformItem(string itemName, string defaultText = "Unknown")
    {
      Console.WriteLine(_platformInfo.ContainsKey(itemName) ? _platformInfo[itemName] : defaultText);
    }

    private static void DisplayHardware()
    {
      DisplayCpuItem("Hardware");
    }

    private static void DisplaySerial()
    {
      DisplayCpuItem("Serial", "NotPresent");
    }

    private static void DisplayPiVersion()
    {
      DisplayPlatformItem("piVersion", "0");
    }

    private static void DisplayPiModel()
    {
      DisplayPlatformItem("modelName");
    }

    private static void DisplayPiRevision()
    {
      DisplayPlatformItem("modelRevision", "0");
    }

    private static void DisplayPiMemory()
    {
      DisplayPlatformItem("modelMemory", "0MB");
    }

    private static void ShowHelp()
    {
      Console.WriteLine("PID V1.00 (Raspberry Pi Identification Utility) written by Shawty/DS");
      Console.WriteLine();
      Console.WriteLine("Usage is as follows:");
      Console.WriteLine("pid <option>");
      Console.WriteLine();
      Console.WriteLine("Where option is one of the following, NOTE: ONLY one option can be used at a time, if you use multiple only the first one will be acted upon.");
      Console.WriteLine();
      Console.WriteLine("\t-CJ\t\tDisplay entire /proc/cpuinfo file as JSON formatted data");
      Console.WriteLine("\t-CX\t\tDisplay entire /proc/cpuinfo file as XML formatted data");
      Console.WriteLine("\t-PJ\t\tDisplay platform information as JSON formatted data");
      Console.WriteLine("\t-PX\t\tDisplay platform information as XML formatted data");
      Console.WriteLine();
      Console.WriteLine("The following options are used to get single responses suitable for use in shell scripts and other utilities:");
      Console.WriteLine("\t-H\t\tDisplay platform hardware");
      Console.WriteLine("\t-S\t\tDisplay platform serial number");
      Console.WriteLine("\t-V\t\tDisplay Raspberry Pi Version (1,2 or 3)");
      Console.WriteLine("\t-M\t\tDisplay Raspberry Pi Model");
      Console.WriteLine("\t-R\t\tDisplay Raspberry Pi Model Revision");
      Console.WriteLine("\t-C\t\tDisplay Raspberry Pi Memory Capacity");
    }

    public static void Main(string[] args)
    {
      LoadCpuInfo();
      DecodeRevision(_cpuInfo.ContainsKey("Revision") ? _cpuInfo["Revision"] : "0");

      if (args.Length != 1)
      {
        ShowHelp();
        return;
      }

      switch (args[0])
      {
        case "-CJ":
          DisplayCpuInfoAsJson();
          break;

        case "-CX":
          DisplayCpuInfoAsXml();
          break;

        case "-PJ":
          DisplayPlatformAsJson();
          break;

        case "-PX":
          DisplayPlatformAsXml();
          break;

        case "-H":
          DisplayHardware();
          break;

        case "-S":
          DisplaySerial();
          break;

        case "-V":
          DisplayPiVersion();
          break;

        case "-M":
          DisplayPiModel();
          break;

        case "-R":
          DisplayPiRevision();
          break;

        case "-C":
          DisplayPiMemory();
          break;

      }

    }
  }
}
