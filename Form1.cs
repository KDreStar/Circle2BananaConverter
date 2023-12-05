using System.Diagnostics;
using System.IO;

namespace Circle_2_Banana_Converter
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}


		private void textBox1_TextChanged(object sender, EventArgs e)
		{

		}

		private void browseButton_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();

			openFileDialog.DefaultExt = "osu";
			openFileDialog.Filter = "Beatmap Files (*.osu)|*.osu";

			openFileDialog.ShowDialog();

			if (openFileDialog.FileName.Length > 0)
			{
				foreach (string filename in openFileDialog.FileNames)
				{
					this.fileTextBox.Text = filename;
				}
			}
		}

		private void convertButton_Click(object sender, EventArgs e)
		{
			string fileName = fileTextBox.Text;

			if (File.Exists(fileName) == false)
			{
				MessageBox.Show("Files not exist", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			int pixelError = pixelErrorTrackBar.Value;
			bool randomDFS = randomDFSCheckBox.Checked;
            StreamWriter sw = new StreamWriter("result.txt");
            sw.Write(Circle2BananaConverter.ConvertToBanana(fileName, pixelError, randomDFS));
            sw.Close();

			MessageBox.Show("Complete");

			if (openCheckBox.Checked)
			{
                System.Diagnostics.Process.Start(new ProcessStartInfo
                {
                    FileName = "result.txt",
                    UseShellExecute = true
                });
            }

        }

		private void pixelErrorTrackBar_Scroll(object sender, EventArgs e)
		{
			pixelErrorValue.Text = pixelErrorTrackBar.Value.ToString();
		}
	}
}
