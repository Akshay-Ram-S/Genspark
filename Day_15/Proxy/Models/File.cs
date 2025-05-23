using System;
using System.IO;
using Proxy.interfaces;

namespace Proxy.models
{
    public class File : IFile
    {
        private readonly string filePath;

        public File(string path)
        {
            filePath = path;
        }

        public void Read()
        {
            if (System.IO.File.Exists(filePath))
            {
                string content = System.IO.File.ReadAllText(filePath);
                Console.WriteLine($"File Content:\n{content}");
            }
            else
            {
                Console.WriteLine("File not found.");
            }
        }

        public void ReadMetaData()
        {
            if (System.IO.File.Exists(filePath))
            {
                FileInfo fileInfo = new FileInfo(filePath);
                Console.WriteLine("File Metadata:");
                Console.WriteLine($"Name: {fileInfo.Name}");
                Console.WriteLine($"Path: {fileInfo.FullName}");
                Console.WriteLine($"Size: {fileInfo.Length} bytes");
                Console.WriteLine($"Created: {fileInfo.CreationTime}");
            }
            else
            {
                Console.WriteLine("File not found.");
            }
        }
    }
}