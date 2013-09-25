using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using Microsoft.Win32;
using IsbmClient;

namespace IsbmPublisher
{
    public partial class MainWindow : Window
    {
        private string _sessionId;
        private ISBMChannelManagementServiceClient _channelClient;
        private ISBMProviderPublicationServiceClient _providerClient;

        public MainWindow()
        {
            InitializeComponent();

            txtChannel.Text = Properties.Settings.Default.DefaultChannel;
            txtTopic.Text = Properties.Settings.Default.DefaultTopic;
            txtFile.Text = Properties.Settings.Default.DefaultFile;

            _channelClient = new ISBMChannelManagementServiceClient();
            _providerClient = new ISBMProviderPublicationServiceClient();
        }

        private void btnFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Select XML file",
                Filter = "All files (*.*)|*.*",
                CheckFileExists = true
            };
            if (dialog.ShowDialog() == true)
            {
                txtFile.Text = dialog.FileName;
            }
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            try
            {
                Channel channel = _channelClient.GetChannel(txtChannel.Text);
                _sessionId = _providerClient.OpenPublicationSession(txtChannel.Text);

                txtStatus.Text = "Connected to ISBM with session id " + _sessionId;

                txtChannel.IsReadOnly = true;
                btnConnect.Visibility = Visibility.Collapsed;
                btnPost.Visibility = Visibility.Visible;
                btnDisconnect.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Arrow;
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Cursor = Cursors.Arrow;
            }
        }

        private void btnPost_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            try
            {
                Stopwatch sw = Stopwatch.StartNew();

                XmlDocument doc = new XmlDocument();
                doc.Load(txtFile.Text);

                string messageId = _providerClient.PostPublication(_sessionId, doc.DocumentElement, new List<string> { txtTopic.Text }, null);

                string fileName = Path.GetFileName(txtFile.Text);

                txtStatus.Text = string.Format("Posted {0} in {1} seconds with message id {2}", fileName, sw.Elapsed.TotalSeconds, messageId);
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Arrow;
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Cursor = Cursors.Arrow;
            }
        }

        private void btnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            try
            {
                _providerClient.ClosePublicationSession(_sessionId);
                _sessionId = null;
                txtStatus.Text = "Disconnected from the ISBM";
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Arrow;
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Cursor = Cursors.Arrow;

                txtChannel.IsReadOnly = false;
                btnConnect.Visibility = Visibility.Visible;
                btnPost.Visibility = Visibility.Collapsed;
                btnDisconnect.Visibility = Visibility.Collapsed;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Clean up any sessions that are still open
            if (_sessionId != null)
            {
                try
                {
                    _providerClient.ClosePublicationSession(_sessionId);
                }
                catch { }
            }
        }
    }
}
