using Signature.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrongNamer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var keyFileName = Directory.EnumerateFiles(".", "*.snk").Concat(Directory.EnumerateFiles(".", "*.pfx")).FirstOrDefault();
                if (keyFileName == null)
                    throw new Exception("Could not find snk or pfx file in current directory.");
                var signer = new PackageSigner();
                foreach (var packageName in Directory.EnumerateFiles(".", "*.nupkg"))
                {
                    var info = new PackageInfo(packageName);
                    signer.SignPackage(packageName, info.SignedPackageName(), keyFileName, "", info.SignedIdentifier());
                    Console.WriteLine("Signed " + packageName + " as " + info.SignedPackageName());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.ReadKey();
        }

        private sealed class PackageInfo
        {
            public PackageInfo(string filename)
            {
                var parts = Path.GetFileNameWithoutExtension(filename).Split('.').ToList();
                var version = "";
                while (parts[parts.Count - 1].All(x => char.IsDigit(x)))
                {
                    version = "." + parts[parts.Count - 1] + version;
                    parts.RemoveAt(parts.Count - 1);
                }
                Version = version.Substring(1);
                Identifier = string.Join(".", parts);
            }

            public string Identifier { get; private set; }
            public string Version { get; private set; }

            public string SignedPackageName()
            {
                return Identifier + ".StrongNamed." + Version + ".nupkg";
            }

            public string SignedIdentifier()
            {
                return Identifier + ".StrongNamed";
            }
        }
    }
}
