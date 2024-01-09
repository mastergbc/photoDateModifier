#region License
//
// Copyright 2002-2017 Drew Noakes
// Ported from Java to C# by Yakov Danilov for Imazen LLC in 2014
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
// More information about this project is available at:
//
//    https://github.com/drewnoakes/metadata-extractor-dotnet
//    https://drewnoakes.com/code/exif/
//
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.IO;
using System.Xml;
using System.Xml.Linq;
using MetadataExtractor.Formats.Xmp;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using XmpCore;

#if true //NET35
using FragmentList = System.Collections.Generic.IList<MetadataExtractor.Formats.Jpeg.JpegFragment>;
//using DirectoryList = System.Collections.Generic.IList<MetadataExtractor.Directory>;
#else
using DirectoryList = System.Collections.Generic.IReadOnlyList<MetadataExtractor.Directory>;
#endif

namespace MetadataExtractor.Formats.Iptc
{
    /// <summary>Reads IPTC data.</summary>
    /// <remarks>
    /// Extracted values are returned from <see cref="Extract"/> in an <see cref="IptcDirectory"/>.
    /// <para />
    /// See the IPTC specification: http://www.iptc.org/std/IIM/4.1/specification/IIMV4.1.pdf
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class IptcWriter : IJpegFragmentMetadataWriter
    {
        /// <summary>
        /// Specifies the type of metadata that this MetadataWriter can handle.
        /// </summary>
        Type IJpegFragmentMetadataWriter.MetadataType => typeof(IptcDirectory);

        private readonly IptcDirectory iptcDirectory;

        public IptcWriter(IptcDirectory iptcDirectory)
        {
            this.iptcDirectory = iptcDirectory;
        }

        public IEnumerable<JpegSegment> WriteSegments(IEnumerable<JpegSegment> segments)
        {
            // Get the IPTC data as a byte array
            byte[] iptcData = iptcDirectory.GetByteArray(IptcDirectory.TagActionAdvised);

            // If the IPTC data is null, do nothing
            if (iptcData == null)
            {
                return segments;
            }

            // Create a new IPTC segment
            JpegSegment iptcSegment = new JpegSegment(JpegSegmentType.AppD, iptcData, 0);

            // Find the last APP1 segment, and insert the IPTC segment after it
            int app1Index = -1;
            int i = 0;
            foreach (JpegSegment segment in segments)
            {
                if (segment.Type == JpegSegmentType.App1)
                {
                    app1Index = i;
                }
                i++;
            }

            if (app1Index >= 0)
            {
                //segments.Insert(app1Index + 1, iptcSegment);
            }
            else
            {
                // If there is no APP1 segment, insert the IPTC segment after the SOI marker
                int soiIndex = -1;
                i = 0;
                foreach (JpegSegment segment in segments)
                {
                    if (segment.Type == JpegSegmentType.Soi)
                    {
                        soiIndex = i;
                    }
                    i++;
                }

                if (soiIndex >= 0)
                {
                    //segments.Insert(soiIndex + 1, iptcSegment);
                }
                else
                {
                    // If there is no SOI marker, insert the IPTC segment at the beginning of the file
                    //segments.Insert(0, iptcSegment);
                }
            }

            return segments;
        }

        public static XDocument ConvertIptcDirectoryToXml(IptcDirectory iptcDirectory)
        {

            XDocument xdoc = null;
            if (iptcDirectory != null)
            {
                var serializedTags = iptcDirectory.Tags
                    .Select(tag => new { Name = tag.Name, Value = tag.Description })
                    .ToDictionary(tag => tag.Name, tag => tag.Value);

                var serializedIptc = JsonConvert.SerializeObject(serializedTags);

                // Check if the XMP string contains any invalid XML characters

                var sb = new StringBuilder(serializedIptc.Length);
                foreach (char c in serializedIptc)
                {
                    if (XmlConvert.IsXmlChar(c))
                    {
                        sb.Append(c);
                    }
                }
                serializedIptc = sb.ToString();
                serializedIptc = Regex.Replace(serializedIptc, @"^[\uFEFF]", string.Empty);

                XNamespace iptcNamespace = "http://iptc.org/std/Iptc4xmpCore/1.0/xmlns/";
                XNamespace rdfNamespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";


                xdoc = new XDocument(
                new XElement(iptcNamespace + "Iptc4xmpCore",
                    serializedTags.Select(tag => new XElement(rdfNamespace + "Description",
                        new XAttribute(rdfNamespace + "about", ""),
                        new XElement(iptcNamespace + tag.Key, tag.Value)))));

                //xdoc = XDocument.Parse(serializedIptc);

                // Set the root element namespace to the IPTC namespace
                XElement root = xdoc.Root;
                root.Name = iptcNamespace + root.Name.LocalName;

                // Set the RDF namespace on all child elements
                foreach (XElement element in root.DescendantsAndSelf())
                {
                    element.Name = rdfNamespace + element.Name.LocalName;
                }
            }

            return xdoc;
        }

        /// <summary>
        /// Updates a list of JpegFragments with new metadata.
        /// <para>
        /// An existing App1 Xmp fragment will be updated. If none is found, a new segment will be
        /// inserted before the first fragment that is not one of {Soi, App0, App1}
        /// </para>
        /// </summary>
        /// <param name="fragments">Original file fragmets</param>
        /// <param name="metadata">The Xmp metadata that shall be written</param>
        /// <returns>A new list of JpegFragments</returns>
        public List<JpegFragment> UpdateFragments([NotNull] FragmentList fragments, [NotNull] object metadata)
        {
            JpegFragment metadataFragment;
            List<JpegFragment> output = new List<JpegFragment>();
            bool wroteData = false;
            int insertPosition = 0;

            if (metadata is XDocument)
            {
                byte[] payloadBytes = EncodeXmpToPayloadBytes((XDocument)metadata);
                JpegSegment metadataSegment = new JpegSegment(JpegSegmentType.App1, payloadBytes, offset: 0);
                metadataFragment = JpegFragment.FromJpegSegment(metadataSegment);
            }
            else
            {
                throw new ArgumentException($"XmpWriter expects metadata to be of type XmpDirectory, but was given {metadata.GetType()}.");
            }

            // First look for any potential Xmp fragment, insert only if none is found

            // Walk over existing fragment and replace or insert the new metadata fragment
            for (int i = 0; i < fragments.Count; i++)
            {
                JpegFragment currentFragment = fragments[i];

                if (!wroteData && currentFragment.IsSegment)
                {
                    JpegSegmentType currentType = currentFragment.Segment.Type;

                    // if this is an existing App1 XMP fragment, overwrite it with the new fragment
                    if (currentType == JpegSegmentType.App1 && currentFragment.Segment.Bytes.Length > XmpReader.JpegSegmentPreamble.Length)
                    {
                        // This App1 segment could be a candidate for overwriting.
                        // Read the encountered segment payload to check if it contains the Xmp preamble
                        string potentialPreamble = Encoding.UTF8.GetString(currentFragment.Segment.Bytes, 0, XmpReader.JpegSegmentPreamble.Length);
                        if (potentialPreamble.Equals(XmpReader.JpegSegmentPreamble, StringComparison.OrdinalIgnoreCase))
                        {
                            // The existing Xmp App1 fragment will be replaced with the new fragment
                            currentFragment = metadataFragment;
                            wroteData = true;
                        }
                    }
                    else if (insertPosition == 0 && currentType != JpegSegmentType.Soi && currentType != JpegSegmentType.App0)
                    {
                        // file begins with Soi (App0) (App1) ...
                        // At this point we have encountered a segment that should not be earlier than an App1.
                        // But there could be another Xmp segment, so we just make a note of this position
                        insertPosition = i;
                    }
                }
                output.Add(currentFragment);
            }

            if (!wroteData)
            {
                // The files does not contain an App1-Xmp segment yet.
                // Therefore we must insert a new App1-Xmp segment at the previously determined position.
                output.Insert(insertPosition, metadataFragment);
                wroteData = true;
            }

            return output;
        }

        private byte[] EncodeXmpToPayloadBytes(XDocument metadata)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Encodes an XDocument to bytes to be used as the payload of an IPTC App13 segment.
        /// </summary>
        /// <param name="iptcDoc">IPTC document to be encoded</param>
        /// <param name="writeable">Indicates if the IPTC packet shall be marked as writable.</param>
        /// <returns>App13 segment payload</returns>
        public static byte[] EncodeIptcToPayloadBytes([NotNull] XDocument iptcDoc, bool writeable = true)
        {
            // constant parts
            byte[] preamble = new byte[] { 0x1C, 0x02 };
            byte[] recordNum = new byte[] { 0x01 };
            byte[] datasetNum = new byte[] { 0x02 };
            byte[] version = new byte[] { 0x03 };
            byte[] dataset = Encoding.UTF8.GetBytes(XmpReader.JpegSegmentPreamble);
            byte[] recordLength = BitConverter.GetBytes((short)iptcDoc.ToString().Length);
            byte[] dataSetLength = BitConverter.GetBytes((short)(dataset.Length + iptcDoc.ToString().Length + 1));

            // 1. preamble
            MemoryStream iptcMS = new MemoryStream();
            iptcMS.Write(preamble, 0, preamble.Length);

            // 2. record number
            iptcMS.Write(recordNum, 0, recordNum.Length);

            // 3. dataset number
            iptcMS.Write(datasetNum, 0, datasetNum.Length);

            // 4. version number
            iptcMS.Write(version, 0, version.Length);

            // 5. dataset name
            iptcMS.Write(dataset, 0, dataset.Length);
            iptcMS.WriteByte(0);

            // 6. record length
            iptcMS.Write(recordLength, 0, recordLength.Length);

            // 7. dataset length
            iptcMS.Write(dataSetLength, 0, dataSetLength.Length);

            // 8. serialized IPTC xml
            byte[] iptcXmlBytes = Encoding.UTF8.GetBytes(iptcDoc.ToString());
            iptcMS.Write(iptcXmlBytes, 0, iptcXmlBytes.Length);

            return iptcMS.ToArray();
        }

        /// <summary>
        /// Creates a string of whitespace with linebreaks for padding within xpacket.
        /// </summary>
        /// <param name="size">Desired total size of whitespace</param>
        /// <returns>String of whitespace with newline character in each line of 100 chars</returns>
        public static string CreateWhitespace(int size = 4096)
        {
            var line = '\u000A' + new String('\u0020', 99);
            return string.Concat(Enumerable.Repeat(line, (int)Math.Ceiling(size / 100.0))).Substring(0, size);
        }
#if false
        public DirectoryList ReadJpegSegments(IEnumerable<JpegSegment> segments)
        {
            // Ensure data starts with the IPTC marker byte
            return segments
                .Where(segment => segment.Bytes.Length != 0 && segment.Bytes[0] == IptcMarkerByte)
                .Select(segment => Extract(new SequentialByteArrayReader(segment.Bytes), segment.Bytes.Length))
#if NET35
                .Cast<Directory>()
#endif
                .ToList();
        }

        /// <summary>Reads IPTC values and returns them in an <see cref="IptcDirectory"/>.</summary>
        /// <remarks>
        /// Note that IPTC data does not describe its own length, hence <paramref name="length"/> is required.
        /// </remarks>
        [NotNull]
        public IptcDirectory Extract([NotNull] SequentialReader reader, long length)
        {
            var directory = new IptcDirectory();

            var offset = 0;

            // for each tag
            while (offset < length)
            {
                // identifies start of a tag
                byte startByte;
                try
                {
                    startByte = reader.GetByte();
                    offset++;
                }
                catch (IOException)
                {
                    directory.AddError("Unable to read starting byte of IPTC tag");
                    break;
                }

                if (startByte != IptcMarkerByte)
                {
                    // NOTE have seen images where there was one extra byte at the end, giving
                    // offset==length at this point, which is not worth logging as an error.
                    if (offset != length)
                        directory.AddError($"Invalid IPTC tag marker at offset {offset - 1}. Expected '0x{IptcMarkerByte:x2}' but got '0x{startByte:x}'.");
                    break;
                }

                // we need at least four bytes left to read a tag
                if (offset + 4 > length)
                {
                    directory.AddError("Too few bytes remain for a valid IPTC tag");
                    break;
                }

                int directoryType;
                int tagType;
                int tagByteCount;
                try
                {
                    directoryType = reader.GetByte();
                    tagType = reader.GetByte();
                    tagByteCount = reader.GetUInt16();
                    if (tagByteCount > 0x7FFF) {
                        // Extended DataSet Tag (see 1.5(c), p14, IPTC-IIMV4.2.pdf)
                        tagByteCount = ((tagByteCount & 0x7FFF) << 16) | reader.GetUInt16();
                        offset += 2;
                    }
                    offset += 4;
                }
                catch (IOException)
                {
                    directory.AddError("IPTC data segment ended mid-way through tag descriptor");
                    break;
                }

                if (offset + tagByteCount > length)
                {
                    directory.AddError("Data for tag extends beyond end of IPTC segment");
                    break;
                }

                try
                {
                    ProcessTag(reader, directory, directoryType, tagType, tagByteCount);
                }
                catch (IOException)
                {
                    directory.AddError("Error processing IPTC tag");
                    break;
                }

                offset += tagByteCount;
            }

            return directory;
        }

        private static void ProcessTag([NotNull] SequentialReader reader, [NotNull] Directory directory, int directoryType, int tagType, int tagByteCount)
        {
            var tagIdentifier = tagType | (directoryType << 8);

            // Some images have been seen that specify a zero byte tag, which cannot be of much use.
            // We elect here to completely ignore the tag. The IPTC specification doesn't mention
            // anything about the interpretation of this situation.
            // https://raw.githubusercontent.com/wiki/drewnoakes/metadata-extractor/docs/IPTC-IIMV4.2.pdf
            if (tagByteCount == 0)
            {
                directory.Set(tagIdentifier, string.Empty);
                return;
            }

            switch (tagIdentifier)
            {
                case IptcDirectory.TagCodedCharacterSet:
                {
                    var bytes = reader.GetBytes(tagByteCount);
                    var charset = Iso2022Converter.ConvertEscapeSequenceToEncodingName(bytes);
                    if (charset == null)
                    {
                        // Unable to determine the charset, so fall through and treat tag as a regular string
                        charset = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                    }
                    directory.Set(tagIdentifier, charset);
                    return;
                }

                case IptcDirectory.TagEnvelopeRecordVersion:
                case IptcDirectory.TagApplicationRecordVersion:
                case IptcDirectory.TagFileVersion:
                case IptcDirectory.TagArmVersion:
                case IptcDirectory.TagProgramVersion:
                {
                    // short
                    if (tagByteCount >= 2)
                    {
                        var shortValue = reader.GetUInt16();
                        reader.Skip(tagByteCount - 2);
                        directory.Set(tagIdentifier, shortValue);
                        return;
                    }
                    break;
                }

                case IptcDirectory.TagUrgency:
                {
                    // byte
                    directory.Set(tagIdentifier, reader.GetByte());
                    reader.Skip(tagByteCount - 1);
                    return;
                }
            }

            // If we haven't returned yet, treat it as a string
            // NOTE that there's a chance we've already loaded the value as a string above, but failed to parse the value
            var encodingName = directory.GetString(IptcDirectory.TagCodedCharacterSet);
            Encoding encoding = null;
            if (encodingName != null)
            {
                try
                {
                    encoding = Encoding.GetEncoding(encodingName);
                }
                catch (ArgumentException)
                { }
            }

            StringValue str;
            if (encoding != null)
                str = reader.GetStringValue(tagByteCount, encoding);
            else
            {
                var bytes = reader.GetBytes(tagByteCount);
                encoding = Iso2022Converter.GuessEncoding(bytes);
                str = new StringValue(bytes, encoding);
            }

            if (directory.ContainsTag(tagIdentifier))
            {
                // this fancy string[] business avoids using an ArrayList for performance reasons
                var oldStrings = directory.GetStringValueArray(tagIdentifier);

                StringValue[] newStrings;
                if (oldStrings == null)
                {
                    // TODO hitting this block means any prior value(s) are discarded
                    newStrings = new StringValue[1];
                }
                else
                {
                    newStrings = new StringValue[oldStrings.Length + 1];
                    Array.Copy(oldStrings, 0, newStrings, 0, oldStrings.Length);
                }
                newStrings[newStrings.Length - 1] = str;
                directory.Set(tagIdentifier, newStrings);
            }
            else
            {
                directory.Set(tagIdentifier, str);
            }
        }
#endif
    }
}
