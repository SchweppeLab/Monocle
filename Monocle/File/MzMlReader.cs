using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using Monocle.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using Nova.Data;
using Nova.Io.Read;

namespace Monocle.File
{
  public class MzMlReader : IScanReader
  {
    private ISpectrumFileReader Reader;

    private ScanFileHeader Header = new ScanFileHeader();

    private string FilePath;

    //public Dictionary<string, string> scanAttrs = new Dictionary<string, string>()
    //    {
    //        { "ms level", "MsOrder" },
    //        { "total ion current", "TotalIonCurrent" },
    //        { "scan start time", "RetentionTime" }, // Time is in minutes.
    //        { "collision energy", "CollisionEnergy" },
    //        { "base peak m/z", "BasePeakMz" },
    //        { "base peak intensity", "BasePeakIntensity" },
    //        { "scan window lower limit", "StartMz" },
    //        { "scan window upper limit", "EndMz" },
    //        { "lowest observed m/z", "LowestMz" },
    //        { "highest observed m/z", "HighestMz" },
    //        { "filter string", "FilterLine" }
    //    };

    //public Dictionary<string, string> precursorAttrs = new Dictionary<string, string>()
    //    {
    //        // Precusor information
    //        { "selected ion m/z", "Mz" },
    //        { "charge state", "Charge" }
    //    };

    /// <summary>
    /// Open new fileStream to mzML file.
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
      Reader = SpectrumFileReaderFactory.GetReader(path, filter);
      // ReadHeader();
    }

    /// <summary>
    /// Returns header information from the mzXML file.
    /// </summary>
    /// <returns>An instance of the ScanFileHeader class</returns>
    public ScanFileHeader GetHeader()
    {
      return Header;
    }

    /// <summary>
    /// Dispose of the reader when reading multiple files.
    /// </summary>
    public void Close()
    {
      Reader.Close();
    }

    /// <summary>
    /// Open the given file and import scans into the reader.
    /// </summary>
    /// <returns></returns>
    public IEnumerator GetEnumerator()
    {
      yield return Reader.GetEnumerator();
    }

    private void ReadHeader()
    {
      Header.FileName = Path.GetFileName(FilePath);
      Header.ScanCount = Reader.ScanCount;

    }



    private void Cleanup()
    {
      if (Reader != null)
      {
        ((IDisposable)Reader).Dispose();
      }
    }

  }


}
