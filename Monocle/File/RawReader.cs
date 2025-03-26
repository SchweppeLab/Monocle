using Monocle.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using ThermoBiz = ThermoFisher.CommonCore.Data.Business;
using ThermoFisher.CommonCore.Data.FilterEnums;
using ThermoFisher.CommonCore.Data.Interfaces;
using ThermoFisher.CommonCore.RawFileReader;
using Nova.Io.Read;

namespace Monocle.File
{
  public class RawReader : IScanReader
  {
    private ISpectrumFileReader Reader;
    private string FilePath;
    private ScanFileHeader Header = new ScanFileHeader();

    /// <summary>
    /// Open new Raw file with warning messages.
    /// </summary>
    /// <param name="path"></param>
    public void Open(string path, ScanReaderOptions options)
    {
      if (!System.IO.File.Exists(path))
      {
        throw new IOException("File not found: " + path);
      }
      FilePath = path;
      MSFilter filter = new MSFilter();
      filter = MSFilter.MS1 | MSFilter.MS2 | MSFilter.MS3;
      Reader = SpectrumFileReaderFactory.GetReader(FilePath, filter);
      //ReadHeader();
    }

    public ScanFileHeader GetHeader()
    {
      Header.FilePath = FilePath;
      //Header.StartTime = (float) rawFile.RunHeaderEx.StartTime;
      //Header.EndTime = (float) rawFile.RunHeaderEx.EndTime;
      //Header.AcquisitionDate = rawFile.CreationDate;
      Header.ScanCount = Reader.ScanCount;
      //Header.InstrumentModel = rawFile.GetInstrumentData().Model;
      Header.InstrumentManufacturer = "ThermoFisher";
      return Header;
    }

    /// <summary>
    /// Dispose of the raw file when reading multiple files.
    /// </summary>
    public void Close()
    {
      Reader.Close();
    }

    /// <summary>
    /// Open the given file and import scans into the reader.
    /// </summary>
    /// <returns></returns>
    public System.Collections.IEnumerator GetEnumerator()
    {
      yield return Reader.GetEnumerator();

    }
  }
}
