using System;
using System.Drawing;
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

        // If canOverflow is false, then this function will throw exception if you try to assign
        // a value greater than progress bar's maximum value.
        public static void UpdateProgressBar(ProgressBar progressBar, int value, bool canOverflow=true)
        {
            if(canOverflow
            && value - 1 >= progressBar.Maximum)
            {
                progressBar.Maximum++;
                progressBar.Value = progressBar.Maximum;
                progressBar.Value--;
                progressBar.Maximum--;
                return;
            }

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

        // If canOverflow is false, then this function will throw exception if you try to assign
        // a value greater than progress bar's maximum value.
        public static void UpdateProgressBarInvoke(ProgressBar progressBar, int value, bool canOverflow=true)
        {
            if(canOverflow
            && value - 1 >= progressBar.Maximum)
            {
                progressBar.Invoke(new Action(() => {
                    progressBar.Maximum++;
                    progressBar.Value = progressBar.Maximum;
                    progressBar.Value--;
                    progressBar.Maximum--;
                }));
                return;
            }

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

        public static void ResetProgressBar(ProgressBar progressBar, int? maxValue=null)
        {
            progressBar.Value   = 0;
            
            if(maxValue != null)
                progressBar.Maximum = (int)maxValue;
        }

        public static void ResetProgressBarInvoke(ProgressBar progressBar, int? maxValue=null)
        {
            progressBar.Invoke(new Action(() => {
                progressBar.Value = 0;
            }));

            if(maxValue != null)
                progressBar.Invoke(new Action(() => {
                    progressBar.Maximum = (int)maxValue;
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

        private static void _DrawMenuStripBorder(PictureBox pbBorder)
        {
            Bitmap bmp = new Bitmap(pbBorder.Width, pbBorder.Height);
            using(var g = Graphics.FromImage(bmp))
            {
                g.DrawLine(new Pen(Color.Gray, 5), 0, pbBorder.Height, pbBorder.Width, pbBorder.Height);
            }
            pbBorder.Image = bmp;
        }

        public static void DrawMenuStripBorder(Form form, MenuStrip menuStrip, PictureBox pbBorder)
        {
            pbBorder.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pbBorder.Width  = form.Size.Width;
            pbBorder.Height = 1;
            pbBorder.Location = new Point(0, menuStrip.Height - 1);

            _DrawMenuStripBorder(pbBorder);

            pbBorder.SizeChanged += (s, e) => { 
                _DrawMenuStripBorder(pbBorder);
            };
        }

        public static void InvokeControl(Control control, Action<Control> action)
        {
            control.Invoke(new Action(() => action(control)));
        }
    }
}
