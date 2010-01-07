﻿// <copyright file="XmpMicrosoftQueries.cs" company="Taasss">Copyright (c) 2009 All Right Reserved</copyright>
// <author>Ben Vincent</author>
// <date>2009-11-04</date>
// <summary>Xmp Microsoft Queries</summary>
namespace FotoFly.MetadataQueries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Media.Imaging;

    public static class XmpMicrosoftQueries
    {
        public static readonly MetdataQuery<string, string> Rating = new MetdataQuery<string, string>("/xmp/MicrosoftPhoto:Rating");

        // "DateAcquired" (LPWSTR) ["http://ns.microsoft.com/photo/1.0/" (LPWSTR)]
        public static readonly MetdataQuery<string, DateTime> DateAcquired = new MetdataQuery<string, DateTime>("/xmp/MicrosoftPhoto:DateAcquired");

        // "RegionInfo" (LPWSTR) ["http://ns.microsoft.com/photo/1.2/" (LPWSTR)]
        public static readonly MetdataQuery<BitmapMetadata, XmpRegionInfo> RegionInfo = new MetdataQuery<BitmapMetadata, XmpRegionInfo>(@"/xmp/MP:RegionInfo");

        // MPRI:DateRegionsValid
        // Does not appear to be ever used
        public static readonly MetdataQuery<string, DateTime> RegionsLastUpdate = new MetdataQuery<string, DateTime>(@"/xmp/MP:RegionInfo/MPRI:DateRegionsValid");

        // "Regions" (LPWSTR) ["http://ns.microsoft.com/photo/1.2/t/RegionInfo#" (LPWSTR)]
        public static readonly MetdataQuery<BitmapMetadata, List<XmpRegion>> Regions = new MetdataQuery<BitmapMetadata, List<XmpRegion>>(@"/xmp/MP:RegionInfo/MPRI:Regions");

        // Region query, meant to be used with String.Format to replace {0} with the appropriate region
        public static readonly MetdataQuery<BitmapMetadata, XmpRegion> Region = new MetdataQuery<BitmapMetadata, XmpRegion>(@"/xmp/MP:RegionInfo/MPRI:Regions/{{ulong={0}}}");

        // "PersonDisplayName" (LPWSTR) ["http://ns.microsoft.com/photo/1.2/t/Region#" (LPWSTR)]
        public static readonly MetdataQuery<string, string> RegionPersonDisplayName = new MetdataQuery<string, string>(@"/MPReg:PersonDisplayName");

        // "Rectangle" (LPWSTR) ["http://ns.microsoft.com/photo/1.2/t/Region#" (LPWSTR)]
        public static readonly MetdataQuery<string, string> RegionRectangle = new MetdataQuery<string, string>(@"/MPReg:Rectangle");

        // "PersonEmailDigest" (LPWSTR) ["http://ns.microsoft.com/photo/1.2/t/Region#" (LPWSTR)]
        public static readonly MetdataQuery<string, string> RegionPersonEmailDigest = new MetdataQuery<string, string>(@"/MPReg:PersonEmailDigest");
        public static readonly MetdataQuery<string, string> RegionPersonLiveIdCID = new MetdataQuery<string, string>(@"/MPReg:PersonLiveIdCID");
    }
}