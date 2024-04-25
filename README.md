# Phantom

Phantom (Crybat/Jlaive Rewrite) is an antivirus evasion tool that can convert executables to undetectable batch files, .NET/Native assemblies are not guaranteed to work.

## Changelog
- Updated UAC Bypass
- New BSTUB with AMSI Patch via Indirect Syscalls
- BSTUB Obfuscation upon Build
- WD Bypassed as of 4/25/2024

![image](https://raw.githubusercontent.com/sexyiam/Phantom/main/Images/Screenshot.png)

## TODO 
- Change BAT Obfuscation to evade YARA Rules
- Added new Obfuscation in Stub.ps1
- Remove usage of reflection in Stub.ps1, rather include shellcode allocation
- ~~Compile UAC Bypass with Rust to avoid fast sigging~~
- Remove usage of donut as it has basic mem loader or do morphing after passing through donut for native files
- Add rootkit
- Remove usage of .vbs(highly sigged)
- Add DLL Unhooking in BSTUB
- Better AMSI Patch
