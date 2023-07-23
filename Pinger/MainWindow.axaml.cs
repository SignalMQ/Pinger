using System;
using System.Net;
using System.Net.Sockets;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MsBox.Avalonia;

namespace Pinger;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        Check.Click += CheckOnClick;
        Clear.Click += ClearOnClick;
    }

    private void ClearOnClick(object? sender, RoutedEventArgs e)
    {
        Address.Clear();
        Port.Clear();
        Result.Clear();
        Increase.Value = 1;
    }

    private void CheckOnClick(object? sender, RoutedEventArgs e)
    {
        if (CheckConnection().Item1)
        {
            for (int i = 0; i < Increase.Value; i++)
            {
                Connect(CheckConnection().Item3, CheckConnection().Item4 + i);
            }
        }
        else
        {
            MessageBoxManager.GetMessageBoxStandard("Error!", CheckConnection().Item2)
                .ShowAsPopupAsync(this);
            ClearOnClick(null, null);
        }
    }

    private (bool, string, IPAddress, int) CheckConnection(string message = "", bool checkStatus = false, IPAddress? ip = null)
    {
        if (!IPAddress.TryParse(Address.Text, out ip) && ip is null)
        {
            message += "Address value is empty or not recognized!\n";
        }
        if (!int.TryParse(Port.Text, out var port))
        {
            message += "Port value is empty or not recognized!\n";
        }
        else if (port > 65535)
        {
            message += "Port value must be in range from 0 to 65535!\n";
        }
        else if (Increase.Value + port > 65535)
        {
            message += "Result of increase port value is greeter than maximum allowable port!\n";
        }
        else if (message == string.Empty)
        {
            checkStatus = true;
        }
        
        return (checkStatus, message, ip, port);
    }

    private void Connect(IPAddress ip, int port)
    {
        string format = "{0} \t | \t {1}";
        TcpClient client = new TcpClient();
        try
        {
            client.ConnectAsync(ip, port).Wait(0);
            Result.Text += $"{ip.ToString()}:{port} | OK\n";
        }
        catch (Exception ex)
        {
            Result.Text += $"{ip.ToString()}:{port} \t| {ex.Message}\n";
        }

        Result.CaretIndex = Result.Text.Length;
        client.Close();
    }
}