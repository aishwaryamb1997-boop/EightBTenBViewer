using System;
using System.Collections.Generic;
using EightBTenBViewer.Models;

namespace EightBTenBViewer.Services
{
    public class EightBTenBDecoder
    {
        private readonly Dictionary<string, SymbolInfo> _map = new();
        private readonly Dictionary<string, int> _sixToFive = new();
        private readonly Dictionary<string, int> _fourToThree = new();
        private readonly string[] _fourRdMinus7 = new string[32];
        private readonly string[] _fourRdPlus7 = new string[32];
        private readonly HashSet<string> _unknownPatterns = new();

        private readonly struct SymbolInfo
        {
            public SymbolInfo(string kind, string name, byte value)
            {
                Kind = kind;
                Name = name;
                Value = value;
            }

            public string Kind { get; }
            public string Name { get; }
            public byte Value { get; }
        }

        public EightBTenBDecoder()
        {
            InitializeMap();
        }

        private void InitializeMap()
        {
            string[] sixRdMinus =
            {
                "100111", "011101", "101101", "110001", "110101", "101001", "011001", "111000",
                "111001", "100101", "010101", "110100", "001101", "101100", "011100", "010111",
                "011011", "100011", "010011", "110010", "001011", "101010", "011010", "111010",
                "110011", "100110", "010110", "110110", "001110", "101110", "011110", "101011"
            };

            string[] sixRdPlus =
            {
                "011000", "100010", "010010", "110001", "001010", "101001", "011001", "000111",
                "000110", "100101", "010101", "110100", "001101", "101100", "011100", "101000",
                "100100", "100011", "010011", "110010", "001011", "101010", "011010", "000101",
                "001100", "100110", "010110", "001001", "001110", "010001", "100001", "010100"
            };

            string[] fourRdMinus = { "0100", "1001", "0101", "0011", "0010", "1010", "0110", "0001" };
            string[] fourRdPlus = { "1011", "1001", "0101", "1100", "1101", "1010", "0110", "1110" };

            string[] fourRdMinus7 =
            {
                "0001", "0001", "0001", "1110", "0001", "1110", "1110", "1110",
                "0001", "1110", "1110", "1110", "1110", "1110", "1110", "0001",
                "0001", "0111", "0111", "1110", "0111", "1110", "1110", "0001",
                "0001", "1110", "1110", "0001", "1110", "0001", "0001", "0001"
            };

            string[] fourRdPlus7 =
            {
                "1110", "1110", "1110", "0001", "1110", "0001", "0001", "0001",
                "1110", "0001", "0001", "1000", "0001", "1000", "1000", "1110",
                "1110", "0001", "0001", "0001", "0001", "0001", "0001", "1110",
                "1110", "0001", "0001", "1110", "0001", "1110", "1110", "1110"
            };

            Array.Copy(fourRdMinus7, _fourRdMinus7, 32);
            Array.Copy(fourRdPlus7, _fourRdPlus7, 32);

            for (int five = 0; five <= 31; five++)
            {
                _sixToFive[sixRdMinus[five]] = five;
                _sixToFive[sixRdPlus[five]] = five;
            }

            for (int three = 0; three <= 6; three++)
            {
                _fourToThree[fourRdMinus[three]] = three;
                _fourToThree[fourRdPlus[three]] = three;
            }

            for (int three = 0; three <= 6; three++)
            {
                for (int five = 0; five <= 31; five++)
                {
                    AddD(five, three, sixRdMinus[five] + fourRdMinus[three], sixRdPlus[five] + fourRdPlus[three]);
                }
            }

            for (int five = 0; five <= 31; five++)
            {
                AddD(five, 7, sixRdMinus[five] + fourRdMinus7[five], sixRdPlus[five] + fourRdPlus7[five]);
            }

            AddK("0011110100", "K28.0", (byte)((0 << 5) | 28));
            AddK("1100001011", "K28.0", (byte)((0 << 5) | 28));
            AddK("0011111001", "K28.1", (byte)((1 << 5) | 28));
            AddK("1100000110", "K28.1", (byte)((1 << 5) | 28));
            AddK("0011110101", "K28.2", (byte)((2 << 5) | 28));
            AddK("1100001010", "K28.2", (byte)((2 << 5) | 28));
            AddK("0011110011", "K28.3", (byte)((3 << 5) | 28));
            AddK("1100001100", "K28.3", (byte)((3 << 5) | 28));
            AddK("0011110010", "K28.4", (byte)((4 << 5) | 28));
            AddK("1100001101", "K28.4", (byte)((4 << 5) | 28));
            AddK("0011111010", "K28.5", (byte)((5 << 5) | 28));
            AddK("1100000101", "K28.5", (byte)((5 << 5) | 28));
            AddK("0011110110", "K28.6", (byte)((6 << 5) | 28));
            AddK("1100001001", "K28.6", (byte)((6 << 5) | 28));
            AddK("0011111000", "K28.7", (byte)((7 << 5) | 28));
            AddK("1100000111", "K28.7", (byte)((7 << 5) | 28));
            AddK("1110101000", "K23.7", (byte)((7 << 5) | 23));
            AddK("0001010111", "K23.7", (byte)((7 << 5) | 23));
            AddK("1101101000", "K27.7", (byte)((7 << 5) | 27));
            AddK("0010010111", "K27.7", (byte)((7 << 5) | 27));
            AddK("1011101000", "K29.7", (byte)((7 << 5) | 29));
            AddK("0100010111", "K29.7", (byte)((7 << 5) | 29));
            AddK("0111101000", "K30.7", (byte)((7 << 5) | 30));
            AddK("1000010111", "K30.7", (byte)((7 << 5) | 30));
        }

