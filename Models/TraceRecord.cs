using System.Collections.Generic;

namespace EightBTenBViewer.Models
{
    public class TraceRecord
    {
        public int SerialNo { get; set; }
        public string Timestamp { get; set; } = string.Empty;
        public List<Lane> Lanes { get; set; } = new();
    }

    public class Lane
    {
        public List<DecodedSymbol> Symbols { get; set; } = new();
    }

    public class DecodedSymbol
    {
        public string Bits10 { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Hex { get; set; } = string.Empty;
    }

    public class TraceRow
    {
        public int SerialNo { get; set; }
        public string Timestamp { get; set; } = string.Empty;
        public int RowIndex { get; set; }
        public List<DecodedSymbol> LaneSymbols { get; set; } = new();
    }
}
