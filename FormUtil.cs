using System;
using System.Windows.Forms;

namespace EB_Utility
{
    public static class FormUtil
    {
        private static Timer labelTimer = new Timer();
        
        // Filter: Text Files|*.txt|Image Files|*.jpg;*.jpeg;*.png;*.gif;*.tif
        public static string ShowOpenFileDialog(string title, string filter="", string initialDirectory="", string initialFileName="")
        {
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                Title    = title,
                Filter   = filter,
                FileName = initialFileName
            };
            
            if(!string.IsNullOrEmpty(initialDirectory))
                fileDialog.InitialDirectory = initialDirectory;

            DialogResult dialogResult = fileDialog.ShowDialog();

            if(dialogResult == DialogResult.Cancel)
                return null;

            if(dialogResult == DialogResult.OK
            && fileDialog.FileName != string.Empty)
                return fileDialog.FileName;

            return string.Empty;
        }

        // Filter: Text Files|*.txt|Image Files|*.jpg;*.jpeg;*.png;*.gif;*.tif
        public static string ShowSaveFileDialog(string title, string filter="", string initialDirectory="", string initialFileName="")
        {
            SaveFileDialog fileDialog = new SaveFileDialog
            {
                Title    = title,
                Filter   = filter,
                FileName = initialFileName
            };

            if(!string.IsNullOrEmpty(initialDirectory))
                fileDialog.InitialDirectory = initialDirectory;

            DialogResult dialogResult = fileDialog.ShowDialog();

            if(dialogResult == DialogResult.Cancel)
                return null;

            if(dialogResult == DialogResult.OK
            && fileDialog.FileName != string.Empty)
                return fileDialog.FileName;

            return string.Empty;
        }

        public static void UpdateProgressBar(ProgressBar progressBar, int value)
        {
            bool maxUpdated = false;

            if(value + 1 > progressBar.Maximum)
            {
                progressBar.Maximum++;
                maxUpdated = true;
            }

            progressBar.Value = value + 1;
            progressBar.Value--;

            if(maxUpdated) progressBar.Maximum--;
        }

        public static void UpdateProgressBarInvoke(ProgressBar progressBar, int value)
        {
            bool maxUpdated = false;

            if(value + 1 > progressBar.Maximum)
            {
                progressBar.Invoke(new Action(() => {
                    progressBar.Maximum++;
                }));
                maxUpdated = true;
            }

            progressBar.Invoke(new Action(() => {
                progressBar.Value = value + 1;
            }));
            progressBar.Invoke(new Action(() => {
                progressBar.Value--;
            }));

            if(maxUpdated)
            { 
                progressBar.Invoke(new Action(() => {
                    progressBar.Maximum--;
                }));
            }
        }

        public static void ResetProgressBar(ProgressBar progressBar, int maxValue)
        {
            progressBar.Value   = 0;
            progressBar.Maximum = maxValue;
        }

        public static void ResetProgressBarInvoke(ProgressBar progressBar, int maxValue)
        {
            progressBar.Invoke(new Action(() => {
                progressBar.Value = 0;
            }));
            progressBar.Invoke(new Action(() => {
                progressBar.Maximum = maxValue;
            }));
        }
        
        public static void DisplayHidingText(Label label, string text, int durationMS=0)
        {
            label.Text = text;
            label.Show();

            if(durationMS != 0)
            {
                labelTimer.Stop();

                labelTimer.Interval = durationMS;
                labelTimer.Tick += (s, e) => { 
                    label.Hide();
                    labelTimer.Stop();
                };

                labelTimer.Start();
            }
        }
    }
}
