using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionTest.ConsoleInterop
{
    internal static class StringValidation
    {
        // a complete list of C# keywords (as of C# 10/.NET 10)
        private static readonly HashSet<string> CSharpKeywords = new(StringComparer.Ordinal)
        {
            "abstract","as","base","bool","break","byte","case","catch","char","checked","class",
            "const","continue","decimal","default","delegate","do","double","else","enum","event",
            "explicit","extern","false","finally","fixed","float","for","foreach","goto","if",
            "implicit","in","int","interface","internal","is","lock","long","namespace","new",
            "null","object","operator","out","override","params","private","protected","public",
            "readonly","ref","return","sbyte","sealed","short","sizeof","stackalloc","static",
            "string","struct","switch","this","throw","true","try","typeof","uint","ulong",
            "unchecked","unsafe","ushort","using","virtual","void","volatile","while","record",
            // contextual keywords you may also want to exclude
            "add","alias","and","await","async","dynamic","file","global","init","let","nameof",
            "not","or","partial","remove","select","set","unmanaged","when","where","yield"
        };

        public static bool IsValidCSharpIdentifier(string id)
        {
            // Not null/empty or just whitespace:
            if (string.IsNullOrWhiteSpace(id))
                return false;

            // First character must be letter or underscore:
            if (!(char.IsLetter(id[0]) || id[0] == '_'))
                return false;

            // All characters must be letter, digit or underscore, and must not be any whitespace:
            foreach (var c in id)
            {
                if (!(char.IsLetterOrDigit(c) || c == '_'))
                    return false;
            }

            // Must not be a C# keyword:
            if (CSharpKeywords.Contains(id))
                return false;

            return true;
        }

    }
}
