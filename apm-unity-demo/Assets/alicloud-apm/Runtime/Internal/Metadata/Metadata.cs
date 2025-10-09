using System;
using UnityEngine;

namespace Alicloud.Apm
{
    /// <summary>
    /// Represents the Unity-specific metadata object that is transformed into JSON.
    ///
    /// Metadata keys are acronyms to save space as there are limits to the maximum size of Keys in
    /// the Android, iOS and tvOS SDKs.
    /// </summary>
    internal class Metadata
    {
        // unityVersion: Version of Unity Engine
        public string uv;

        // isDebugBuild: Whether "Development Build" is checked
        public bool idb;

        // processorType
        public string pt;

        // processorCount: Number of cores
        public int pc;

        // processorFrequency
        public int pf;

        // systemMemorySize: RAM size
        public int sms;

        // graphicsMemorySize
        public int gms;

        // graphicsDeviceID
        public int gdid;

        // graphicsDeviceVendorID
        public int gdvid;

        // graphicsDeviceName
        public string gdn;

        // graphicsDeviceVendor
        public string gdv;

        // graphicsDeviceVersion
        public string gdver;

        // graphicsDeviceType
        public UnityEngine.Rendering.GraphicsDeviceType gdt;

        // graphicsShaderLevel
        // https://docs.unity3d.com/540/Documentation/ScriptReference/SystemInfo-graphicsShaderLevel.html
        public int gsl;

        // graphicsRenderTargetCount
        public int grtc;

        // graphicsCopyTextureSupport
        public UnityEngine.Rendering.CopyTextureSupport gcts;

        // graphicsMaxTextureSize
        public int gmts;

        // screenSize
        public string ss;

        // screenDPI
        public float sdpi;

        // screenRefreshRate
        public double srr;

        public Metadata()
        {
            uv = Application.unityVersion;
            idb = Debug.isDebugBuild;
            pt = SystemInfo.processorType;
            pc = SystemInfo.processorCount;
            pf = SystemInfo.processorFrequency;
            sms = SystemInfo.systemMemorySize;
            gms = SystemInfo.graphicsMemorySize;
            gdid = SystemInfo.graphicsDeviceID;
            gdvid = SystemInfo.graphicsDeviceVendorID;
            gdn = SystemInfo.graphicsDeviceName;
            gdv = SystemInfo.graphicsDeviceVendor;
            gdver = SystemInfo.graphicsDeviceVersion;
            gdt = SystemInfo.graphicsDeviceType;
            gsl = SystemInfo.graphicsShaderLevel;
            grtc = SystemInfo.supportedRenderTargetCount;
            gcts = SystemInfo.copyTextureSupport;
            gmts = SystemInfo.maxTextureSize;
            ss = String.Format("{0}x{1}", Screen.width, Screen.height);
            sdpi = Screen.dpi;
#if UNITY_2021_2_OR_NEWER
            srr = Screen.currentResolution.refreshRateRatio.value;
#else
            srr = Screen.currentResolution.refreshRate; // 旧版本仍可用
#endif
        }
    }
}
