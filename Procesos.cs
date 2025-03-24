using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            bool allSuccessful = true;
            string errorMessage = "";

            // Comprobar y lanzar OBS
            if (!LaunchProcessIfNotRunning("obs64", @"C:\Program Files\obs-studio\bin\64bit\obs64.exe"))
            {
                allSuccessful = false;
                errorMessage += "No se pudo iniciar OBS.\n";
            }

            // Comprobar y lanzar Webcam.exe
            if (!LaunchProcessIfNotRunning("Webcam", @"C:\Program Files\NDI\NDI 6 Tools\Webcam\Webcam.exe"))
            {
                allSuccessful = false;
                errorMessage += "No se pudo iniciar Webcam.\n";
            }

            // Comprobar y lanzar vMix con su proyecto
            if (!LaunchVMixProject(@"C:\Users\AUTOMATIZACION\Documents\vMixStorage\EntradaBlackMagic.vmix"))
            {
                allSuccessful = false;
                errorMessage += "No se pudo iniciar vMix.\n";
            }

            // Comprobar si WhatsApp está en ejecución
            var whatsappProcess = GetRunningProcess("WhatsApp");

            if (whatsappProcess == null)
            {
                allSuccessful = false;
                errorMessage += "WhatsApp Desktop no está en ejecución. Por favor, inícialo manualmente.\n";
            }
            else
            {
                IntPtr handle = whatsappProcess.MainWindowHandle;

                if (handle != IntPtr.Zero)
                {
                    ShowWindow(handle, SW_MINIMIZE);
                }
            }

            // Mostrar resultados
            if (allSuccessful)
            {
                MessageBox.Show("Todos los programas se iniciaron correctamente.");
                Environment.Exit(0); // Cerrar aplicación si todo salió bien
            }
            else
            {
                MessageBox.Show(errorMessage);
            }
        }

        private bool LaunchProcessIfNotRunning(string processName, string fullPath)
        {
            if (!IsProcessRunning(processName))
            {
                try
                {
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = fullPath,
                        WorkingDirectory = Path.GetDirectoryName(fullPath)
                    };
                    Process.Start(startInfo);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        private bool LaunchVMixProject(string projectPath)
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
                        Thread.Sleep(2000); // Esperar para que vMix abra la ventana
                        IntPtr handle = process.MainWindowHandle;

                        if (handle != IntPtr.Zero)
                        {
                            ShowWindow(handle, SW_MINIMIZE); // Minimizar la ventana
                        }
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsProcessRunning(string processName)
        {
            return Process.GetProcessesByName(processName).Length > 0;
        }

        private Process GetRunningProcess(string processName)
        {
            var processes = Process.GetProcessesByName(processName);
            return processes.Length > 0 ? processes[0] : null;
        }
    }
}
