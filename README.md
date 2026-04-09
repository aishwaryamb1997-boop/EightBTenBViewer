# EightBTenBViewer

## Project Summary
EightBTenBViewer is a WPF desktop tool that reads a 640-bit trace file, splits each record into 16 lanes, decodes 8b/10b symbols, and displays the results in a grid. It also supports exporting the decoded view to CSV.

## Features
- Reads trace records from a text file with `timestamp: <640-bit>` lines.
- Splits each 640-bit record into 16 lanes (40 bits each), then into 4 symbols (10 bits each).
- Decodes symbols as D/K or error markers using an 8b/10b decode table.
- Displays decoded symbols per lane in a WPF DataGrid.
- Exports the decoded view to CSV.

## Input Format
Each non-empty line should be:
```
<Timestamp>: <640-bit binary string>
```
Lines with missing fields or invalid bit length are skipped.

## How It Works (High Level)
- `FileReaderService` loads the trace file and validates each line.
- `BitProcessorService` splits 640-bit strings into 16 lanes and 10-bit symbols.
- `EightBTenBDecoder` decodes 10-bit symbols into D/K types and hex values.
- `MainWindow` shows results and allows CSV export.

## Build and Run
Prerequisites:
- Windows
- .NET 8 SDK

Build:
```
dotnet build
```

Run:
```
dotnet run
```

## Configure the Input File
The input file path is currently hardcoded in `MainViewModel`:
```
C:\Users\Prodigy technovation\source\MIL1553\EightBTenBViewer\vc.txt
```
Update that path to point to your local trace file if needed.

## CSV Export
Click **Export CSV** in the UI to save the decoded grid to a `.csv` file.
