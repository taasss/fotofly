﻿namespace Fotofly.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    using Fotofly;
    using Fotofly.BitmapMetadataTools;
    using Fotofly.Geotagging;
    using Fotofly.MetadataQueries;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class JpgPhotoUnitTests
    {
        private string samplePhotosFolder = @"..\..\..\~Sample Files\JpgPhotos\";

        // Basic file properties
        private JpgPhoto jpgPhotoOne;

        // Contains GPS Data
        private JpgPhoto jpgPhotoTwo;

        public JpgPhotoUnitTests()
        {
            this.jpgPhotoOne = new JpgPhoto(this.samplePhotosFolder + TestPhotos.UnitTest1);
            this.jpgPhotoTwo = new JpgPhoto(this.samplePhotosFolder + TestPhotos.UnitTest2);
        }

        #region Pre & Post Test Pass Code, not currently used
        // Run code after all tests in a class have run
        [ClassCleanup()]
        public static void PostTestPassCleanup()
        {
        }

        // Run code before running the first test in the class
        [ClassInitialize()]
        public static void PreTestPassInitialize(TestContext testContext)
        {
        }
        #endregion

        /// <summary>
        /// Check test photo can be read and metadata loaded into memory
        /// </summary>
        [TestMethod]
        public void ReadMetadataFromFile()
        {
            try
            {
                this.jpgPhotoOne.ReadMetadata();
            }
            catch
            {
                Assert.Fail("Metadata could not be read (" + this.jpgPhotoOne.FileFullName + ")");
            }

            try
            {
                this.jpgPhotoTwo.ReadMetadata();
            }
            catch
            {
                Assert.Fail("Metadata could not be read (" + this.jpgPhotoTwo.FileFullName + ")");
            }
        }

        /// <summary>
        /// ReadStringMetadata
        /// </summary>
        [TestMethod]
        public void ReadStringMetadata()
        {
            // All string values
            StringAssert.Matches(this.jpgPhotoOne.Metadata.Authors[0], new Regex("Test Author"), "Author");
            StringAssert.Matches(this.jpgPhotoOne.Metadata.CameraManufacturer, new Regex("Canon"), "CameraManufacturer");
            StringAssert.Matches(this.jpgPhotoOne.Metadata.CameraModel, new Regex("Canon"), "CameraModel");
            StringAssert.Matches(this.jpgPhotoOne.Metadata.Comment, new Regex("Test Comment"), "Comment");
            StringAssert.Matches(this.jpgPhotoOne.Metadata.CreationSoftware, new Regex(FotoflyAssemblyInfo.ShortBuildVersion), "CreationSoftware");
            StringAssert.Matches(this.jpgPhotoOne.Metadata.Copyright, new Regex("Test Copyright"), "Copyright");
            StringAssert.Matches(this.jpgPhotoOne.Metadata.FocalLength, new Regex("380 mm"));
            StringAssert.Matches(this.jpgPhotoOne.Metadata.Tags.Last().Name, new Regex(@"Test Tag Î"));

            StringAssert.Matches(this.jpgPhotoOne.Metadata.Description, new Regex(@"Test Title"));
            StringAssert.Matches(this.jpgPhotoOne.Metadata.Comment, new Regex(@"Test Comment"));
            StringAssert.Matches(this.jpgPhotoOne.Metadata.Subject, new Regex(@"Test Subject"));
        }

        /// <summary>
        /// ReadNumericMetadata
        /// </summary>
        [TestMethod]
        public void ReadNumericMetadata()
        {
            Assert.AreEqual<int>(this.jpgPhotoOne.Metadata.ImageHeight, 480, "ImageHeight");
            Assert.AreEqual<int>(this.jpgPhotoOne.Metadata.ImageWidth, 640, "ImageWidth");

            Assert.AreEqual<int>(this.jpgPhotoOne.Metadata.HorizontalResolution, 350, "HorizontalResolution");
            Assert.AreEqual<int>(this.jpgPhotoOne.Metadata.VerticalResolution, 350, "VerticalResolution");

            // TODO Get test file
            Assert.AreEqual<double?>(this.jpgPhotoOne.Metadata.DigitalZoomRatio, null, "DigitalZoomRatio");
        }

        /// <summary>
        /// Read Ratings
        /// </summary>
        [TestMethod]
        public void ReadRatingMetadata()
        {
            Assert.AreEqual<Rating.Ratings>(this.jpgPhotoOne.Metadata.Rating.AsEnum, Rating.Ratings.ThreeStar, "Rating");

            Assert.AreEqual<Rating.Ratings>(this.jpgPhotoTwo.Metadata.Rating.AsEnum, Rating.Ratings.NoRating, "Rating");
        }

        /// <summary>
        /// ReadEnumMetadata
        /// </summary>
        [TestMethod]
        public void ReadEnumMetadata()
        {
            Assert.AreEqual<MetadataEnums.MeteringModes>(this.jpgPhotoTwo.Metadata.MeteringMode, MetadataEnums.MeteringModes.CenterWeightedAverage);
        }

        /// <summary>
        /// Check Dates
        /// </summary>
        [TestMethod]
        public void ReadDateMetadata()
        {
            StringAssert.Matches(this.jpgPhotoOne.Metadata.DateDigitised.ToString(), new Regex("10/10/2009 21:46:37"));
            StringAssert.Matches(this.jpgPhotoOne.Metadata.DateTaken.ToString(), new Regex("10/10/2009 14:46:37"));
            StringAssert.Matches(this.jpgPhotoOne.Metadata.DateAquired.ToString(), new Regex("15/11/2009 00:05:58"));
        }

        /// <summary>
        /// Read and Write Gps metadata
        /// </summary>
        [TestMethod]
        public void ReadGpsMetadata()
        {
            StringAssert.Matches(this.jpgPhotoOne.Metadata.GpsPositionOfLocationCreated.ToString(), new Regex("N 037° 48' 25.00\" W 122° 25' 23.00\" -17.464m"), "GpsPosition of Location");
            StringAssert.Matches(this.jpgPhotoOne.Metadata.GpsPositionOfLocationCreated.Source, new Regex("Garmin Dakota 20"), "GPS Device");

            Assert.AreEqual<double>(this.jpgPhotoOne.Metadata.GpsPositionOfLocationCreated.Altitude, -17.464, "Altitude");
            Assert.AreEqual<GpsPosition.Dimensions>(this.jpgPhotoOne.Metadata.GpsPositionOfLocationCreated.Dimension, GpsPosition.Dimensions.ThreeDimensional, "Dimensions");
            Assert.AreEqual<DateTime>(this.jpgPhotoOne.Metadata.GpsPositionOfLocationCreated.SatelliteTime, new DateTime(2009, 10, 10, 21, 46, 24), "Satalite Time");
        }

        /// <summary>
        /// Check Microsoft RegionInfo regions data are read correctly
        /// </summary>
        [TestMethod]
        public void ReadImageRegionMetadata()
        {
            if (this.jpgPhotoOne.Metadata.MicrosoftRegionInfo == null)
            {
                Assert.Fail("Metadata contains no Regions");
            }
            else if (this.jpgPhotoOne.Metadata.MicrosoftRegionInfo.Regions.Count != 1)
            {
                Assert.Fail("Metadata doesn't contain 1 region");
            }

            StringAssert.Matches(this.jpgPhotoOne.Metadata.MicrosoftRegionInfo.Regions[0].PersonDisplayName, new Regex("Ben Vincent"));
            StringAssert.Matches(this.jpgPhotoOne.Metadata.MicrosoftRegionInfo.Regions[0].PersonEmailDigest, new Regex("68A7D36853D6CBDEC05624C1516B2533F8F665E0"));
            StringAssert.Matches(this.jpgPhotoOne.Metadata.MicrosoftRegionInfo.Regions[0].PersonLiveIdCID, new Regex("3058747437326753075"));
            StringAssert.Matches(this.jpgPhotoOne.Metadata.MicrosoftRegionInfo.Regions[0].RectangleString, new Regex("0.365625, 0.145833, 0.126562, 0.16875"));
        }

        /// <summary>
        /// Read Aperture, Iso Speed and Shutter Speed
        /// </summary>
        [TestMethod]
        public void ReadComplexProperties()
        {
            Assert.AreEqual<ExposureBias>(this.jpgPhotoOne.Metadata.ExposureBias, new ExposureBias());
            Assert.AreEqual<ShutterSpeed>(this.jpgPhotoOne.Metadata.ShutterSpeed, new ShutterSpeed("1/1000"), "ShutterSpeed");
            Assert.AreEqual<Aperture>(this.jpgPhotoOne.Metadata.Aperture, new Aperture(9), "Aperture");
            Assert.AreEqual<IsoSpeed>(this.jpgPhotoOne.Metadata.IsoSpeed, new IsoSpeed(400), "IsoSpeed");
        }

        /// <summary>
        /// ReadExposureBias
        /// </summary>
        [TestMethod]
        public void ReadExposureBias()
        {
            // -1.3 step
            JpgPhoto jpgPhoto = new JpgPhoto(this.samplePhotosFolder + TestPhotos.ExposureBiasMinus13);
            Assert.AreEqual<ExposureBias>(jpgPhoto.Metadata.ExposureBias, new ExposureBias("-4/3"));

            // +1.3 step
            jpgPhoto = new JpgPhoto(this.samplePhotosFolder + TestPhotos.ExposureBiasPlus13);
            Assert.AreEqual<ExposureBias>(jpgPhoto.Metadata.ExposureBias, new ExposureBias("4/3"));
        }

        /// <summary>
        /// Read Files from different cameras
        /// </summary>
        [TestMethod]
        public void ReadCameraMake()
        {
            // Read photos from a couple of different cameras, the aim is to ensure no exception is thrown reading the data
            JpgPhoto photo = new JpgPhoto(this.samplePhotosFolder + TestPhotos.MakeKodakDX4900);
            photo.ReadMetadata();

            photo = new JpgPhoto(this.samplePhotosFolder + TestPhotos.MakeNikonCoolPixP80);
            photo.ReadMetadata();

            photo = new JpgPhoto(this.samplePhotosFolder + TestPhotos.MakeNikonD70);
            photo.ReadMetadata();

            photo = new JpgPhoto(this.samplePhotosFolder + TestPhotos.MakePentaxOptioS);
            photo.ReadMetadata();

            photo = new JpgPhoto(this.samplePhotosFolder + TestPhotos.MakeSonyDSCT30);
            photo.ReadMetadata();

            photo = new JpgPhoto(this.samplePhotosFolder + TestPhotos.MakeiPhone3GsUntouched);
            photo.ReadMetadata();

            Assert.AreEqual<string>(photo.Metadata.CameraManufacturer, "Apple", "CameraManufacturer");
            Assert.AreEqual<string>(photo.Metadata.CameraModel, "iPhone 3GS", "CameraModel");
            Assert.AreEqual<DateTime>(photo.Metadata.DateDigitised, new DateTime(2010, 02, 01, 08, 24, 40), "DateDigitised");
            Assert.AreEqual<MetadataEnums.MeteringModes>(photo.Metadata.MeteringMode, MetadataEnums.MeteringModes.Average, "MeteringMode");
            Assert.AreEqual<Aperture>(photo.Metadata.Aperture, new Aperture(14, 5), "Aperture");
            Assert.AreEqual<ShutterSpeed>(photo.Metadata.ShutterSpeed, new ShutterSpeed(1, 170), "ShutterSpeed");

            photo = new JpgPhoto(this.samplePhotosFolder + TestPhotos.MakeiPhone3GsWithTags);
            photo.ReadMetadata();

            Assert.AreEqual<Tag>(photo.Metadata.Tags.First(), new Tag("Test"), "Tag");
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void BulkRead()
        {
            foreach (string file in Directory.GetFiles(this.samplePhotosFolder, "*,jpg"))
            {
                JpgPhoto jpgPhoto = new JpgPhoto(file);

                try
                {
                    jpgPhoto.ReadMetadata();
                }
                catch
                {
                    Assert.Fail("Unable to read Metadata: " + file);
                }
            }
        }

        /// <summary>
        /// Read and Write Gps metadata
        /// </summary>
        [TestMethod]
        public void WriteGpsMetadata()
        {
            // Copy Test file
            string testPhoto = this.samplePhotosFolder + TestPhotos.UnitTestTemp9;
            File.Copy(this.jpgPhotoOne.FileFullName, testPhoto, true);

            // Test data, includes Unicode strings
            GpsPosition positionCreated = new GpsPosition(101.23, -34.321, -99.8);
            GpsPosition positionShow = new GpsPosition(-123.0, 179);

            // Scrub existing data
            JpgPhoto jpgPhoto = new JpgPhoto(testPhoto);
            jpgPhoto.Metadata.GpsPositionOfLocationCreated = new GpsPosition();
            jpgPhoto.Metadata.GpsPositionOfLocationShown = new GpsPosition();
            jpgPhoto.WriteMetadata();

            // Check for empty data
            jpgPhoto = new JpgPhoto(testPhoto);
            Assert.AreEqual<GpsPosition>(jpgPhoto.Metadata.GpsPositionOfLocationCreated, new GpsPosition(), "Blank GpsPosition Created");
            Assert.AreEqual<GpsPosition>(jpgPhoto.Metadata.GpsPositionOfLocationShown, new GpsPosition(), "Blank GpsPosition Shown");

            // Write GpsPosition Created
            jpgPhoto = new JpgPhoto(testPhoto);
            jpgPhoto.Metadata.GpsPositionOfLocationCreated = positionCreated;
            jpgPhoto.Metadata.GpsPositionOfLocationShown = new GpsPosition();
            jpgPhoto.WriteMetadata();

            // And Check Created
            jpgPhoto = new JpgPhoto(testPhoto);
            Assert.AreEqual<GpsPosition>(jpgPhoto.Metadata.GpsPositionOfLocationCreated, positionCreated, "WriteCreated Created");
            Assert.AreEqual<GpsPosition>(jpgPhoto.Metadata.GpsPositionOfLocationShown, new GpsPosition(), "WriteCreated Shown");

            // Write GpsPosition Shown
            jpgPhoto = new JpgPhoto(testPhoto);
            jpgPhoto.Metadata.GpsPositionOfLocationCreated = new GpsPosition();
            jpgPhoto.Metadata.GpsPositionOfLocationShown = positionShow;
            jpgPhoto.WriteMetadata();

            // And Check Shown
            jpgPhoto = new JpgPhoto(testPhoto);
            Assert.AreEqual<GpsPosition>(jpgPhoto.Metadata.GpsPositionOfLocationCreated, new GpsPosition(), "WriteShown Created");
            Assert.AreEqual<GpsPosition>(jpgPhoto.Metadata.GpsPositionOfLocationShown, positionShow, "WriteShown Shown");

            // Tidy up
            File.Delete(testPhoto);
        }

        /// <summary>
        /// Read and Write Address metadata
        /// </summary>
        [TestMethod]
        public void WriteAddressMetadata()
        {
            // Copy Test file
            string testPhoto = this.samplePhotosFolder + TestPhotos.UnitTestTemp10;
            File.Copy(this.jpgPhotoOne.FileFullName, testPhoto, true);

            // Test data, includes Unicode strings
            Address addressCreated = new Address(@"CòuntryCreàted/RegÎon/Ĉity/Stréét");
            Address addressShown = new Address(@"CòuntryShówn/RegÎon/Ĉity/Stréét");
            
            // Scrub existing data
            JpgPhoto jpgPhoto = new JpgPhoto(testPhoto);
            jpgPhoto.Metadata.AddressOfLocationCreated = new Address();
            jpgPhoto.Metadata.AddressOfLocationShown = new Address();
            jpgPhoto.WriteMetadata();

            // Check for empty data
            jpgPhoto = new JpgPhoto(testPhoto);
            Assert.AreEqual<Address>(jpgPhoto.Metadata.AddressOfLocationCreated, new Address(), "Blank Address Created");
            Assert.AreEqual<Address>(jpgPhoto.Metadata.AddressOfLocationShown, new Address(), "Blank Address Shown");

            // Write Address Created
            jpgPhoto = new JpgPhoto(testPhoto);
            jpgPhoto.Metadata.AddressOfLocationCreated = addressCreated;
            jpgPhoto.Metadata.AddressOfLocationShown = new Address();
            jpgPhoto.WriteMetadata();

            // And Check Created
            jpgPhoto = new JpgPhoto(testPhoto);
            Assert.AreEqual<Address>(jpgPhoto.Metadata.AddressOfLocationCreated, addressCreated, "WriteAddress Created");
            Assert.AreEqual<Address>(jpgPhoto.Metadata.AddressOfLocationShown, new Address(), "WriteAddress Shown");

            // Write Address Shown
            jpgPhoto = new JpgPhoto(testPhoto);
            jpgPhoto.Metadata.AddressOfLocationCreated = new Address();
            jpgPhoto.Metadata.AddressOfLocationShown = addressShown;
            jpgPhoto.WriteMetadata();

            // And Check Shown
            jpgPhoto = new JpgPhoto(testPhoto);
            Assert.AreEqual<Address>(jpgPhoto.Metadata.AddressOfLocationCreated, new Address(), "WriteShown Created");
            Assert.AreEqual<Address>(jpgPhoto.Metadata.AddressOfLocationShown, addressShown, "WriteShown Shown");

            // Tidy up
            File.Delete(testPhoto);
        }

        /// <summary>
        /// Check Microsoft RegionInfo regions are written correctly
        /// </summary>
        [TestMethod]
        public void WriteImageRegionMetadata()
        {
            string testValueSuffix = DateTime.Now.ToUniversalTime().ToString();

            this.jpgPhotoOne.Metadata.MicrosoftRegionInfo.Regions[0].PersonDisplayName = "PersonDisplayName" + testValueSuffix;
            this.jpgPhotoOne.Metadata.MicrosoftRegionInfo.Regions[0].PersonEmailDigest = "PersonEmailDigest" + testValueSuffix;
            this.jpgPhotoOne.Metadata.MicrosoftRegionInfo.Regions[0].PersonLiveIdCID = "PersonLiveIdCID" + testValueSuffix;
            this.jpgPhotoOne.Metadata.MicrosoftRegionInfo.Regions[0].RectangleString = "0.1, 0.2, 0.3, 0.4";

            this.jpgPhotoOne.WriteMetadata(this.samplePhotosFolder + TestPhotos.UnitTestTemp4);

            JpgPhoto jpgPhotoTemp = new JpgPhoto(this.samplePhotosFolder + TestPhotos.UnitTestTemp4);

            StringAssert.Matches(jpgPhotoTemp.Metadata.MicrosoftRegionInfo.Regions[0].PersonDisplayName, new Regex("PersonDisplayName" + testValueSuffix));
            StringAssert.Matches(jpgPhotoTemp.Metadata.MicrosoftRegionInfo.Regions[0].PersonEmailDigest, new Regex("PersonEmailDigest" + testValueSuffix));
            StringAssert.Matches(jpgPhotoTemp.Metadata.MicrosoftRegionInfo.Regions[0].PersonLiveIdCID, new Regex("PersonLiveIdCID" + testValueSuffix));
            StringAssert.Matches(jpgPhotoTemp.Metadata.MicrosoftRegionInfo.Regions[0].RectangleString, new Regex("0.1, 0.2, 0.3, 0.4"));

            File.Delete(this.samplePhotosFolder + TestPhotos.UnitTestTemp4);
        }

        /// <summary>
        /// WriteMetadataToFile
        /// </summary>
        [TestMethod]
        public void WriteMetadataToFile()
        {
            // Clean up from previous test
            if (File.Exists(this.samplePhotosFolder + TestPhotos.UnitTestTemp3))
            {
                File.Delete(this.samplePhotosFolder + TestPhotos.UnitTestTemp3);
            }

            // Test value to write
            string testString = "Test " + DateTime.Now.ToString();
            DateTime testDate = DateTime.Now.AddTicks(-DateTime.Now.TimeOfDay.Ticks);
            string testTag = "Test Tag Î";

            // Write text
            this.jpgPhotoTwo.Metadata.Description = testString;
            this.jpgPhotoTwo.Metadata.Comment = testString;
            this.jpgPhotoTwo.Metadata.Copyright = testString;
            this.jpgPhotoTwo.Metadata.AddressOfGpsSource = testString;
            this.jpgPhotoTwo.Metadata.Tags = new TagList();
            this.jpgPhotoTwo.Metadata.Tags.Add(new Tag(testTag));

            // Write dates
            this.jpgPhotoTwo.Metadata.DateAquired = testDate;
            this.jpgPhotoTwo.Metadata.DateDigitised = testDate;
            this.jpgPhotoTwo.Metadata.DateTaken = testDate;
            this.jpgPhotoTwo.Metadata.DateUtc = testDate;
            this.jpgPhotoTwo.Metadata.AddressOfGpsLookupDate = testDate;

            // Save Photo Three
            this.jpgPhotoTwo.WriteMetadata(this.samplePhotosFolder + TestPhotos.UnitTestTemp3);

            // Check the file was created
            if (!File.Exists(this.samplePhotosFolder + TestPhotos.UnitTestTemp3))
            {
                Assert.Fail("File save failed");
            }

            // Read metadata
            JpgPhoto jpgPhotoTemp = new JpgPhoto(this.samplePhotosFolder + TestPhotos.UnitTestTemp3);
            jpgPhotoTemp.ReadMetadata();

            // Check the file was created
            if (jpgPhotoTemp.Metadata == null)
            {
                Assert.Fail("Unable to read saved files metadata");
            }

            // Check the text was set correctly
            StringAssert.Matches(jpgPhotoTemp.Metadata.Description, new Regex(testString), "Description");
            StringAssert.Matches(jpgPhotoTemp.Metadata.Comment, new Regex(testString), "Comment");
            StringAssert.Matches(jpgPhotoTemp.Metadata.Copyright, new Regex(testString), "Copyright");
            StringAssert.Matches(jpgPhotoTemp.Metadata.AddressOfGpsSource, new Regex(testString), "AddressOfGpsSource");

            // Check date was written
            Assert.AreEqual<DateTime>(jpgPhotoTemp.Metadata.DateAquired, testDate, "DateAquired");
            Assert.AreEqual<DateTime>(jpgPhotoTemp.Metadata.DateDigitised, testDate, "DateDigitised");
            Assert.AreEqual<DateTime>(jpgPhotoTemp.Metadata.DateTaken, testDate, "DateTaken");
            Assert.AreEqual<DateTime>(jpgPhotoTemp.Metadata.DateUtc, testDate, "UtcDate");
            Assert.AreEqual<DateTime>(jpgPhotoTemp.Metadata.AddressOfGpsLookupDate, testDate, "AddressOfGpsLookupDate");
            Assert.AreEqual<Tag>(jpgPhotoTemp.Metadata.Tags.Last(), new Tag(testTag), "Tags");

            if (new FileInfo(this.jpgPhotoTwo.FileFullName).Length > new FileInfo(jpgPhotoTemp.FileFullName).Length)
            {
                Assert.Fail("Photo has decreased in size after saving");
            }

            File.Delete(this.samplePhotosFolder + TestPhotos.UnitTestTemp3);
        }

        /// <summary>
        /// WriteMetadataToFile
        /// </summary>
        [TestMethod]
        public void WriteMetadataAndCheckForMetadataLoss()
        {
            // Clean up from previous test
            if (File.Exists(this.samplePhotosFolder + TestPhotos.UnitTestTemp5))
            {
                File.Delete(this.samplePhotosFolder + TestPhotos.UnitTestTemp5);
            }

            // Change date and save
            this.jpgPhotoOne.Metadata.DateLastFotoflySave = DateTime.Now.AddTicks(-DateTime.Now.TimeOfDay.Ticks);
            this.jpgPhotoOne.WriteMetadata(this.samplePhotosFolder + TestPhotos.UnitTestTemp5);

            MetadataDump beforeDump;
            MetadataDump afterDump;

            using (WpfFileManager wpfFileManager = new WpfFileManager(this.samplePhotosFolder + TestPhotos.UnitTest1))
            {
                beforeDump = new MetadataDump(wpfFileManager.BitmapMetadata);
                beforeDump.GenerateStringList();
            }

            using (WpfFileManager wpfFileManager = new WpfFileManager(this.samplePhotosFolder + TestPhotos.UnitTestTemp5))
            {
                afterDump = new MetadataDump(wpfFileManager.BitmapMetadata);
                afterDump.GenerateStringList();
            }

            for (int i = 0; i < beforeDump.StringList.Count; i++)
            {
                // Ignore schema changes, edit dates and created software
                if (beforeDump.StringList[i] != afterDump.StringList[i]
                    && !beforeDump.StringList[i].Contains("LastEditDate")
                    && !beforeDump.StringList[i].Contains("ushort=513")
                    && !beforeDump.StringList[i].Contains("OffsetSchema"))
                {
                    Assert.Fail("Metadata mismatch " + beforeDump.StringList[i] + " != " + afterDump.StringList[i]);
                }
            }

            if (new FileInfo(this.jpgPhotoTwo.FileFullName).Length > new FileInfo(this.samplePhotosFolder + TestPhotos.UnitTestTemp5).Length)
            {
                Assert.Fail("Photo has decreased in size after saving");
            }

            // Clean up
            File.Delete(this.samplePhotosFolder + TestPhotos.UnitTestTemp5);
        }

        /// <summary>
        /// WriteMetadataToFile Lossless Check
        /// </summary>
        [TestMethod]
        public void WriteMetadataAndCheckForImageLoss()
        {
            string beforeFile = this.samplePhotosFolder + TestPhotos.UnitTestTemp6;
            string afterFile = this.samplePhotosFolder + TestPhotos.UnitTestTemp7;

            // Get a copy of a file
            File.Copy(this.jpgPhotoOne.FileFullName, beforeFile, true);

            // Change some metadata
            JpgPhoto beforePhoto = new JpgPhoto(beforeFile);
            beforePhoto.Metadata.Description = "Test Description" + DateTime.Now.ToString();
            beforePhoto.Metadata.Comment = "Test Comment" + DateTime.Now.ToString();
            beforePhoto.Metadata.Copyright = "Test Copyright" + DateTime.Now.ToString();
            beforePhoto.Metadata.Subject = "Subject Copyright" + DateTime.Now.ToString();

            // Save as temp file
            beforePhoto.WriteMetadata(afterFile);

            // Open Original File
            using (Stream beforeStream = File.Open(beforeFile, FileMode.Open, FileAccess.Read))
            {
                // Open the Saved File
                using (Stream afterStream = File.Open(afterFile, FileMode.Open, FileAccess.Read))
                {
                    // Compare every pixel to ensure it has changed
                    BitmapSource beforeBitmap = BitmapDecoder.Create(beforeStream, BitmapCreateOptions.None, BitmapCacheOption.OnDemand).Frames[0];
                    BitmapSource afterBitmap = BitmapDecoder.Create(afterStream, BitmapCreateOptions.None, BitmapCacheOption.OnDemand).Frames[0];

                    PixelFormat pf = PixelFormats.Bgra32;

                    FormatConvertedBitmap fcbOne = new FormatConvertedBitmap(beforeBitmap, pf, null, 0);
                    FormatConvertedBitmap fcbTwo = new FormatConvertedBitmap(afterBitmap, pf, null, 0);

                    GC.AddMemoryPressure(((fcbOne.Format.BitsPerPixel * (fcbOne.PixelWidth + 7)) / 8) * fcbOne.PixelHeight);
                    GC.AddMemoryPressure(((fcbTwo.Format.BitsPerPixel * (fcbTwo.PixelWidth + 7)) / 8) * fcbTwo.PixelHeight);

                    int width = fcbOne.PixelWidth;
                    int height = fcbOne.PixelHeight;

                    int bpp = pf.BitsPerPixel;
                    int stride = (bpp * (width + 7)) / 8;

                    byte[] scanline0 = new byte[stride];
                    byte[] scanline1 = new byte[stride];

                    Int32Rect lineRect = new Int32Rect(0, 0, width, 1);

                    // Loop through each row
                    for (int y = 0; y < height; y++)
                    {
                        lineRect.Y = y;

                        fcbOne.CopyPixels(lineRect, scanline0, stride, 0);
                        fcbTwo.CopyPixels(lineRect, scanline1, stride, 0);

                        // Loop through each column
                        for (int b = 0; b < stride; b++)
                        {
                            if (Math.Abs(scanline0[b] - scanline1[b]) > 0)
                            {
                                Assert.Fail("Saved file was not solved losslessly");
                            }
                        }
                    }
                }
            }

            // Tidy UP
            File.Delete(beforeFile);
            File.Delete(afterFile);
        }

        /// <summary>
        /// WriteMetadataToXmlFile
        /// </summary>
        [TestMethod]
        public void WriteAndReadMetadataToXmlFile()
        {
            string imgFile = this.samplePhotosFolder + TestPhotos.UnitTestTemp8;
            string xmlFile = imgFile.Replace(".jpg", ".xml");

            // Clean up from previous test
            if (File.Exists(xmlFile))
            {
                File.Delete(xmlFile);
            }

            File.Copy(this.samplePhotosFolder + TestPhotos.UnitTest1, imgFile, true);

            JpgPhoto jpgPhoto = new JpgPhoto(imgFile);
            jpgPhoto.ReadMetadata();
            jpgPhoto.WriteMetadataToXml(xmlFile);

            if (!File.Exists(xmlFile))
            {
                Assert.Fail("Serialised File was not found: " + xmlFile);
            }

            JpgPhoto xmlPhoto = new JpgPhoto(imgFile);
            xmlPhoto.ReadMetadataFromXml(xmlFile);

            List<CompareResult> changes = new List<CompareResult>();

            PhotoMetadataTools.CompareMetadata(jpgPhoto.Metadata, xmlPhoto.Metadata, ref changes);

            if (changes.Count > 0)
            {
                Assert.Fail("Serialised File was incompleted: " + changes[0]);
            }

            File.Delete(imgFile);
            File.Delete(xmlFile);
        }

        [TestCleanup()]
        public void PostTestCleanup()
        {
        }

        #region Pre\Post Test Code
        // Run code before running each test 
        [TestInitialize()]
        public void PreTestInitialize()
        {
        }
        #endregion
    }
}
