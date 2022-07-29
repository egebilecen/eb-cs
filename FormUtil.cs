using System;
using System.Windows.Forms;

namespace EB_Utility
{
    public static class FormUtil
    {
        private static Timer labelTimer = new Timer();
        
        public static string ShowOpenFileDialog(string title, string filter="")
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title  = title;
            fileDialog.Filter = filter;

            DialogResult dialogResult = fileDialog.ShowDialog();

            if(dialogResult == DialogResult.Cancel)
                throw new CustomException.Abort("User did close the dialog.");

            if(dialogResult == DialogResult.OK
            && fileDialog.FileName != string.Empty)
                return fileDialog.FileName;

            return string.Empty;
        }

        public static string ShowSaveFileDialog(string title, string filter="")
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Title  = title;
            fileDialog.Filter = filter;

            DialogResult dialogResult = fileDialog.ShowDialog();

            if(dialogResult == DialogResult.Cancel)
                throw new CustomException.Abort("User did close the dialog.");

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
