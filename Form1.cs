using System.Diagnostics;
using System.IO;

namespace Circle_2_Banana_Converter
{
	public partial class Form1 : Form {
		public Form1() {
			InitializeComponent();
		}


		private void textBox1_TextChanged(object sender, EventArgs e) {

		}

		private void browseButton_Click(object sender, EventArgs e) {
			OpenFileDialog openFileDialog = new OpenFileDialog();

			openFileDialog.DefaultExt = "osu";
			openFileDialog.Filter = "Beatmap Files (*.osu)|*.osu";

			openFileDialog.ShowDialog();

			if (openFileDialog.FileName.Length > 0) {
				foreach (string filename in openFileDialog.FileNames) {
					this.fileTextBox.Text = filename;
				}
			}
		}

		private void convertButton_Click(object sender, EventArgs e) {
			string path = fileTextBox.Text;

			if (File.Exists(path) == false) {
				MessageBox.Show("Files not exist", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			int pixelError = pixelErrorTrackBar.Value;
			bool randomDFS = randomDFSCheckBox.Checked;

			ReduceOption reduceOption = ReduceOption.None;

			if (noneButton.Checked == true) {
				reduceOption = ReduceOption.None;
			} else if (dummySpinnerButton.Checked == true) {
				reduceOption = ReduceOption.DummySpinner;
			}
			
			ConvertOption option = new ConvertOption(pixelError, randomDFS, reduceOption, -5000);

			Circle2BananaConverter.ConvertToBanana(path, option);

			MessageBox.Show("Complete");
		}

		private void pixelErrorTrackBar_Scroll(object sender, EventArgs e) {
			pixelErrorValue.Text = pixelErrorTrackBar.Value.ToString();
		}
	}
}
