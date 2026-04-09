using System;
using System.Collections.Generic;
using EightBTenBViewer.Models;

namespace EightBTenBViewer.Services
{
    public class BitProcessorService
    {
        // 🔹 Added decoder instance
        private readonly EightBTenBDecoder _decoder;
        public BitProcessorService(EightBTenBDecoder decoder)
        {
            _decoder = decoder;
        }

        public List<Lane> Process(string bits640)
        {
            var lanes = new List<Lane>();

            // Step 1: Split into 16 lanes (RIGHT → LEFT)
            for (int i = 0; i < 16; i++)
            {
                int start = bits640.Length - ((i + 1) * 40);
                string laneBits = bits640.Substring(start, 40);

                var lane = ProcessLane(laneBits);
                lanes.Insert(0, lane); // maintain L0 → L15 order
            }

            return lanes;
        }
        

        private Lane ProcessLane(string bits40)
        {
            var symbols = new List<DecodedSymbol>();

            // Step 2: Split 40 → 4 chunks (RIGHT → LEFT)
            for (int i = 0; i < 4; i++)
            {
                int start = bits40.Length - ((i + 1) * 10);
                string bits10 = bits40.Substring(start, 10);

                string reversed = Reverse(bits10);

                // 🔹 Decode using 8b/10b decoder
                var decoded = _decoder.Decode(reversed);

                // 🔹 Ensure original 10-bit value is preserved
                decoded.Bits10 = reversed;

                // Keep right-to-left order so index 0 shows the first (rightmost) 10b chunk.
                symbols.Add(decoded);
            }

            return new Lane { Symbols = symbols };
        }

        private string Reverse(string input)
        {
            char[] arr = input.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }
    }
}
