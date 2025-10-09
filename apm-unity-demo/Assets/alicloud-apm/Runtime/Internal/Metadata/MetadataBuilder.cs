using System;
using UnityEngine;

namespace Alicloud.Apm
{
    /// <summary>
    /// Automatically collects metadata and serializes to JSON in a space-efficient way.
    /// </summary>
    internal class MetadataBuilder
    {
        public static string METADATA_KEY = "com.alicloud.apm.metadata.unity";

        public static string GenerateMetadataJSON()
        {
            try
            {
                Metadata metadata = new Metadata();
                return JsonUtility.ToJson(metadata);
            }
            catch (Exception e)
            {
                ApmLogger.Error(
                    "Failed to generate Unity-specific metadata for APM due to: " + e.ToString()
                );
                return "";
            }
        }
    }
}
