namespace Circle_2_Banana_Converter {
	partial class Form1 {
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			beatmapPath = new Label();
			fileTextBox = new TextBox();
			browseButton = new Button();
			convertButton = new Button();
			randomDFSCheckBox = new CheckBox();
			pixelErrorTrackBar = new TrackBar();
			allowedPixelError = new Label();
			pixelErrorValue = new Label();
			groupBox1 = new GroupBox();
			dummySpinnerButton = new RadioButton();
			noneButton = new RadioButton();
			((System.ComponentModel.ISupportInitialize)pixelErrorTrackBar).BeginInit();
			groupBox1.SuspendLayout();
			SuspendLayout();
			// 
			// beatmapPath
			// 
			beatmapPath.AutoSize = true;
			beatmapPath.Font = new Font("맑은 고딕", 10F, FontStyle.Regular, GraphicsUnit.Point);
			beatmapPath.Location = new Point(12, 11);
			beatmapPath.Name = "beatmapPath";
			beatmapPath.Size = new Size(96, 19);
			beatmapPath.TabIndex = 0;
			beatmapPath.Text = "Beatmap path";
			// 
			// fileTextBox
			// 
			fileTextBox.BorderStyle = BorderStyle.FixedSingle;
			fileTextBox.Font = new Font("맑은 고딕", 10F, FontStyle.Regular, GraphicsUnit.Point);
			fileTextBox.Location = new Point(114, 9);
			fileTextBox.Name = "fileTextBox";
			fileTextBox.Size = new Size(332, 25);
			fileTextBox.TabIndex = 1;
			// 
			// browseButton
			// 
			browseButton.Location = new Point(452, 9);
			browseButton.Name = "browseButton";
			browseButton.Size = new Size(85, 25);
			browseButton.TabIndex = 2;
			browseButton.Text = "Browse";
			browseButton.UseVisualStyleBackColor = true;
			browseButton.Click += browseButton_Click;
			// 
			// convertButton
			// 
			convertButton.Location = new Point(452, 40);
			convertButton.Name = "convertButton";
			convertButton.Size = new Size(85, 27);
			convertButton.TabIndex = 3;
			convertButton.Text = "Convert";
			convertButton.UseVisualStyleBackColor = true;
			convertButton.Click += convertButton_Click;
			// 
			// randomDFSCheckBox
			// 
			randomDFSCheckBox.AutoSize = true;
			randomDFSCheckBox.Location = new Point(227, 100);
			randomDFSCheckBox.Name = "randomDFSCheckBox";
			randomDFSCheckBox.Size = new Size(177, 19);
			randomDFSCheckBox.TabIndex = 4;
			randomDFSCheckBox.Text = "Find another random routes";
			randomDFSCheckBox.UseVisualStyleBackColor = true;
			// 
			// pixelErrorTrackBar
			// 
			pixelErrorTrackBar.Location = new Point(143, 45);
			pixelErrorTrackBar.Name = "pixelErrorTrackBar";
			pixelErrorTrackBar.Size = new Size(261, 45);
			pixelErrorTrackBar.TabIndex = 5;
			pixelErrorTrackBar.Value = 5;
			pixelErrorTrackBar.Scroll += pixelErrorTrackBar_Scroll;
			// 
			// allowedPixelError
			// 
			allowedPixelError.AutoSize = true;
			allowedPixelError.Font = new Font("맑은 고딕", 10F, FontStyle.Regular, GraphicsUnit.Point);
			allowedPixelError.Location = new Point(12, 45);
			allowedPixelError.Name = "allowedPixelError";
			allowedPixelError.Size = new Size(125, 19);
			allowedPixelError.TabIndex = 6;
			allowedPixelError.Text = "Allowed pixel error";
			// 
			// pixelErrorValue
			// 
			pixelErrorValue.AutoSize = true;
			pixelErrorValue.Location = new Point(417, 49);
			pixelErrorValue.Name = "pixelErrorValue";
			pixelErrorValue.Size = new Size(14, 15);
			pixelErrorValue.TabIndex = 7;
			pixelErrorValue.Text = "5";
			// 
			// groupBox1
			// 
			groupBox1.Controls.Add(dummySpinnerButton);
			groupBox1.Controls.Add(noneButton);
			groupBox1.Location = new Point(12, 77);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new Size(200, 71);
			groupBox1.TabIndex = 9;
			groupBox1.TabStop = false;
			groupBox1.Text = "Option to reduce lag";
			// 
			// dummySpinnerButton
			// 
			dummySpinnerButton.AutoSize = true;
			dummySpinnerButton.Location = new Point(6, 47);
			dummySpinnerButton.Name = "dummySpinnerButton";
			dummySpinnerButton.Size = new Size(153, 19);
			dummySpinnerButton.TabIndex = 1;
			dummySpinnerButton.Text = "Create dummy spinners";
			dummySpinnerButton.UseVisualStyleBackColor = true;
			// 
			// noneButton
			// 
			noneButton.AutoSize = true;
			noneButton.Checked = true;
			noneButton.Location = new Point(6, 22);
			noneButton.Name = "noneButton";
			noneButton.Size = new Size(54, 19);
			noneButton.TabIndex = 0;
			noneButton.TabStop = true;
			noneButton.Text = "None";
			noneButton.UseVisualStyleBackColor = true;
			// 
			// Form1
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			AutoSizeMode = AutoSizeMode.GrowAndShrink;
			ClientSize = new Size(549, 158);
			Controls.Add(groupBox1);
			Controls.Add(pixelErrorValue);
			Controls.Add(allowedPixelError);
			Controls.Add(pixelErrorTrackBar);
			Controls.Add(randomDFSCheckBox);
			Controls.Add(convertButton);
			Controls.Add(browseButton);
			Controls.Add(fileTextBox);
			Controls.Add(beatmapPath);
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "Form1";
			Text = "Circle 2 Banana Converter";
			((System.ComponentModel.ISupportInitialize)pixelErrorTrackBar).EndInit();
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private Label beatmapPath;
		private TextBox fileTextBox;
		private Button browseButton;
		private Button convertButton;
		private CheckBox randomDFSCheckBox;
		private TrackBar pixelErrorTrackBar;
		private Label allowedPixelError;
		private Label pixelErrorValue;
		private CheckBox openCheckBox;
		private GroupBox groupBox1;
		private RadioButton dummySpinnerButton;
		private RadioButton noneButton;
	}
}