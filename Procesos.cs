using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WhatsAppLive
{
    public class Procesos
    {
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_RESTORE = 9;
        private const int SW_MINIMIZE = 6;

        public void CheckAndLaunchProcesses()
        {
            // Comprobar y lanzar OBS
            LaunchProcessIfNotRunning("obs64", @"C:\Program Files\obs-studio\bin\64bit\obs64.exe");

            // Comprobar y lanzar Webcam.exe
            LaunchProcessIfNotRunning("Webcam", @"C:\Program Files\NDI\NDI 6 Tools\Webcam\Webcam.exe");

            // Comprobar y lanzar EntradaBlackMagic.vmix (vMix)
            LaunchVMixProject(@"C:\Users\AUTOMATIZACION\Documents\vMixStorage\EntradaBlackMagic.vmix");

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

        private void LaunchVMixProject(string projectPath)
        {
            if (!IsProcessRunning("vMix64"))
            {
                try
                {
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = @"C:\Program Files (x86)\vMix\vMix64.exe",
                        Arguments = $"\"{projectPath}\"",
                        WorkingDirectory = @"C:\Program Files (x86)\vMix\"
                    };

                    var process = Process.Start(startInfo);

                    if (process != null)
                    {
                        // Esperar un poco para asegurarse de que la ventana se abra
                        Thread.Sleep(2000);

                        // Minimizar la ventana si está abierta
                        IntPtr handle = process.MainWindowHandle;
                        if (handle != IntPtr.Zero)
                        {
                            ShowWindow(handle, SW_MINIMIZE);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"No se pudo iniciar vMix con el proyecto especificado: {ex.Message}");
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
