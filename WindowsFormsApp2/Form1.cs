#define _CRT_SECURE_NO_WARNINGS
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using PInvoke;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }



        private void button1_Click(object sender, EventArgs e)
        {
            RotateScreen(0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RotateScreen(1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            RotateScreen(2);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            RotateScreen(3);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    
        [DllImport("user32.dll")]
        private static extern int EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);

        [DllImport("user32.dll")]
        private static extern int ChangeDisplaySettingsEx(string lpszDeviceName, ref DEVMODE lpDevMode, IntPtr hwnd, ChangeDisplaySettingsFlags dwflags, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern int EnumDisplaySettings(string lpszDeviceName, int iModeNum, ref DEVMODE lpDevMode);


        private const int ENUM_CURRENT_SETTINGS = -1;
        private const int DISP_CHANGE_SUCCESSFUL = 0;

        [StructLayout(LayoutKind.Sequential)]
        public struct DEVMODE
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public int dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmFormName;
            public short dmLogPixels;
            public int dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;
            public int dmPanningWidth;
            public int dmPanningHeight;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAY_DEVICE
        {
            public int cb;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceString;
            public uint StateFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceKey;
        }

        [Flags]
        private enum ChangeDisplaySettingsFlags : uint
        {
            CDS_NONE = 0,
            CDS_UPDATEREGISTRY = 0x00000001,
            CDS_TEST = 0x00000002,
            CDS_FULLSCREEN = 0x00000004,
            CDS_GLOBAL = 0x00000008,
            CDS_SET_PRIMARY = 0x00000010,
            CDS_VIDEOPARAMETERS = 0x00000020,
            CDS_ENABLE_UNSAFE_MODES = 0x00000100,
            CDS_DISABLE_UNSAFE_MODES = 0x00000200,
            CDS_RESET = 0x40000000,
            CDS_RESET_EX = 0x20000000,
            CDS_NORESET = 0x10000000
        }
        public static int screenState = 0;
        public static void RotateScreen(int angle)
        {
            DISPLAY_DEVICE displayDevice = new DISPLAY_DEVICE();
            displayDevice.cb = Marshal.SizeOf(typeof(DISPLAY_DEVICE));
            int devNum = 0;

            if (EnumDisplayDevices(null, (uint)devNum, ref displayDevice, 0) != 0)
            {
                DEVMODE devMode = new DEVMODE();
                devMode.dmSize = (short)Marshal.SizeOf(typeof(DEVMODE));

                if (EnumDisplaySettings(displayDevice.DeviceName, ENUM_CURRENT_SETTINGS, ref devMode) == 0)
                {
                    Console.WriteLine($"Failed to get current display settings for device {displayDevice.DeviceName}");
                    devNum++;
                    //continue;
                }

                //for rotate screen size
                if (screenState == 0 && (angle == 1 || angle == 3))
                {
                    screenState = 1;

                    int temp = devMode.dmPelsHeight;
                    devMode.dmPelsHeight = devMode.dmPelsWidth;
                    devMode.dmPelsWidth = temp;
                }
                if(screenState == 1 && (angle == 0 || angle == 2))
                {
                    screenState = 0;

                    int temp = devMode.dmPelsHeight;
                    devMode.dmPelsHeight = devMode.dmPelsWidth;
                    devMode.dmPelsWidth = temp;
                }

                devMode.dmDisplayOrientation = angle;
                // devMode.dmYResolution = 420;
                // devMode.dmPositionX = 420;

                //ChangeDisplaySettingsEx(displayDevice.DeviceName, ref devMode, IntPtr.Zero, (ChangeDisplaySettingsFlags.CDS_ENABLE_UNSAFE_MODES), IntPtr.Zero);//| ChangeDisplaySettingsFlags.CDS_NORESET
                int result = ChangeDisplaySettingsEx("\\\\.\\DISPLAY1", ref devMode, IntPtr.Zero, (ChangeDisplaySettingsFlags.CDS_UPDATEREGISTRY), IntPtr.Zero);

                if (result == DISP_CHANGE_SUCCESSFUL)
                {
                    Console.WriteLine($"Screen rotation successful for device {displayDevice.DeviceName}");
                    Console.WriteLine($"Screen rotation successful for device {devMode.dmSize}");
                    Console.WriteLine($"Screen rotation successful for device {devMode.dmPositionX}");
                }
                else
                {
                    Console.WriteLine($"Screen rotation failed for device {displayDevice.DeviceName} with error code: {result}");
                }

                //devNum++;
            }
        }
        
    }
}
