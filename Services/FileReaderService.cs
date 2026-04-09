using System;
using System.Collections.Generic;
using System.IO;

namespace EightBTenBViewer.Services
{
    public class FileReaderService
    {
        public List<(string Timestamp, string Bits)> ReadFile(string filePath)
        {
            var result = new List<(string, string)>();

            var lines = File.ReadAllLines(filePath);

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var parts = line.Split(':');

                if (parts.Length != 2)
                    continue; // skip invalid lines

                var timestamp = parts[0].Trim();
                var bits = parts[1].Trim();

                // Validate 640 bits
                if (bits.Length != 640)
                    continue;

                result.Add((timestamp, bits));
            }

            return result;
        }
    }
}