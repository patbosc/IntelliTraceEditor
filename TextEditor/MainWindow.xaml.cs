using System;
using System.Collections.Generic;
using System.Linq;
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
using Microsoft.Win32;
using System.IO;
using Path = System.IO.Path;
using System.Windows.Threading;

namespace TextEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FileSystemWatcher fileWatcher = null;
        private string fullPath = String.Empty;
        private bool hasPendingChanges = false;
        public const string PENDING_CHANGES_MESSAGE = "The file has been changed outside this text editor, but you have unsaved changes.Be warned that if you save right now the other users' changes will be lost.";
        public const string PENDING_CHANGES_CAPTION = "File changed outside this editor";

        public MainWindow()
        {
            InitializeComponent();
            this.txtFileContents.Focus();
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents (.txt)|*.txt";

            if (dlg.ShowDialog().GetValueOrDefault())
            {
                this.OpenFile(dlg.FileName);
            }
        }

        private void OpenFile(string fullPath)
        {
            // Clean up in case this is not the first time
            this.CleanUp();
            // Initialize textbox
            this.txtFileContents.Text = String.Empty;
            this.txtFileContents.IsReadOnly = false;
            this.fullPath = fullPath;
            // Initialize watcher
            this.InitializeFileSystemWatcher(fullPath);
            // Populate textbox
            this.txtFileContents.Text = this.ReadFile(fullPath);
            this.txtFileContents.Focus();
        }

        private void InitializeFileSystemWatcher(string fullPath)
        {
            // If the watcher was already active, disable it
            if (this.fileWatcher != null)
            {
                this.fileWatcher.EnableRaisingEvents = false;
                this.fileWatcher.Changed -= fileWatcher_Changed;
                this.fileWatcher.Dispose();
                this.fileWatcher = null;
            }

            // Enable the watcher
            var directoryPath = Path.GetDirectoryName(fullPath);
            this.fileWatcher = new FileSystemWatcher(directoryPath, "*.txt");
            this.fileWatcher.Changed += fileWatcher_Changed;
            this.fileWatcher.EnableRaisingEvents = true;
        }

        private void CleanUp()
        {
            if (this.fileWatcher != null)
            {
                this.fileWatcher.EnableRaisingEvents = false;
                this.fileWatcher.Changed -= fileWatcher_Changed;
                this.fileWatcher = null;
            }
        }

        void fileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            // If the file we currently have open was changed
            if (e.ChangeType == WatcherChangeTypes.Changed &&
                String.Compare(e.FullPath, this.fullPath) == 0)
            {
                Dispatcher.BeginInvoke(new Action(delegate () { this.OnFileChanged(); }));
            }
        }

        private void OnFileChanged()
        {
            // If the file has pending changes, warn the user that saving would 
            // result in work being lost, else just reload the file to pick up the changes
            if (this.hasPendingChanges)
            {
                MessageBox.Show(this, PENDING_CHANGES_MESSAGE, PENDING_CHANGES_CAPTION, 
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                // BUG:By commenting this out the file will get reloaded after it is saved
                //if (!this.ShouldReloadFile(this.fullPath)) { return; }

                this.OpenFile(this.fullPath);
            }
        }

        private bool ShouldReloadFile(string path)
        {
            // TODO: Figure out a better way of knowing if the file was changed
            // by us or somebody else. This is good enough for now because if the file
            // was changed and the editor has no pending changes, then we can deduce
            // it was edited by somebody else outside of the editor
            return this.FileContainsChanges(this.fullPath);
        }

        private string ReadFile(string fullPath)
        {
            try
            {
                using (var reader = File.OpenText(fullPath))
                {
                   return reader.ReadToEnd();
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message, "Could not read the selected file", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return "File contents could not be read";
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.CleanUp();
            this.Close();
        }

        private void btnSaveFile_Click(object sender, RoutedEventArgs e)
        {
            this.hasPendingChanges = false;
            
            try
            {
                using (var writer = File.OpenWrite(this.fullPath))
                {
                    var contents = this.GetBytes(this.txtFileContents.Text);
                    writer.Write(contents, 0, contents.Length);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unable to save file", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            this.txtFileContents.Focus();
        }

        private byte[] GetBytes(string value)
        {
            return System.Text.Encoding.UTF8.GetBytes(value);
        }

        private void txtFileContents_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.hasPendingChanges = true;
        }

        private bool FileContainsChanges(string fullPath)
        {
            return String.Compare(this.ReadFile(fullPath), this.txtFileContents.Text) != 0;
        }
    }
}
