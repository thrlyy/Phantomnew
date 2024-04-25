using System;
using System.Collections.Generic;
using System.Linq;

namespace ScrubCrypt
{
    internal class Randomiser
    {
        public static Random RNG
        {
            get
            {
                if (Randomiser._rng == null)
                {
                    Randomiser._rng = new Random();
                }
                return Randomiser._rng;
            }
        }

        public static string GetString(int length)
        {
            string text = string.Empty;
            while (text == string.Empty)
            {
                string stringUnsafe = Randomiser.GetStringUnsafe(length);
                text = (Randomiser._usedStrings.Contains(stringUnsafe, StringComparer.OrdinalIgnoreCase) ? string.Empty : stringUnsafe);
            }
            Randomiser._usedStrings.Add(text);
            return text;
        }

        public static string GetStringUnsafe(int length)
        {
            return new string((from s in Enumerable.Repeat<string>("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ", length)
                               select s[Randomiser.RNG.Next(s.Length)]).ToArray<char>());
        }

        public static string[] GetStrings(int length, int amount)
        {
            List<string> list = new List<string>();
            for (int i = 0; i < amount; i++)
            {
                list.Add(Randomiser.GetString(length));
            }
            return list.ToArray();
        }

        public static void ClearStrings()
        {
            Randomiser._usedStrings.Clear();
        }

        private static Random _rng;

        private const string _chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private static readonly List<string> _usedStrings = new List<string>();
    }
}
