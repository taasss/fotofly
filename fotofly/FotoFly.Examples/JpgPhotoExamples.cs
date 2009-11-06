﻿namespace FotoFly.Examples
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows.Media.Imaging;

    public static class JpgPhotoExamples
    {
        public static void ReadMetadata(string inputFile)
        {
            JpgPhoto jpgPhoto = new JpgPhoto(inputFile);
            jpgPhoto.ReadMetadata();

            Debug.WriteLine(jpgPhoto.Metadata.CameraModel);
        }

        public static void WriteMetadata(string inputFile)
        {
            File.Copy(inputFile, "JpgPhotoExamples.WriteMetadata.jpg", true);

            JpgPhoto jpgPhoto = new JpgPhoto("JpgPhotoExamples.WriteMetadata.jpg");
            jpgPhoto.ReadMetadata();

            jpgPhoto.Metadata.Comment = "Test Comment";

            jpgPhoto.SaveMetadata();
        }

        public static void AddTag(string inputFile)
        {
            File.Copy(inputFile, "JpgPhotoExamples.AddTag.jpg", true);

            JpgPhoto jpgPhoto = new JpgPhoto("JpgPhotoExamples.AddTag.jpg");
            jpgPhoto.ReadMetadata();

            jpgPhoto.Metadata.Tags.Add("Test Tag: " + DateTime.Now.ToString());

            jpgPhoto.SaveMetadata();
        }

        public static void AddRegion(string inputFile)
        {
            File.Copy(inputFile, "JpgPhotoExamples.AddRegion.jpg", true);

            // Open file and read metadata
            JpgPhoto jpgPhoto = new JpgPhoto("JpgPhotoExamples.AddRegion.jpg");
            jpgPhoto.ReadMetadata();

            // Create new Region
            XmpRegion newRegion = new XmpRegion();
            newRegion.PersonDisplayName = "Ben Vincent";
            newRegion.RectangleString = "0.1, 0.1, 0.1, 0.1";

            // Add the new region to the photo
            jpgPhoto.Metadata.RegionInfo.Regions.Add(newRegion);

            // Save
            jpgPhoto.SaveMetadata();
        }

        public static void AddGpsCoor(string inputFile)
        {
            File.Copy(inputFile, "JpgPhotoExamples.AddGpsCoor.jpg", true);

            JpgPhoto jpgPhoto = new JpgPhoto("JpgPhotoExamples.AddGpsCoor.jpg");
            jpgPhoto.ReadMetadata();

            jpgPhoto.Metadata.GpsPosition.Latitude.Numeric = 1.05;
            jpgPhoto.Metadata.GpsPosition.Longitude.Numeric = -0.95;
            jpgPhoto.Metadata.GpsPosition.Dimension = GpsPosition.Dimensions.TwoDimensional;
            jpgPhoto.Metadata.GpsPosition.Source = "GPS Test";

            jpgPhoto.SaveMetadata();
        }
    }
}