using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using AudioSwitcher.AudioApi.CoreAudio;

namespace WhatsAppLive;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private string deviceLive; // Nombre exacto del dispositivo
    private string deviceOff;  // Nombre exacto del dispositivo
    private CoreAudioController audioController = new CoreAudioController();

    public MainWindow()
    {
        InitializeComponent();
        ConfigManager.LoadConfig();
        deviceLive = ConfigManager.Device_LIVE;
        deviceOff = ConfigManager.Device_OFF;
        DetectAndHandleUnknownAudioDevice();
        UpdateButtonState();
        Procesos p = new Procesos();
        p.CheckAndLaunchProcesses();
    }


    //CAMBIO EN SISTEMA AUDIO
    private void btnSwitch_Click(object sender, RoutedEventArgs e)
    {
        ToggleAudioDevice();
        UpdateButtonState();
    }

    private void ToggleAudioDevice()
    {
        var currentDevice = audioController.DefaultPlaybackDevice;
        // MessageBox.Show($"Dispositivo actual antes de cambiar: {currentDevice.FullName}");

        var targetDevice = audioController.GetPlaybackDevices()
                                      .Where(d => d.FullName == (currentDevice.FullName == deviceLive ? deviceOff : deviceLive))
                                      .FirstOrDefault();
        //MessageBox.Show($"Target: {targetDevice.FullName}");
        if (targetDevice != null)
        {
            try
            {
                targetDevice.SetAsDefault();
                targetDevice.SetAsDefaultCommunications();

                var newCurrentDevice = audioController.DefaultPlaybackDevice;
                //MessageBox.Show($"Dispositivo actual después de cambiar: {newCurrentDevice.FullName}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cambiar dispositivo de audio: {ex.Message}");
            }
        }
        else
        {
            MessageBox.Show("No se encontró el dispositivo de destino.");
        }
    }

    private void DetectAndHandleUnknownAudioDevice()
    {
        var currentDevice = audioController.DefaultPlaybackDevice;
        if (!currentDevice.FullName.Contains(deviceLive) && !currentDevice.FullName.Contains(deviceOff))
        {
            MessageBoxResult result = MessageBox.Show(
                $"Actualmente se está utilizando una salida de audio distinta a la configurada como 'Fuera de emisión':\n\n" +
                $"Dispositivo actual: {currentDevice.FullName}\n\n" +
                $"¿Deseas usar este dispositivo como 'Fuera de emisión'?",
                "Dispositivo desconocido",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                deviceOff = currentDevice.FullName; // Guardar el nuevo dispositivo como "Off"
            }
            else
            {
                // Restaurar la salida de audio a la preconfigurada en deviceOff
                var defaultOffDevice = audioController.GetPlaybackDevices()
                                        .FirstOrDefault(d => d.FullName.Contains(deviceOff));

                if (defaultOffDevice != null)
                {
                    defaultOffDevice.SetAsDefault();
                    defaultOffDevice.SetAsDefaultCommunications();
                }
            }
        }
    }

    private void UpdateButtonState()
    {
        var currentDevice = audioController.DefaultPlaybackDevice;
        bool isLive = currentDevice.FullName.Contains(deviceLive);

        btnSwitch.Content = isLive ? "LIVE" : "OFF";
        btnSwitch.Foreground = isLive ? Brushes.White : Brushes.Black;
        btnSwitch.Background = isLive ? Brushes.Green : Brushes.LightGray;
    }
}