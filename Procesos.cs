using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime.InteropServices;

namespace WhatsAppLive
{
    public class Procesos
    {
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_RESTORE = 9;

        public void CheckAndLaunchProcesses()
        {
            // Comprobar y lanzar OBS
            LaunchProcessIfNotRunning("obs64", @"C:\Program Files\obs-studio\bin\64bit\obs64.exe");

            // Comprobar y lanzar Webcam.exe
            LaunchProcessIfNotRunning("Webcam", @"C:\Program Files\NDI\NDI 6 Tools\Webcam\Webcam.exe");

            // Comprobar y lanzar EntradaBlackMagic.vmix (vMix)
            LaunchProcessIfNotRunning("vMix", @"C:\Users\AUTOMATIZACION\Documents\vMixStorage\EntradaBlackMagic.vmix");

            // Comprobar si WhatsApp está en ejecución
            var whatsappProcess = GetRunningProcess("WhatsApp");

            if (whatsappProcess == null)
            {
                MessageBox.Show("WhatsApp Desktop no está en ejecución. Por favor, inícialo manualmente.");
            }
            else
            {
                IntPtr handle = whatsappProcess.MainWindowHandle;

                if (handle != IntPtr.Zero)
                {
                    ShowWindow(handle, SW_RESTORE);
                    SetForegroundWindow(handle);
                }
            }
        }

        private void LaunchProcessIfNotRunning(string processName, string fullPath)
        {
            if (!IsProcessRunning(processName))
            {
                try
                {
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = fullPath,
                        WorkingDirectory = System.IO.Path.GetDirectoryName(fullPath)
                    };
                    Process.Start(startInfo);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"No se pudo iniciar {processName}: {ex.Message}");
                }
            }
        }

        private bool IsProcessRunning(string processName)
        {
            return Process.GetProcessesByName(processName).Length > 0;
        }

        private Process GetRunningProcess(string processName)
        {
            return Process.GetProcessesByName(processName).FirstOrDefault();
        }
    }
}
