using System;
using System.IO;

public sealed class FileManager
{
    private static FileManager _instance;
    private static readonly object _lock = new object();
    private StreamWriter _writer;
    private StreamReader _reader;
    private FileStream _fileStream;

    private FileManager(string filePath)
    {
        _fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        _writer = new StreamWriter(_fileStream);
        _reader = new StreamReader(_fileStream);
    }

    public static FileManager GetInstance(string filePath)
    {
        lock (_lock)
        {
            if (_instance == null)
            {
                _instance = new FileManager(filePath);
            }
        }
        return _instance;
    }

    public void WriteToFile(string data)
    {
        _writer.WriteLine(data);
        _writer.Flush();
    }

    public string ReadFromFile()
    {
        _fileStream.Seek(0, SeekOrigin.Begin); // Reset file pointer
        return _reader.ReadToEnd();
    }

    public void CloseFile()
    {
        _writer.Close();
        _reader.Close();
        _fileStream.Close();
    }
}

class Program
{
    static void Main()
    {
        string filePath = "example.txt";
        
        FileManager fileManager = FileManager.GetInstance(filePath);
        
        fileManager.WriteToFile("Singleton Pattern");
        
        Console.WriteLine("File Content:");
        Console.WriteLine(fileManager.ReadFromFile());
        
        fileManager.CloseFile();
    }
}