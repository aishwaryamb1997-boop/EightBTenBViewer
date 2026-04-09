using System.Collections.ObjectModel;
using EightBTenBViewer.Models;
using EightBTenBViewer.Services;

namespace EightBTenBViewer.ViewModels
{
    public class MainViewModel
    {
        public ObservableCollection<TraceRecord> Records { get; set; } = new();

        public MainViewModel()
        {
            LoadData();
        }

        private void LoadData()
        {
            var reader = new FileReaderService();
            var decoder = new EightBTenBDecoder();
            var processor = new BitProcessorService(decoder);

            var data = reader.ReadFile(@"C:\Users\Prodigy technovation\source\MIL1553\EightBTenBViewer\vc.txt");

            int sl = 1;

            foreach (var item in data)
            {
                var lanes = processor.Process(item.Bits);

                Records.Add(new TraceRecord
                {
                    SerialNo = sl++,
                    Timestamp = item.Timestamp,
                    Lanes = lanes
                });
            }
        }
    }
}