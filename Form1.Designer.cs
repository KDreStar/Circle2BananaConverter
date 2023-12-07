namespace Circle_2_Banana_Converter
{
	partial class Form1
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
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
			openCheckBox = new CheckBox();
			reduceToOverlapCheckBox = new CheckBox();
			((System.ComponentModel.ISupportInitialize)pixelErrorTrackBar).BeginInit();
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
			randomDFSCheckBox.Location = new Point(12, 91);
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
			// openCheckBox
			// 
			openCheckBox.AutoSize = true;
			openCheckBox.Checked = true;
			openCheckBox.CheckState = CheckState.Checked;
			openCheckBox.Location = new Point(428, 91);
			openCheckBox.Name = "openCheckBox";
			openCheckBox.Size = new Size(109, 19);
			openCheckBox.TabIndex = 8;
			openCheckBox.Text = "Open result.txt ";
			openCheckBox.UseVisualStyleBackColor = true;
			// 
			// reduceToOverlapCheckBox
			// 
			reduceToOverlapCheckBox.AutoSize = true;
			reduceToOverlapCheckBox.Location = new Point(195, 91);
			reduceToOverlapCheckBox.Name = "reduceToOverlapCheckBox";
			reduceToOverlapCheckBox.Size = new Size(171, 19);
			reduceToOverlapCheckBox.TabIndex = 9;
			reduceToOverlapCheckBox.Text = "Reduce to overlap bananas";
			reduceToOverlapCheckBox.UseVisualStyleBackColor = true;
			// 
			// Form1
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			AutoSizeMode = AutoSizeMode.GrowAndShrink;
			ClientSize = new Size(549, 122);
			Controls.Add(reduceToOverlapCheckBox);
			Controls.Add(openCheckBox);
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
		private CheckBox reduceToOverlapCheckBox;
	}
}
