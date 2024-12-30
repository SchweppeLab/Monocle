
using System;
using Monocle.Data;
using ProjectMIDAS.Data.Spectrum;
using ProjectMIDAS.Data;

namespace Monocle.File {
    class ExtendedMzXmlWriter : MzXmlWriter {
        public override void WriteScan(Spectrum scan) {
            writer.WriteStartElement("scan");

            // Get position of scan tag for index.
            // Flush after writing since the previous tag isnt closed
            // until the scan tag is written.
            writer.Flush();
            long pos = output.BaseStream.Position - 5; // pos - length of "<scan"
            scanIndex.Add(scan.ScanNumber, pos);

            writer.WriteAttributeString("num", scan.ScanNumber.ToString());
            writer.WriteAttributeString("msLevel", scan.MsLevel.ToString());
            writer.WriteAttributeString("scanEvent", scan.ScanEvent.ToString());
            writer.WriteAttributeString("masterIndex", scan.PrecursorMasterScanNumber.ToString());
            writer.WriteAttributeString("peaksCount", scan.Count().ToString());
            writer.WriteAttributeString("ionInjectionTime", scan.IonInjectionTime.ToString());
            writer.WriteAttributeString("elapsedScanTime", scan.ElapsedScanTime.ToString());
            writer.WriteAttributeString("polarity", scan.Polarity ? "+" : "-");
      writer.WriteAttributeString("scanType", "scanType"); // scan.ScanType.ToString());
            writer.WriteAttributeString("filterLine", scan.ScanFilter);
            //if (scan.Description.Length > 0) {
            //    writer.WriteAttributeString("description", scan.Description);
            //}
            writer.WriteAttributeString("retentionTime", MakeRetentionTimeString(scan.RetentionTime));
            writer.WriteAttributeString("startMz", scan.StartMz.ToString());
            writer.WriteAttributeString("endMz", scan.EndMz.ToString());
            writer.WriteAttributeString("lowMz", scan.LowestMz.ToString());
            writer.WriteAttributeString("highMz", scan.HighestMz.ToString());
            writer.WriteAttributeString("basePeakMz", scan.BasePeakMz.ToString());
            writer.WriteAttributeString("basePeakIntensity", scan.BasePeakIntensity.ToString());
            writer.WriteAttributeString("totIonCurrent", scan.TotalIonCurrent.ToString());
            //writer.WriteAttributeString("faimsState", scan.FaimsState.ToString());
            writer.WriteAttributeString("compensationVoltage", scan.FaimsCV.ToString());

            //tSIM/MSX methods could be MS1s with "SPS" ions so no ms order consideration here
            if (scan.MsLevel > 1 && scan.Precursors.Count > 0)
            {
                writer.WriteAttributeString("collisionEnergy", scan.CollisionEnergy.ToString());
                foreach (PrecursorIon precursor in scan.Precursors)
                {
                    writer.WriteStartElement("precursorMz");
                    writer.WriteAttributeString("precursorScanNum", scan.PrecursorMasterScanNumber.ToString());
                    writer.WriteAttributeString("precursorIntensity", precursor.Intensity.ToString());
                    writer.WriteAttributeString("precursorCharge", precursor.Charge.ToString());
                    //writer.WriteAttributeString("activationMethod", scan.PrecursorActivationMethod.ToString());
                    writer.WriteAttributeString("isolationWidth", precursor.IsolationWidth.ToString());
                    writer.WriteAttributeString("isolationMz", precursor.IsolationMz.ToString());
                    writer.WriteString(precursor.MonoisotopicMz.ToString());
                    writer.WriteEndElement(); // precursorMz
                }
            }

            writer.WriteStartElement("peaks");
            writer.WriteAttributeString("precision", "32");
            writer.WriteAttributeString("byteOrder", "network");
            writer.WriteAttributeString("contentType", "m/z-int");
            writer.WriteAttributeString("compressionType", "none");
            writer.WriteAttributeString("compressedLen", "0");
            writer.WriteString(EncodePeaks(scan));
            writer.WriteEndElement(); // peaks

            writer.WriteStartElement("labelData");
            writer.WriteAttributeString("precision", "32");
            writer.WriteAttributeString("byteOrder", "network");
            writer.WriteAttributeString("contentType", "m/z-int");
            writer.WriteAttributeString("compressionType", "none");
            writer.WriteAttributeString("compressedLen", "0");
            writer.WriteString(EncodeLabelData(scan));
            writer.WriteEndElement(); // peaks

            writer.WriteEndElement(); // scan
        }

        /// <summary>
        /// Saves mz, baseline, and noise in base64, 32bit, little-endian
        /// </summary>
        /// <param name="scan"></param>
        /// <returns></returns>
        protected string EncodeLabelData(Spectrum scan) {
            if (scan.Count() == 0 ){//|| (scan.Centroids[0].Noise == 0 && scan.Centroids[0].Baseline == 0)) {
                return "AAAAAAAAAAA=";
            }
            
            // Allocate space for mz, baseline, and noise triplets, four bytes each.
            byte[] bytes = new byte[scan.Count() * 3 * 4];

            for (int i = 0; i < scan.Count(); ++i) {
                var peak = scan.DataPoints[i];
                var mzBytes = BitConverter.GetBytes((float)peak.Mz);
                Array.Reverse(mzBytes);
                mzBytes.CopyTo(bytes, i * 12);

        //var baselineBytes = BitConverter.GetBytes((float)peak.Baseline);
        var baselineBytes = BitConverter.GetBytes(0);
                Array.Reverse(baselineBytes);
                baselineBytes.CopyTo(bytes, (i * 12) + 4);

                //var noiseBytes = BitConverter.GetBytes((float)peak.Noise);
                var noiseBytes = BitConverter.GetBytes(0);
                Array.Reverse(noiseBytes);
                noiseBytes.CopyTo(bytes, (i * 12) + 8);
            }
            return Convert.ToBase64String(bytes);
        }

    }
}