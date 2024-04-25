using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Obfuscate
{
    internal class Obfuscate
    {
        public static string random_string(int length)
        {
            string text;
            do
            {
                text = new string((from s in Enumerable.Repeat<string>("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", length)
                                   select s[Obfuscate.random.Next(s.Length)]).ToArray<char>());
            }
            while (Obfuscate.names.Contains(text));
            return text;
        }

        public static void clean_asm(ModuleDef md)
        {
            foreach (TypeDef typeDef in md.GetTypes())
            {
                foreach (MethodDef methodDef in typeDef.Methods)
                {
                    if (methodDef.HasBody)
                    {
                        methodDef.Body.SimplifyBranches();
                        methodDef.Body.OptimizeBranches();
                    }
                }
            }
        }

        public static void obfuscate_strings(ModuleDef md)
        {
            foreach (TypeDef typeDef in md.GetTypes())
            {
                foreach (MethodDef methodDef in typeDef.Methods)
                {
                    if (methodDef.HasBody)
                    {
                        for (int i = 0; i < methodDef.Body.Instructions.Count<Instruction>(); i++)
                        {
                            if (methodDef.Body.Instructions[i].OpCode == OpCodes.Ldstr)
                            {
                                string text = methodDef.Body.Instructions[i].Operand.ToString();
                                string text2 = Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
                                Console.WriteLine(text + " -> " + text2);
                                methodDef.Body.Instructions[i].OpCode = OpCodes.Nop;
                                methodDef.Body.Instructions.Insert(i + 1, new Instruction(OpCodes.Call, md.Import(typeof(Encoding).GetMethod("get_UTF8", new Type[0]))));
                                methodDef.Body.Instructions.Insert(i + 2, new Instruction(OpCodes.Ldstr, text2));
                                methodDef.Body.Instructions.Insert(i + 3, new Instruction(OpCodes.Call, md.Import(typeof(Convert).GetMethod("FromBase64String", new Type[]
                                {
                                    typeof(string)
                                }))));
                                methodDef.Body.Instructions.Insert(i + 4, new Instruction(OpCodes.Callvirt, md.Import(typeof(Encoding).GetMethod("GetString", new Type[]
                                {
                                    typeof(byte[])
                                }))));
                                i += 4;
                            }
                        }
                    }
                }
            }
        }

        public static void obfuscate_methods(ModuleDef md)
        {
            foreach (TypeDef typeDef in md.GetTypes())
            {
                foreach (MethodDef methodDef in typeDef.Methods)
                {
                    if (methodDef.HasBody && !methodDef.IsConstructor && !methodDef.HasOverrides && !methodDef.IsRuntimeSpecialName && !methodDef.DeclaringType.IsForwarder)
                    {
                        string text = Obfuscate.random_string(10);
                        Console.WriteLine(string.Format("{0} -> {1}", methodDef.Name, text));
                        methodDef.Name = text;
                    }
                }
            }
        }

        public static void obfuscate_resources(ModuleDef md, string name, string encName)
        {
            if (name == "")
            {
                return;
            }
            foreach (Resource resource in md.Resources)
            {
                string text = resource.Name.Replace(name, encName);
                if (resource.Name != text)
                {
                    Console.WriteLine(string.Format("{0} -> {1}", resource.Name, text));
                }
                resource.Name = text;
            }
        }

        public static void obfuscate_classes(ModuleDef md)
        {
            foreach (TypeDef typeDef in md.GetTypes())
            {
                string text = Obfuscate.random_string(10);
                Console.WriteLine(string.Format("{0} -> {1}", typeDef.Name, text));
                Obfuscate.obfuscate_resources(md, typeDef.Name, text);
                typeDef.Name = text;
            }
        }

        public static void obfuscate_namespace(ModuleDef md)
        {
            foreach (TypeDef typeDef in md.GetTypes())
            {
                string text = Obfuscate.random_string(10);
                Console.WriteLine(string.Format("{0} -> {1}", typeDef.Namespace, text));
                Obfuscate.obfuscate_resources(md, typeDef.Namespace, text);
                typeDef.Namespace = text;
            }
        }

        public static void obfuscate_assembly_info(ModuleDef md)
        {
            string text = Obfuscate.random_string(10);
            Console.WriteLine(string.Format("{0} -> {1}", md.Assembly.Name, text));
            md.Assembly.Name = text;
            string[] source = new string[]
            {
                "AssemblyDescriptionAttribute",
                "AssemblyTitleAttribute",
                "AssemblyProductAttribute",
                "AssemblyCopyrightAttribute",
                "AssemblyCompanyAttribute",
                "AssemblyFileVersionAttribute"
            };
            foreach (CustomAttribute customAttribute in md.Assembly.CustomAttributes)
            {
                if (source.Any(new Func<string, bool>(customAttribute.AttributeType.Name.Contains)))
                {
                    string text2 = Obfuscate.random_string(10);
                    Console.WriteLine(string.Format("{0} = {1}", customAttribute.AttributeType.Name, text2));
                    customAttribute.ConstructorArguments[0] = new CAArgument(md.CorLibTypes.String, new UTF8String(text2));
                }
            }
        }

        private static void obfuscate(string inFile, string outFile)
        {
            if (inFile == "" || outFile == "")
            {
                return;
            }
            ModuleDefMD moduleDefMD = ModuleDefMD.Load(inFile);
            moduleDefMD.Name = Obfuscate.random_string(10);
            Obfuscate.obfuscate_strings(moduleDefMD);
            Obfuscate.obfuscate_methods(moduleDefMD);
            Obfuscate.obfuscate_classes(moduleDefMD);
            Obfuscate.obfuscate_namespace(moduleDefMD);
            Obfuscate.obfuscate_assembly_info(moduleDefMD);
            Obfuscate.clean_asm(moduleDefMD);
            moduleDefMD.Write(outFile);
        }

        private static void Main(string inFile = "", string outFile = "")
        {
            if (outFile == "")
            {
                outFile = inFile + ".Obfuscated";
            }
            Obfuscate.obfuscate(inFile, outFile);
        }

        private static Random random = new Random();

        private static List<string> names = new List<string>();
    }
}
