using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Revit.Addon.RevitDBLink.CS
{
	public class ProgressForm : Form
	{
		public static System.DateTime LastTime = System.DateTime.Now;

		private IContainer components;

		private ProgressBar progressBar1;

		private Label label1;

		public int ProgressMaximum
		{
			set
			{
				this.progressBar1.Maximum = value;
			}
		}

		public int ProgressStep
		{
			set
			{
				this.progressBar1.Step = value;
			}
		}

		public string ProgressBarLableTitle
		{
			set
			{
				this.label1.Text = value;
			}
		}

		public int ProgressBarValue
		{
			get
			{
				return this.progressBar1.Value;
			}
		}

		public ProgressForm()
		{
			this.InitializeComponent();
		}

		public void PerformStep()
		{
			this.progressBar1.PerformStep();
			if (Command.ConfigFile.DebugProgressBar)
			{
				System.DateTime now = System.DateTime.Now;
				System.TimeSpan timeSpan = now.Subtract(ProgressForm.LastTime);
				ProgressForm.LastTime = now;
				double totalMilliseconds = timeSpan.TotalMilliseconds;
				if (totalMilliseconds > 20.0)
				{
					Log.WriteLine("[{0}] >>>>>!!!!!!Perform Step: {1}", new object[]
					{
						totalMilliseconds,
						this.progressBar1.Value
					});
					return;
				}
				Log.WriteLine("[{0}] >>>>>Perform Step: {1}", new object[]
				{
					totalMilliseconds,
					this.progressBar1.Value
				});
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ProgressForm));
			this.progressBar1 = new ProgressBar();
			this.label1 = new Label();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.progressBar1, "progressBar1");
			this.progressBar1.Name = "progressBar1";
			componentResourceManager.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Dpi;
			base.ControlBox = false;
			base.Controls.Add(this.label1);
			base.Controls.Add(this.progressBar1);
			base.FormBorderStyle = FormBorderStyle.FixedDialog;
			base.KeyPreview = true;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ProgressForm";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
