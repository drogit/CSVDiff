using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DiffExtract
{
    class Program
    {
        static CsvWriter _fileOutWriter;

        static void Main(string[] args)
        {
            if (args.Length != 3) Console.WriteLine("Invalid arguments - [old] [new] [output]");
            if (!File.Exists(args[0])) Console.WriteLine("Old file not found");
            if (!File.Exists(args[1])) Console.WriteLine("New file not found");

            var file1Stream = File.OpenText(args[0]);
            var file1Reader = new CsvReader(file1Stream)
            {
                Configuration = { HasHeaderRecord = true }
            };
            List<string[]> file1Records = new List<string[]>();
            while (file1Reader.Read())
            {
                file1Records.Add(file1Reader.CurrentRecord);
            }

            var file2Stream = File.OpenText(args[1]);
            var file2Reader = new CsvReader(file2Stream)
            {
                Configuration = { HasHeaderRecord = true }
            };
            List<string[]> file2Records = new List<string[]>();
            while (file2Reader.Read())
            {
                file2Records.Add(file2Reader.CurrentRecord);
            }

            var fileOutStream = File.CreateText(args[2]);
            _fileOutWriter = new CsvWriter(fileOutStream);

            OutputRecord(file1Reader.FieldHeaders);

            foreach (var file2Record in file2Records)
            {
                var file1Record = file1Records.FirstOrDefault(x => x[0] == file2Record[0]);

                if (file1Record == null)
                {
                    OutputRecord(file2Record);
                    continue;
                }

                for (int i = 1; i < file2Record.Length; i++)
                {
                    if (file1Record[i] != file2Record[i])
                    {
                        OutputRecord(file2Record);
                        break;
                    }
                }

            }

            fileOutStream.Flush();
            fileOutStream.Close();
        }

        private static void OutputRecord(string[] record)
        {
            foreach (string s in record)
            {
                _fileOutWriter.WriteField(s);
            }
            _fileOutWriter.NextRecord();
        }
    }
}
