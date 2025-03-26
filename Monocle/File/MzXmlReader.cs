using Monocle.Data;
using Nova.Io.Read;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Monocle.File
{
    public class MzXmlReader : IScanReader
    {
        private ISpectrumFileReader Reader;
        
        private ScanFileHeader Header = new ScanFileHeader();

        private string FilePath;

        //#region Attributes for the MZXML file
        //public Dictionary<string, string> mzxmlAttributes = new Dictionary<string, string>()
        //{
        //    { "num" , "ScanNumber" },
        //    { "msLevel" , "MsOrder" },
        //    { "scanEvent" , "ScanEvent" },
        //    { "masterIndex" , "MasterIndex" },
        //    { "peaksCount" , "PeakCount" },
        //    { "ionInjectionTime" , "IonInjectionTime" },
        //    { "elapsedScanTime" , "ElapsedScanTime" },
        //    { "polarity" , "Polarity" },
        //    { "scanType" , "ScanType" },
        //    { "filterLine" , "FilterLine" },
        //    { "description" , "Description" },
        //    { "startMz","StartMz" },
        //    { "endMz","EndMz" },
        //    { "lowMz","LowestMz" },
        //    { "highMz","HighestMz" },
        //    { "basePeakMz","BasePeakMz" },
        //    { "basePeakIntensity","BasePeakIntensity" },
        //    { "faimsVoltageOn","FaimsVoltageOn" },
        //    { "faimsCv","FaimsCV" }
        //};

        //public Dictionary<string, string> mzxmlMsnAttributes = new Dictionary<string, string>()
        //{
        //    { "totIonCurrent","TotalIonCurrent" },
        //    { "collisionEnergy","CollisionEnergy" }
        //};

        //public Dictionary<string, string> mzxmlPrecursorAttributes = new Dictionary<string, string>()
        //{
        //    // Precusor information
        //    { "precursorScanNum","PrecursorMasterScanNumber" },
        //    { "activationMethod","PrecursorActivationMethod" }
        //};

        //public Dictionary<string, string> mzxmlPeaksAttributes = new Dictionary<string, string>()
        //{
        //    // Peaks information
        //    { "precision","PeaksPrecision" },
        //    { "byteOrder","PeaksByteOrder" },
        //    { "contentType","PeaksContentType" },
        //    { "compressionType", "PeaksCompressionType" },
        //    { "compressedLen", "PeaksCompressedLength" }
        //};
        //#endregion

        /// <summary>
        /// Open new fileStream to mzXML file.
        /// </summary>
        /// <param name="path"></param>
        public void Open(string path, ScanReaderOptions options)
        {
            if (!System.IO.File.Exists(path)) {
                throw new IOException("File not found: " + path);
            }
            FilePath = path;
      MSFilter filter = new MSFilter();
      filter = MSFilter.MS1 | MSFilter.MS2 | MSFilter.MS3;
      Reader = SpectrumFileReaderFactory.GetReader(FilePath,filter);
            ReadHeader();
        }

        /// <summary>
        /// Returns header information from the mzXML file.
        /// </summary>
        /// <returns>An instance of the ScanFileHeader class</returns>
        public ScanFileHeader GetHeader() {
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
        public IEnumerator GetEnumerator() {
      yield return Reader.GetEnumerator();
      
        }

        private void ReadHeader() {
            Header.FileName = Path.GetFileName(FilePath);
      Header.ScanCount = Reader.ScanCount;
      Header.StartTime = 0;
      Header.EndTime = 0;

        }
   }
}
