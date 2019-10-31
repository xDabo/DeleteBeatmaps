using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System.Windows.Threading;

namespace DeleteBeatmaps
{
    public partial class MainWindow : Window
    {
        string folder;
        StreamReader reader;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void FileDialogButton_Click(object sender, RoutedEventArgs e)
        {
            String currentDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            currentDirectory = currentDirectory + @"\osu!";

            var dlg = new CommonOpenFileDialog();

            dlg.Title = "Beatmaps";
            dlg.IsFolderPicker = true;
            dlg.InitialDirectory = currentDirectory;
            dlg.AddToMostRecentlyUsedList = false;
            dlg.AllowNonFileSystemItems = false;
            dlg.DefaultDirectory = currentDirectory;
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.ShowPlacesList = true;

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                folder = dlg.FileName;
                deleteButton.IsEnabled = true;
                standardCheckBox.IsEnabled = true;
                taikoCheckBox.IsEnabled = true;
                ctbCheckBox.IsEnabled = true;
                maniaCheckBox.IsEnabled = true;
                pathTextBox.Text = dlg.FileName;
            }
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string[] filesindirectory = Directory.GetDirectories(folder);
                statusProgressBar.Maximum = filesindirectory.Length;
                foreach (string subdir in filesindirectory)
                {
                    statusProgressBar.Dispatcher.Invoke(() => statusProgressBar.Value = Array.IndexOf(filesindirectory, subdir) + 1, DispatcherPriority.Background);
                    var modes = new List<int>();
                    var filepaths = new List<string>();
                    foreach (string fileName in Directory.GetFiles(subdir))
                    {
                        if (Path.GetExtension(fileName) == ".osu")
                        {
                            reader = null;

                            reader = File.OpenText(fileName);
                            string line = reader.ReadLine();

                            while (line != null)
                            {
                                if (line.Contains("Mode:"))
                                {
                                    string mode = line.Substring(line.Length - 1, 1);
                                    modes.Add((Convert.ToInt32(mode)));
                                    filepaths.Add(mode + fileName);
                                    break;
                                }
                                line = reader.ReadLine();
                            }
                            reader.Close();
                        }
                    }
                    if (standardCheckBox.IsChecked == true)
                    {
                        if (modes.Contains(0) == true && modes.Contains(1) == false && modes.Contains(2) == false && modes.Contains(3) == false)
                        {
                            Directory.Delete(subdir, true);
                        }
                        else if ((modes.Contains(0) == true) && (modes.Contains(1) == true || modes.Contains(2) == true || modes.Contains(3) == true))
                        {
                            foreach (string file in filepaths)
                            {
                                if (file.Substring(0, 1) == "0")
                                {
                                    File.Delete(file.Substring(1));
                                }
                            }
                        }
                    }
                    if (taikoCheckBox.IsChecked == true)
                    {
                        if (modes.Contains(0) == false && modes.Contains(1) == true && modes.Contains(2) == false && modes.Contains(3) == false)
                        {
                            Directory.Delete(subdir, true);
                        }
                        else if ((modes.Contains(1) == true) && (modes.Contains(0) == true || modes.Contains(2) == true || modes.Contains(3) == true))
                        {
                            foreach (string file in filepaths)
                            {
                                if (file.Substring(0, 1) == "1")
                                {
                                    File.Delete(file.Substring(1));
                                }
                            }
                        }
                    }
                    if (ctbCheckBox.IsChecked == true)
                    {
                        if (modes.Contains(0) == false && modes.Contains(1) == false && modes.Contains(2) == true && modes.Contains(3) == false)
                        {
                            Directory.Delete(subdir, true);
                        }
                        else if ((modes.Contains(2) == true) && (modes.Contains(0) == true || modes.Contains(1) == true || modes.Contains(3) == true))
                        {
                            foreach (string file in filepaths)
                            {
                                if (file.Substring(0, 1) == "2")
                                {
                                    File.Delete(file.Substring(1));
                                }
                            }
                        }
                    }
                    if (maniaCheckBox.IsChecked == true)
                    {
                        if (modes.Contains(0) == false && modes.Contains(1) == false && modes.Contains(2) == false && modes.Contains(3) == true)
                        {
                            Directory.Delete(subdir, true);
                        }
                        else if ((modes.Contains(3) == true) && (modes.Contains(0) == true || modes.Contains(1) == true || modes.Contains(2) == true))
                        {
                            foreach (string file in filepaths)
                            {
                                if (file.Substring(0, 1) == "3")
                                {
                                    File.Delete(file.Substring(1));
                                }
                            }
                        }
                    }
                }
                MessageBox.Show("All selected files have been successfully deleted!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if(reader != null)
                {
                    reader.Close();
                }
            }
        }
    }
}