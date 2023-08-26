# Code2.Tools.Csv
Csv Reader / deserializer tools

## options
 - **Header**, array of strings with header names, default is null
 - **Delimiter**, cell delimiter char, default is ',';
 - **KeepEnclosureQuotes**, indicates whether to keep cell enclosure quotes, default is false
 - **Explicit**, explicit line checking compares header cell count to line cell count, default is false


## example 1: process plaint text
```
using TextReader reader = File.OpenText(filePath);
var csvReader = new CsvReader(reader);
csvReader.Options.Header = csvReader.ReadLine();
while(!csvReader.EndOfStream)
{
    string? line = csvReader.ReadLine();
    ProcessCsvLine(line, csvReader.CurrentLineNumber);
}
```

## example 2: process objects
```
using TextReader reader = File.OpenText(filePath);
var csvReader = new CsvReader<Poco>(reader);
csvReader.Options.Header = csvReader.ReadLine();
while(!csvReader.EndOfStream)
{
    Poco? item = csvReader.ReadObject();
    ProcessObject(item, csvReader.CurrentLineNumber);
}
```

## references
csv specs [rfc4180](https://datatracker.ietf.org/doc/html/rfc4180)