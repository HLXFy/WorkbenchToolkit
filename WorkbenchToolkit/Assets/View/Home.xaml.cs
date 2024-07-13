using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Management;
using System.IO;

namespace WorkbenchToolkit.Assets.View
{
    /// <summary>
    /// Java.xaml 的交互逻辑
    /// </summary>
    public partial class Java : Page
    {
        private PerformanceCounter cpuCounter;
        private PerformanceCounter ramCounter;
        private DriveInfo[] drives;
        public Java()
        {
            InitializeComponent();
            // 初始化CPU和内存的PerformanceCounter
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            ramCounter = new PerformanceCounter("Memory", "Available MBytes");

            // 获取所有硬盘信息
            drives = DriveInfo.GetDrives();

            // 启动定时器，每隔一段时间更新一次进度条
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();


            // Get motherboard manufacturer
            txtMotherboard.Text = GetHardwareInfo("Win32_BaseBoard", "Manufacturer");

            // Get memory manufacturer (assumes you want the manufacturer of the first memory device)
            txtMemory.Text = GetMemoryManufacturer();

            // Get graphics card manufacturer and model
            string[] graphicsCardInfo = GetGraphicsCardInfo();
            txtGraphicsCardManufacturer.Text = graphicsCardInfo[0];
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string windowsVersion = GetWindowsVersion();
            versionInfoTextBlock.Text = windowsVersion;
        }

        private string GetWindowsVersion()
        {
            StringBuilder versionInfo = new StringBuilder();

            // 获取主版本号和次版本号
            OperatingSystem os = Environment.OSVersion;
            versionInfo.AppendLine($"系统: Windows {os.Version.Major}.{os.Version.Minor}");

            // 获取服务包版本
            if (os.ServicePack != "")
            {
                versionInfo.AppendLine($"Service Pack: {os.ServicePack}");
            }

            // 获取系统架构信息
            versionInfo.AppendLine($"架构: {RuntimeInformation.OSArchitecture}");

            // 获取详细版本信息
            versionInfo.AppendLine($"版本号: {os.VersionString}");

            return versionInfo.ToString();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // 更新CPU进度条
            float cpuUsage = cpuCounter.NextValue();
            cpuProgressBar.Value = cpuUsage;

            // 更新内存进度条
            float ramUsage = (ramCounter.NextValue() / 1024); // 转换为MB
            memoryProgressBar.Value = ramUsage;

            // 更新硬盘进度条（这里示例更新C盘的占用情况）
            foreach (DriveInfo drive in drives)
            {
                if (drive.IsReady && drive.Name == "C:\\")
                {
                    double diskUsage = ((drive.TotalSize - drive.TotalFreeSpace) / (double)drive.TotalSize) * 100;
                    diskProgressBar.Value = diskUsage;
                    break;
                }
            }
        }
        private string GetHardwareInfo(string wmiClass, string propertyName)
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", $"SELECT * FROM {wmiClass}");
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    return queryObj[propertyName]?.ToString();
                }
            }
            catch (ManagementException)
            {
                // Handle exception
            }
            return "N/A";
        }

        // Method to retrieve memory manufacturer (assuming the first memory device)
        private string GetMemoryManufacturer()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PhysicalMemory");
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    return queryObj["Manufacturer"]?.ToString();
                }
            }
            catch (ManagementException)
            {
                // Handle exception
            }
            return "N/A";
        }

        // Method to retrieve graphics card manufacturer and model
        private string[] GetGraphicsCardInfo()
        {
            string[] info = new string[2] { "N/A", "N/A" };
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_VideoController");
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    info[0] = queryObj["AdapterManufacturer"]?.ToString();
                    info[1] = queryObj["Caption"]?.ToString();
                    break; // Assuming we only need info from the first adapter found
                }
            }
            catch (ManagementException)
            {
                // Handle exception
            }
            return info;
        }
    }
}
