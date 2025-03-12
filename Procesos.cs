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
        // Importar función de la API de Windows para traer un proceso al frente
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        public void CheckAndLaunchProcesses()
        {
            // Comprobar si OBS está en ejecución, si no, lanzarlo
            if (!IsProcessRunning("obs64"))
            {
                try
                {
                    Process.Start("obs64.exe");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"No se pudo iniciar OBS: {ex.Message}");
                }
            }

            // Comprobar si WhatsApp está en ejecución
            var whatsappProcess = GetRunningProcess("WhatsApp");

            if (whatsappProcess == null)
            {
                MessageBox.Show("WhatsApp Desktop no está en ejecución. Por favor, inícialo manualmente.");
            }
            else
            {
                // Llevar WhatsApp al frente si está ejecutándose
                if (whatsappProcess.MainWindowHandle != IntPtr.Zero)
                {
                    SetForegroundWindow(whatsappProcess.MainWindowHandle);
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
