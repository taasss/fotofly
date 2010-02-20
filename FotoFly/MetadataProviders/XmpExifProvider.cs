﻿// <copyright file="XmpExifProvider.cs" company="Taasss">Copyright (c) 2009 All Right Reserved</copyright>
// <author>Ben Vincent</author>
// <date>2010-02-15</date>
// <summary>XmpExifProvider Class</summary>
namespace Fotofly.MetadataProviders
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.Text;
    using System.Windows.Media.Imaging;

    using Fotofly.BitmapMetadataTools;
    using Fotofly.MetadataQueries;

    public class XmpExifProvider : BaseProvider
    {
        public XmpExifProvider(BitmapMetadata bitmapMetadata)
            : base(bitmapMetadata)
        { }

        /// <summary>
        /// GpsPosition, encapculates Latitude, Longitude, Altitude, Source, Dimension and SatelliteTime
        /// </summary>
        public GpsPosition GpsPosition
        {
            get
            {
                GpsPosition gpsPosition = new GpsPosition();
                gpsPosition.Latitude = this.GpsLatitude.Clone() as GpsCoordinate;
                gpsPosition.Longitude = this.GpsLongitude.Clone() as GpsCoordinate;
                gpsPosition.Altitude = this.GpsAltitude;
                gpsPosition.Source = this.GpsProcessingMethod;
                gpsPosition.Dimension = this.GpsMeasureMode;
                gpsPosition.SatelliteTime = this.GpsDateTimeStamp;

                return gpsPosition;
            }

            set
            {
                if (value.IsValidCoordinate)
                {
                    this.GpsAltitude = value.Altitude;
                    this.GpsLatitude = value.Latitude.Clone() as GpsCoordinate;
                    this.GpsLongitude = value.Longitude.Clone() as GpsCoordinate;
                    this.GpsDateTimeStamp = value.SatelliteTime;
                    this.GpsMeasureMode = value.Dimension;
                    this.GpsProcessingMethod = value.Source;
                    this.GpsVersionID = "2200";
                }
                else
                {
                    // Clear all properties
                    this.GpsAltitude = double.NaN;
                    this.GpsLatitude = new GpsCoordinate();
                    this.GpsLongitude = new GpsCoordinate();
                    this.GpsDateTimeStamp = new DateTime();
                    this.GpsProcessingMethod = string.Empty;
                    this.GpsVersionID = string.Empty;
                    this.GpsMeasureMode = GpsPosition.Dimensions.NotSpecified;
                }
            }
        }

        /// <summary>
        /// Gps Altitude
        /// </summary>
        public double GpsAltitude
        {
            get
            {
                URational urational = this.BitmapMetadata.GetQuery<URational>(XmpExifQueries.GpsAltitude.Query);

                if (this.GpsAltitudeRef == GpsPosition.AltitudeRef.NotSpecified || urational == null || double.IsNaN(urational.ToDouble()))
                {
                    return double.NaN;
                }
                else if (this.GpsAltitudeRef == GpsPosition.AltitudeRef.BelowSeaLevel)
                {
                    return -urational.ToDouble(3);
                }
                else
                {
                    return urational.ToDouble(3);
                }
            }

            set
            {
                if (this.ValueHasChanged(value, this.GpsAltitude))
                {
                    // Check to see if the values have changed
                    if (double.IsNaN(value))
                    {
                        // Blank out existing values
                        this.GpsAltitudeRef = GpsPosition.AltitudeRef.NotSpecified;

                        this.BitmapMetadata.SetQuery(XmpExifQueries.GpsAltitude.Query, string.Empty);
                    }
                    else
                    {
                        this.GpsAltitudeRef = value > 0 ? GpsPosition.AltitudeRef.AboveSeaLevel : GpsPosition.AltitudeRef.BelowSeaLevel;

                        this.BitmapMetadata.SetQuery(XmpExifQueries.GpsAltitude.Query, Math.Abs(value));
                    }
                }
            }
        }

        public GpsPosition.AltitudeRef GpsAltitudeRef
        {
            get
            {
                if (this.BitmapMetadata.ContainsQuery(XmpExifQueries.GpsAltitudeRef.Query))
                {
                    string gpsAltitudeRef = this.BitmapMetadata.GetQuery<string>(XmpExifQueries.GpsAltitudeRef.Query);

                    if (gpsAltitudeRef == "0")
                    {
                        return GpsPosition.AltitudeRef.AboveSeaLevel;
                    }
                    else if (gpsAltitudeRef == "1")
                    {
                        return GpsPosition.AltitudeRef.BelowSeaLevel;
                    }
                }

                return GpsPosition.AltitudeRef.NotSpecified;
            }

            set
            {
                if (this.ValueHasChanged(value, this.GpsAltitudeRef))
                {
                    if (value == GpsPosition.AltitudeRef.NotSpecified)
                    {
                        this.BitmapMetadata.SetQuery(XmpExifQueries.GpsAltitudeRef.Query, string.Empty);
                    }
                    else if (value == GpsPosition.AltitudeRef.AboveSeaLevel)
                    {
                        this.BitmapMetadata.SetQuery(XmpExifQueries.GpsAltitudeRef.Query, "0");
                    }
                    else if (value == GpsPosition.AltitudeRef.BelowSeaLevel)
                    {
                        this.BitmapMetadata.SetQuery(XmpExifQueries.GpsAltitudeRef.Query, "1");
                    }
                }
            }
        }

        /// <summary>
        /// Gps Latitude
        /// </summary>
        public GpsCoordinate GpsLatitude
        {
            get
            {
                // Format
                // “DDD,MM,SSk” or “DDD,MM.mmk”
                //  51,55,84N or 51,55.6784N
                string gpsLatitudeString = this.BitmapMetadata.GetQuery<string>(XmpExifQueries.GpsLatitude.Query);
                GpsCoordinate.LatitudeRef gpsLatitudeRef = this.GpsLatitudeRef;

                // Do basic validation, 3 is minimum length
                if (gpsLatitudeRef != GpsCoordinate.LatitudeRef.NotSpecified && gpsLatitudeString.Length > 3)
                {
                    // Split the string
                    string[] splitString = gpsLatitudeString.Substring(0, gpsLatitudeString.Length - 1).Split(',');

                    if (splitString.Length == 2)
                    {
                        double degrees;
                        double minutes;

                        if (Double.TryParse(splitString[0], out degrees) && Double.TryParse(splitString[1], out minutes))
                        {
                            return new GpsCoordinate(gpsLatitudeRef, degrees, minutes);
                        }
                    }
                    else if (splitString.Length == 3)
                    {
                        double degrees;
                        double minutes;
                        double seconds;

                        if (Double.TryParse(splitString[0], out degrees) && Double.TryParse(splitString[1], out minutes) && Double.TryParse(splitString[2], out seconds))
                        {
                            return new GpsCoordinate(gpsLatitudeRef, degrees, minutes, seconds);
                        }
                    }
                }

                return new GpsCoordinate();
            }

            set
            {
                if (this.ValueHasChanged(value, this.GpsLatitude))
                {
                    if (value.IsValidCoordinate)
                    {
                        string latitudeAsString = string.Format("{0},{1},{2}", value.Degrees, value.Minutes, value.Seconds);

                        this.BitmapMetadata.SetQuery(XmpExifQueries.GpsLatitude.Query, latitudeAsString);

                        this.GpsLatitudeRef = value.Numeric > 0 ? GpsCoordinate.LatitudeRef.North : GpsCoordinate.LatitudeRef.South;
                    }
                    else
                    {
                        this.BitmapMetadata.RemoveQuery(XmpExifQueries.GpsLatitude.Query);

                        this.GpsLatitudeRef = GpsCoordinate.LatitudeRef.NotSpecified;
                    }
                }
            }
        }

        public GpsCoordinate.LatitudeRef GpsLatitudeRef
        {
            get
            {
                if (this.BitmapMetadata.ContainsQuery(XmpExifQueries.GpsLatitude.Query))
                {
                    string gpsLatitudeRef = this.BitmapMetadata.GetQuery<string>(XmpExifQueries.GpsLatitude.Query);

                    if (gpsLatitudeRef.EndsWith("N"))
                    {
                        return GpsCoordinate.LatitudeRef.North;
                    }
                    else if (gpsLatitudeRef.EndsWith("S"))
                    {
                        return GpsCoordinate.LatitudeRef.South;
                    }
                }

                return GpsCoordinate.LatitudeRef.NotSpecified;
            }

            set
            {
                if (this.ValueHasChanged(value, this.GpsLatitudeRef))
                {
                    if (value == GpsCoordinate.LatitudeRef.NotSpecified)
                    {
                        this.BitmapMetadata.RemoveQuery(XmpExifQueries.GpsLatitude.Query);
                    }
                    else
                    {
                        string gpsLatitude = this.BitmapMetadata.GetQuery<string>(XmpExifQueries.GpsLatitude.Query);
                        gpsLatitude.TrimEnd('N');
                        gpsLatitude.TrimEnd('S');

                        if (value == GpsCoordinate.LatitudeRef.North)
                        {
                            this.BitmapMetadata.SetQuery(XmpExifQueries.GpsLatitude.Query, gpsLatitude + "N");
                        }
                        else if (value == GpsCoordinate.LatitudeRef.South)
                        {
                            this.BitmapMetadata.SetQuery(XmpExifQueries.GpsLatitude.Query, gpsLatitude + "S");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gps Longitude
        /// </summary>
        public GpsCoordinate GpsLongitude
        {
            get
            {
                string gpsLongitudeString = this.BitmapMetadata.GetQuery<string>(XmpExifQueries.GpsLongitude.Query);
                GpsCoordinate.LongitudeRef gpsLongitudeRef = this.GpsLongitudeRef;

                // Do basic validation, 3 is minimum length
                if (gpsLongitudeRef != GpsCoordinate.LongitudeRef.NotSpecified && !string.IsNullOrEmpty(gpsLongitudeString) && gpsLongitudeString.Length > 3)
                {
                    // Split the string
                    string[] splitString = gpsLongitudeString.Substring(0, gpsLongitudeString.Length - 1).Split(',');

                    if (splitString.Length == 2)
                    {
                        double degrees;
                        double minutes;

                        if (Double.TryParse(splitString[0], out degrees) && Double.TryParse(splitString[1], out minutes))
                        {
                            return new GpsCoordinate(gpsLongitudeRef, degrees, minutes);
                        }
                    }
                    else if (splitString.Length == 3)
                    {
                        double degrees;
                        double minutes;
                        double seconds;

                        if (Double.TryParse(splitString[0], out degrees) && Double.TryParse(splitString[1], out minutes) && Double.TryParse(splitString[2], out seconds))
                        {
                            return new GpsCoordinate(gpsLongitudeRef, degrees, minutes, seconds);
                        }
                    }
                }

                return new GpsCoordinate();
            }

            set
            {
                if (this.ValueHasChanged(value, this.GpsLongitude))
                {
                    if (value.IsValidCoordinate)
                    {
                        string latitudeAsString = string.Format("{0},{1},{2}", value.Degrees, value.Minutes, value.Seconds);

                        this.BitmapMetadata.SetQuery(XmpExifQueries.GpsLongitude.Query, latitudeAsString);

                        this.GpsLongitudeRef = value.Numeric > 0 ? GpsCoordinate.LongitudeRef.East : GpsCoordinate.LongitudeRef.West;
                    }
                    else
                    {
                        this.BitmapMetadata.RemoveQuery(XmpExifQueries.GpsLongitude.Query);

                        this.GpsLongitudeRef = GpsCoordinate.LongitudeRef.NotSpecified;
                    }
                }
            }
        }

        public GpsCoordinate.LongitudeRef GpsLongitudeRef
        {
            get
            {
                if (this.BitmapMetadata.ContainsQuery(XmpExifQueries.GpsLongitude.Query))
                {
                    string gpsLongitudeRef = this.BitmapMetadata.GetQuery<string>(XmpExifQueries.GpsLongitude.Query);

                    if (gpsLongitudeRef.EndsWith("E"))
                    {
                        return GpsCoordinate.LongitudeRef.East;
                    }
                    else if (gpsLongitudeRef.EndsWith("W"))
                    {
                        return GpsCoordinate.LongitudeRef.West;
                    }
                }

                return GpsCoordinate.LongitudeRef.NotSpecified;
            }

            set
            {
                if (this.ValueHasChanged(value, this.GpsLongitudeRef))
                {
                    if (value == GpsCoordinate.LongitudeRef.NotSpecified)
                    {
                        this.BitmapMetadata.RemoveQuery(XmpExifQueries.GpsLongitude.Query);
                    }
                    else
                    {
                        string gpsLongitude = this.BitmapMetadata.GetQuery<string>(XmpExifQueries.GpsLongitude.Query);
                        gpsLongitude.TrimEnd('E');
                        gpsLongitude.TrimEnd('W');

                        if (value == GpsCoordinate.LongitudeRef.West)
                        {
                            this.BitmapMetadata.SetQuery(XmpExifQueries.GpsLongitude.Query, gpsLongitude + "W");
                        }
                        else if (value == GpsCoordinate.LongitudeRef.East)
                        {
                            this.BitmapMetadata.SetQuery(XmpExifQueries.GpsLongitude.Query, gpsLongitude + "E");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gps Time stamp
        /// </summary>
        public DateTime GpsDateTimeStamp
        {
            get
            {
                // Format 2010-02-01T22:02:34+01:00
                DateTime gpsDateTimeStamp = this.BitmapMetadata.GetQuery<DateTime>(XmpExifQueries.GpsDateTimeStamp.Query);

                return gpsDateTimeStamp;
            }

            set
            {
                if (this.ValueHasChanged(value, this.GpsDateTimeStamp))
                {
                    if (value != null && value != new DateTime())
                    {
                        this.BitmapMetadata.RemoveQuery(XmpExifQueries.GpsDateTimeStamp.Query);
                    }
                    else
                    {
                        string gpsDateTimeStamp = value.ToString("z");

                        this.BitmapMetadata.SetQuery(XmpExifQueries.GpsDateTimeStamp.Query, gpsDateTimeStamp);
                    }
                }
            }
        }

        /// <summary>
        /// Gps Version ID
        /// </summary>
        public string GpsVersionID
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.GpsVersionID.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.GpsVersionID))
                {
                    this.BitmapMetadata.SetQuery(XmpExifQueries.GpsVersionID.Query, value);
                }
            }
        }

        /// <summary>
        /// Gps Processing Mode
        /// </summary>
        public string GpsProcessingMethod
        {
            get
            {
                if (string.IsNullOrEmpty(this.BitmapMetadata.GetQuery<string>(XmpExifQueries.GpsProcessingMethod.Query)))
                {
                    return null;
                }
                else
                {
                    return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.GpsProcessingMethod.Query);
                }
            }

            set
            {
                if (this.ValueHasChanged(value, this.GpsProcessingMethod))
                {
                    if (string.IsNullOrEmpty(value) || value == "None")
                    {
                        this.BitmapMetadata.RemoveQuery(XmpExifQueries.GpsProcessingMethod.Query);
                    }
                    else
                    {
                        this.BitmapMetadata.SetQuery(XmpExifQueries.GpsProcessingMethod.Query, value);
                    }
                }
            }
        }

        /// <summary>
        /// Gps Measure Mode
        /// </summary>
        public GpsPosition.Dimensions GpsMeasureMode
        {
            get
            {
                if (this.BitmapMetadata.ContainsQuery(XmpExifQueries.GpsMeasureMode.Query))
                {
                    string measureMode = this.BitmapMetadata.GetQuery<string>(XmpExifQueries.GpsMeasureMode.Query);

                    if (!string.IsNullOrEmpty(measureMode) && measureMode == "2")
                    {
                        return GpsPosition.Dimensions.TwoDimensional;
                    }
                    else if (!string.IsNullOrEmpty(measureMode) && measureMode == "3")
                    {
                        return GpsPosition.Dimensions.ThreeDimensional;
                    }
                }

                return GpsPosition.Dimensions.NotSpecified;
            }

            set
            {
                if (this.ValueHasChanged(value, this.GpsMeasureMode))
                {
                    switch (value)
                    {
                        default:
                        case GpsPosition.Dimensions.NotSpecified:
                            this.BitmapMetadata.SetQuery(XmpExifQueries.GpsMeasureMode.Query, string.Empty);
                            break;

                        case GpsPosition.Dimensions.ThreeDimensional:
                            this.BitmapMetadata.SetQuery(XmpExifQueries.GpsMeasureMode.Query, "3");
                            break;

                        case GpsPosition.Dimensions.TwoDimensional:
                            this.BitmapMetadata.SetQuery(XmpExifQueries.GpsMeasureMode.Query, "2");
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Aperture
        /// </summary>
        public string Aperture
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.ApertureValue.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.Aperture))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.ApertureValue.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// BrightnessValue
        /// </summary>
        public string Brightness
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.BrightnessValue.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.Brightness))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.BrightnessValue.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// Color Filter Array Pattern
        /// </summary>
        public string ColorFilterArrayPattern
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.ColorFilterArrayPattern.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.ColorFilterArrayPattern))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.ColorFilterArrayPattern.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// ColorSpace
        /// </summary>
        public string ColorSpace
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.ColorFilterArrayPattern.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.ColorFilterArrayPattern))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.ColorFilterArrayPattern.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// ComponentsConfiguration
        /// </summary>
        public string ComponentsConfiguration
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.ComponentsConfiguration.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.ComponentsConfiguration))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.ComponentsConfiguration.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// CompressedBitsPerPixel
        /// </summary>
        public string CompressedBitsPerPixel
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.CompressedBitsPerPixel.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.CompressedBitsPerPixel))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.CompressedBitsPerPixel.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// Contrast
        /// </summary>
        public string Contrast
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.Contrast.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.Contrast))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.Contrast.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// CustomRendered
        /// </summary>
        public string CustomRendered
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.CustomRendered.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.CustomRendered))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.CustomRendered.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// DateTimeDigitized
        /// </summary>
        public string DateTimeDigitized
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.DateTimeDigitized.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.DateTimeDigitized))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.DateTimeDigitized.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// DateTimeOriginal
        /// </summary>
        public string DateTimeOriginal
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.DateTimeOriginal.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.DateTimeOriginal))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.DateTimeOriginal.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// DeviceSettingDescription
        /// </summary>
        public string DeviceSettingDescription
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.DeviceSettingDescription.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.DeviceSettingDescription))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.DeviceSettingDescription.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// DigitalZoomRatio
        /// </summary>
        public string DigitalZoomRatio
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.DigitalZoomRatio.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.DigitalZoomRatio))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.DigitalZoomRatio.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// ExifVersion
        /// </summary>
        public string ExifVersion
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.ExifVersion.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.ExifVersion))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.ExifVersion.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// ExposureBiasValue
        /// </summary>
        public string ExposureBiasValue
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.ExposureBiasValue.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.ExposureBiasValue))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.ExposureBiasValue.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// ExposureIndex
        /// </summary>
        public string ExposureIndex
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.ExposureIndex.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.ExposureIndex))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.ExposureIndex.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// ExposureMode
        /// </summary>
        public string ExposureMode
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.ExposureMode.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.ExposureMode))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.ExposureMode.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// ExposureProgram
        /// </summary>
        public string ExposureProgram
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.ExposureProgram.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.ExposureProgram))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.ExposureProgram.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// ExposureTime
        /// </summary>
        public string ExposureTime
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.ExposureTime.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.ExposureTime))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.ExposureTime.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// FileSource
        /// </summary>
        public string FileSource
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.FileSource.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.FileSource))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.FileSource.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// Flash
        /// </summary>
        public string Flash
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.Flash.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.Flash))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.Flash.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// FlashEnergy
        /// </summary>
        public string FlashEnergy
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.FlashEnergy.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.FlashEnergy))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.FlashEnergy.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// FlashpixVersion
        /// </summary>
        public string FlashpixVersion
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.FlashpixVersion.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.FlashpixVersion))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.FlashpixVersion.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// FNumber
        /// </summary>
        public string FNumber
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.FNumber.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.FNumber))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.FNumber.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// FocalLength
        /// </summary>
        public string FocalLength
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.FocalLength.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.FocalLength))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.FocalLength.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// FocalLengthIn35mmFilm
        /// </summary>
        public string FocalLengthIn35mmFilm
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.FocalLengthIn35mmFilm.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.FocalLengthIn35mmFilm))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.FocalLengthIn35mmFilm.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// FocalPlaneResolutionUnit
        /// </summary>
        public string FocalPlaneResolutionUnit
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.FocalPlaneResolutionUnit.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.FocalPlaneResolutionUnit))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.FocalPlaneResolutionUnit.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// FocalPlaneXResolution
        /// </summary>
        public string FocalPlaneXResolution
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.FocalPlaneXResolution.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.FocalPlaneXResolution))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.FocalPlaneXResolution.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// ExposureMode
        /// </summary>
        public string FocalPlaneYResolution
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.FocalPlaneYResolution.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.FocalPlaneYResolution))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.FocalPlaneYResolution.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// GainControl
        /// </summary>
        public string GainControl
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.GainControl.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.GainControl))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.GainControl.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// ImageSensorType
        /// </summary>
        public string SensingMethod
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.SensingMethod.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.SensingMethod))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.SensingMethod.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// ImageUniqueID
        /// </summary>
        public string ImageUniqueID
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.ImageUniqueID.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.ImageUniqueID))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.ImageUniqueID.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// ISOSpeedRatings
        /// </summary>
        public string ISOSpeed
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.ISOSpeed.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.ISOSpeed))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.ISOSpeed.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// LightSource
        /// </summary>
        public string LightSource
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.LightSource.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.LightSource))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.LightSource.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// MaxApertureValue
        /// </summary>
        public string MaxApertureValue
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.MaxApertureValue.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.MaxApertureValue))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.MaxApertureValue.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// MeteringMode
        /// </summary>
        public string MeteringMode
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.MeteringMode.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.MeteringMode))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.MeteringMode.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// OptoElectoricConversionFunction
        /// </summary>
        public string OptoElectoricConversionFunction
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.OptoElectoricConversionFunction.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.OptoElectoricConversionFunction))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.OptoElectoricConversionFunction.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// PixelXDimension
        /// </summary>
        public string PixelXDimension
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.PixelXDimension.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.PixelXDimension))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.PixelXDimension.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// PixelYDimension
        /// </summary>
        public string PixelYDimension
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.PixelYDimension.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.PixelYDimension))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.PixelYDimension.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// RelatedSoundFile
        /// </summary>
        public string RelatedSoundFile
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.RelatedSoundFile.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.RelatedSoundFile))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.RelatedSoundFile.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// Saturation
        /// </summary>
        public string Saturation
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.Saturation.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.Saturation))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.Saturation.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// SceneCaptureType
        /// </summary>
        public string SceneCaptureType
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.SceneCaptureType.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.SceneCaptureType))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.SceneCaptureType.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// SceneType
        /// </summary>
        public string SceneType
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.SceneType.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.SceneType))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.SceneType.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// Sharpness
        /// </summary>
        public string Sharpness
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.Sharpness.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.Sharpness))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.Sharpness.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// ShutterSpeedValue
        /// </summary>
        public string ShutterSpeedValue
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.ShutterSpeedValue.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.ShutterSpeedValue))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.ShutterSpeedValue.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// SpatialFrequencyResponse
        /// </summary>
        public string SpatialFrequencyResponse
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.SpatialFrequencyResponse.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.SpatialFrequencyResponse))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.SpatialFrequencyResponse.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// SpectralSensitivity
        /// </summary>
        public string SpectralSensitivity
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.SpectralSensitivity.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.SpectralSensitivity))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.SpectralSensitivity.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// SubjectArea
        /// </summary>
        public string SubjectArea
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.SubjectArea.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.SubjectArea))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.SubjectArea.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// SubjectDistance
        /// </summary>
        public string SubjectDistance
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.SubjectDistance.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.SubjectDistance))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.SubjectDistance.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// SubjectDistanceRange
        /// </summary>
        public string SubjectDistanceRange
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.SubjectDistanceRange.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.SubjectDistanceRange))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.SubjectDistanceRange.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// SubjectLocation
        /// </summary>
        public string SubjectLocation
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.SubjectLocation.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.SubjectLocation))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.SubjectLocation.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// UserComment
        /// </summary>
        public string UserComment
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.UserComment.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.UserComment))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.UserComment.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }

        /// <summary>
        /// WhiteBalance
        /// </summary>
        public string WhiteBalance
        {
            get
            {
                return this.BitmapMetadata.GetQuery<string>(XmpExifQueries.WhiteBalance.Query);
            }

            set
            {
                if (this.ValueHasChanged(value, this.WhiteBalance))
                {
                    this.BitmapMetadata.SetQueryOrRemove(XmpExifQueries.WhiteBalance.Query, value, string.IsNullOrEmpty(value));
                }
            }
        }
    }
}