        private void AddD(int five, int three, string rdMinus, string rdPlus)
        {
            byte value = (byte)((three << 5) | five);
            string name = $"D{five}.{three}";
            AddSymbol(rdMinus, "D", name, value);
            AddSymbol(rdPlus, "D", name, value);
        }

        private void AddK(string bits10, string name, byte value)
        {
            AddSymbol(bits10, "K", name, value);
        }

        private void AddSymbol(string bits10, string kind, string name, byte value)
        {
            _map[bits10] = new SymbolInfo(kind, name, value);
        }

        public DecodedSymbol Decode(string bits10)
        {
            if (bits10.Length != 10)
            {
                return Error(bits10);
            }

            if (!IsBinary(bits10))
            {
                return Error(bits10);
            }

            if (_map.TryGetValue(bits10, out SymbolInfo info))
            {
                return new DecodedSymbol
                {
                    Bits10 = bits10,
                    Type = info.Name,
                    Hex = $"{info.Value:X2}"
                };
            }

            if (TryLooseDecode(bits10, out SymbolInfo looseInfo))
            {
                return new DecodedSymbol
                {
                    Bits10 = bits10,
                    Type = looseInfo.Name,
                    Hex = $"{looseInfo.Value:X2}"
                };
            }

            return Error(bits10);
        }

        private static bool IsBinary(string bits)
        {
            for (int i = 0; i < bits.Length; i++)
            {
                char c = bits[i];
                if (c != '0' && c != '1')
                {
                    return false;
                }
            }
            return true;
        }

        private DecodedSymbol Error(string bits)
        {
            _unknownPatterns.Add(bits);

            return new DecodedSymbol
            {
                Bits10 = bits,
                Type = "EE",
                Hex = "EE"
            };
        }

        public HashSet<string> GetUnknownPatterns()
        {
            return _unknownPatterns;
        }

        private bool TryLooseDecode(string bits10, out SymbolInfo info)
        {
            info = default;
            if (bits10.Length != 10)
            {
                return false;
            }

            string six = bits10.Substring(0, 6);
            string four = bits10.Substring(6, 4);

            if (!_sixToFive.TryGetValue(six, out int five))
            {
                return false;
            }

            if (_fourToThree.TryGetValue(four, out int three))
            {
                byte value = (byte)((three << 5) | five);
                info = new SymbolInfo("D", $"D{five}.{three}", value);
                return true;
            }

            if (four == _fourRdMinus7[five] || four == _fourRdPlus7[five])
            {
                byte value = (byte)((7 << 5) | five);
                info = new SymbolInfo("D", $"D{five}.7", value);
                return true;
            }

            return false;
        }
    }
}
