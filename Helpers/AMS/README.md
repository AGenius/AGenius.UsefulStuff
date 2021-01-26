# AMS

This helper came from https://www.codeproject.com/Articles/5304/Read-Write-XML-files-Config-files-INI-files-or-the

It was originally made available in 2005 It is a nice compact way to create and access configuration files in a number of formats

i.e. XML, INI, CONFIG and even Registry

# Usage

```c#
using AGenius.UsefulStuff.AMS.Profile;

Xml profile = new Xml();
...
int width = profile.GetValue("Main Window", "Width", 800);
int height = profile.GetValue("Main Window", "Height", 600);
...
profile.SetValue("Main Window", "Width", this.Size.Width);
profile.SetValue("Main Window", "Height", this.Size.Height);
```