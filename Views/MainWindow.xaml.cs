using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using EightBTenBViewer.Models;
using EightBTenBViewer.ViewModels;

namespace EightBTenBViewer.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void ExportCsv_Click(object sender, RoutedEventArgs e)
        {
            var records = GetRecords();
            if (records == null || records.Count == 0)
            {
                MessageBox.Show("No records to export.", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dialog = new SaveFileDialog
            {
                Filter = "CSV (*.csv)|*.csv",
                FileName = "EightBTenB.csv"
            };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            File.WriteAllText(dialog.FileName, BuildCsv(records), Encoding.UTF8);
            MessageBox.Show("CSV export completed.", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private ObservableCollection<TraceRecord>? GetRecords()
        {
            if (DataContext is MainViewModel vm)
            {
                return vm.Records;
            }

            return null;
        }

        private static string BuildCsv(ObservableCollection<TraceRecord> records)
        {
            var sb = new StringBuilder();
            sb.Append("SL,Timestamp");
            for (int lane = 0; lane < 16; lane++)
            {
                sb.Append($",L{lane}");
            }
            sb.AppendLine();

            foreach (var record in records)
            {
                for (int row = 0; row < 4; row++)
                {
                    if (row == 0)
                    {
                        sb.Append(EscapeCsv(record.SerialNo.ToString()));
                        sb.Append(",");
                        sb.Append(EscapeCsv(record.Timestamp));
                    }
                    else
                    {
                        sb.Append(",");
                        sb.Append("");
                    }

                    for (int lane = 0; lane < 16; lane++)
                    {
                        sb.Append(",");
                        sb.Append(EscapeCsv(FormatLaneRow(record, lane, row)));
                    }

                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }

        private static string EscapeCsv(string value)
        {
            if (value.Contains("\""))
            {
                value = value.Replace("\"", "\"\"");
            }

            if (value.Contains(",") || value.Contains("\n") || value.Contains("\r"))
            {
                return $"\"{value}\"";
            }

            return value;
        }

        private static string FormatLaneRow(TraceRecord record, int lane, int row)
        {
            if (record.Lanes == null || lane < 0 || lane >= record.Lanes.Count)
            {
                return string.Empty;
            }

            var symbols = record.Lanes[lane].Symbols;
            if (symbols == null || symbols.Count == 0 || row < 0 || row >= symbols.Count)
            {
                return string.Empty;
            }

            var symbol = symbols[row];
            return $"{symbol.Bits10} {symbol.Type} {symbol.Hex}";
        }

    }
}
