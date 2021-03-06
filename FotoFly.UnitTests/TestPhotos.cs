﻿namespace Fotofly.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class TestPhotos
    {
        public static readonly string PhotosFolder = @"..\..\..\~Sample Files\JpgPhotos\";

        public static readonly string TagsWithUnicode = "TagsWithUnicode.jpg";

        public static readonly string MakeKodakDX4900 = "Make-KodakDX4900.jpg";
        public static readonly string MakeNikonCoolPixP80 = "Make-NikonCoolPixP80.jpg";
        public static readonly string MakeNikonD70 = "Make-NikonD70.jpg";
        public static readonly string MakePentaxOptioS = "Make-PentaxOptioS.jpg";
        public static readonly string MakeSonyDSCT30 = "Make-SonyDSCT30.jpg";
        public static readonly string MakeiPhone3GsUntouched = "Make-iPhone3Gs-Untouched.jpg";
        public static readonly string MakeiPhone3GsWithTags = "Make-iPhone3Gs-WithTags.jpg";

        public static readonly string SchemaXmpMicrosoft = "Schema-XmpMicrosoft.jpg";
        public static readonly string SchemaXmpExif = "Schema-XmpExif.jpg";
        public static readonly string SchemaXmpXap = "Schema-XmpXap.jpg";
        public static readonly string SchemaXmpTiff = "Schema-XmpTiff.jpg";
        public static readonly string SchemaIptc = "Schema-Iptc.jpg";

        public static readonly string NoPadding = "NoPadding.jpg";

        public static readonly string LightSourceAuto = "LightSource-Auto.jpg";
        public static readonly string LightSourceCloudy = "LightSource-Cloudy.jpg";
        public static readonly string LightSourceDaylight = "LightSource-Daylight.jpg";
        public static readonly string LightSourceEvaluative = "LightSource-Evaluative.jpg";
        public static readonly string LightSourceFlash = "LightSource-Flash.jpg";
        public static readonly string LightSourceFlourescentH = "LightSource-FlourescentH.jpg";
        public static readonly string LightSourceFluorescent = "LightSource-Fluorescent.jpg";
        public static readonly string LightSourceTungsten = "LightSource-Tungsten.jpg";
        public static readonly string LightSourceUnderwater = "LightSource-Underwater.jpg";

        public static readonly string MeteringModeCenterWeightedAverage = "MeteringMode-CenterWeightedAverage.jpg";
        public static readonly string MeteringModeEvaluative = "MeteringMode-Evaluative.jpg";
        public static readonly string MeteringModeSpot = "MeteringMode-Spot.jpg";

        public static readonly string DigitalZoom = "DigitalZoom.jpg";

        public static readonly string ExposureBiasMinus03 = "ExposureBias-0.3.jpg";
        public static readonly string ExposureBiasMinus10 = "ExposureBias-1.0.jpg";
        public static readonly string ExposureBiasMinus13 = "ExposureBias-1.3.jpg";

        public static readonly string ExposureBiasPlus03 = "ExposureBias+0.3.jpg";
        public static readonly string ExposureBiasPlus10 = "ExposureBias+1.0.jpg";
        public static readonly string ExposureBiasPlus13 = "ExposureBias+1.3.jpg";

        public static readonly string ShutterSpeed1Over60 = "ShutterSpeed-1-60.jpg";
        public static readonly string ShutterSpeed1Over285 = "ShutterSpeed-1-285.jpg";
        public static readonly string ShutterSpeed1Over1000 = "ShutterSpeed-1-1000.jpg";
        public static readonly string ShutterSpeed1Over2 = "ShutterSpeed-1-2.jpg";
        public static readonly string ShutterSpeed1Over10 = "ShutterSpeed-1-10.jpg";
        public static readonly string ShutterSpeed2Seconds5 = "ShutterSpeed-2.5.jpg";
        public static readonly string ShutterSpeed10Seconds = "ShutterSpeed-10.jpg";

        public static readonly string Aperture28 = "Aperture-2.8.jpg";
        public static readonly string Aperture71 = "Aperture-7.1.jpg";
        public static readonly string Aperture80 = "Aperture-8.0.jpg";

        public static readonly string FlashAuto = "Flash-Auto.jpg";
        public static readonly string FlashManual = "Flash-Manual.jpg";

        public static readonly string GeotaggedKin = "GeotaggedKin.jpg";

        public static readonly string GeotaggedExif1 = "GeotaggedExif1.jpg";

        public static readonly string GeotaggedXmp1 = "GeotaggedXmp1.jpg";
        public static readonly string GeotaggedXmp2 = "GeotaggedXmp2.jpg";
        public static readonly string GeotaggedXmp3 = "GeotaggedXmp3.jpg";
        public static readonly string GeotaggedXmp4 = "GeotaggedXmp4.jpg";

        public static readonly string ISO400 = "ISO-400.jpg";
        public static readonly string ISOAuto = "ISO-Auto.jpg";

        public static readonly string ExposureProgramAv = "ExposureProgram-Av.jpg";
        public static readonly string ExposureProgramManual = "ExposureProgram-Manual.jpg";
        public static readonly string ExposureProgramP = "ExposureProgram-P.jpg";
        public static readonly string ExposureProgramTV = "ExposureProgram-Tv.jpg";

        public static readonly string Orientation180Clockwise = "Orientation-180Clockwise.jpg";
        public static readonly string Orientation270Clockwise = "Orientation-270Clockwise.jpg";
        public static readonly string Orientation90Clockwise = "Orientation-90Clockwise.jpg";
        public static readonly string OrientationHorizontal = "Orientation-Horizontal.jpg";

        public static readonly string Regions0 = "Regions-0.jpg";
        public static readonly string Regions1 = "Regions-1.jpg";
        public static readonly string Regions2 = "Regions-2.jpg";

        // Not a real file, used to save the during testing
        public static readonly string CorruptExposureBias = "CorruptExposureBias.jpg";

        // Basic Metadata
        public static readonly string UnitTest1 = "UnitTest-1.jpg";

        // Contains GPS Data
        public static readonly string UnitTest2 = "UnitTest-2.jpg";

        // For comparisons
        public static readonly string UnitTest3 = "UnitTest-3.jpg";

        // Not a real file, used to save the during testing
        public static readonly string UnitTestTemp1 = "UnitTest-Temp1.jpg";
        public static readonly string UnitTestTemp2 = "UnitTest-Temp2.jpg";
        public static readonly string UnitTestTemp3 = "UnitTest-Temp3.jpg";
        public static readonly string UnitTestTemp4 = "UnitTest-Temp4.jpg";
        public static readonly string UnitTestTemp5 = "UnitTest-Temp5.jpg";
        public static readonly string UnitTestTemp6 = "UnitTest-Temp6.jpg";
        public static readonly string UnitTestTemp7 = "UnitTest-Temp7.jpg";
        public static readonly string UnitTestTemp8 = "UnitTest-Temp8.jpg";

        // WriteGpsMetadata
        public static readonly string UnitTestTemp9 = "UnitTest-Temp9.jpg";

        // WriteAddressMetadata
        public static readonly string UnitTestTemp10 = "UnitTest-Temp10.jpg";

        public static JpgPhoto Load(string name)
        {
            JpgPhoto jpgPhoto = new JpgPhoto(PhotosFolder + name);
            return jpgPhoto;
        }
    }
}
