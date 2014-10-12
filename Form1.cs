/*

(c) 2004, Marc Clifton
All Rights Reserved

Redistribution and use in source and binary forms, with or without modification,
are permitted provided that the following conditions are met:

Redistributions of source code must retain the above copyright notice, 
this list of conditions and the following disclaimer. 

Redistributions in binary form must reproduce the above copyright notice, 
this list of conditions and the following disclaimer in the documentation 
and/or other materials provided with the distribution. 

Neither the name of Marc Clifton nor the names of its contributors may 
be used to endorse or promote products derived from this software without 
specific prior written permission. 

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL 
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

*/

using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting;
using System.Threading;
using System.Windows.Forms;

namespace Drop2Phone
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		protected int _lastX = 0;
		protected int _lastY = 0;
		protected string _lastFilename = String.Empty;
        protected bool _validData = false;
		protected DragDropEffects _effect;

        protected const string DEST_DIR = "/storage/external_SD/Downloads";

		private System.Windows.Forms.PictureBox pb;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1()
        {
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
        {
			if (disposing)
            {
				if (components != null)
                {
					components.Dispose();
				}
			}

			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
        {
			this.pb = new System.Windows.Forms.PictureBox();
			this.SuspendLayout();
			// 
			// pb
			// 
			//			this.pb.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pb.Location = new System.Drawing.Point(0, 0);
			this.pb.Name = "pb";
			this.pb.Size = new System.Drawing.Size(292, 266);
			this.pb.TabIndex = 0;
			this.pb.TabStop = false;
			this.pb.SizeMode=PictureBoxSizeMode.StretchImage;
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.OnDragEnter);
			this.DragLeave += new System.EventHandler(this.OnDragLeave);
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.OnDragDrop);
			this.DragOver += new System.Windows.Forms.DragEventHandler(this.OnDragOver);
			this.AllowDrop = true;

			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.Controls.Add(this.pb);
			this.Name = "Form1";
			this.Text = "Drop 2 Phone";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
        {
			Application.Run(new Form1());
		}

		private void OnDragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
			Debug.WriteLine("OnDragDrop");
			if (_validData)
			{
                // TODO: Handle file drop
                Debug.WriteLine("Dropped file");

                Process push = new Process();
                push.StartInfo.FileName = "adb";
                push.StartInfo.Arguments = "push \"" + _lastFilename + "\" " + DEST_DIR;
                push.StartInfo.UseShellExecute = false;

                try
                {
                    push.Start();
                    push.WaitForExit();
                }
                catch (InvalidOperationException ex)
                {
                    Debug.WriteLine(ex);
                }
                catch (Win32Exception ex)
                {
                    Debug.WriteLine(ex);
                }
			}
		}

		private void OnDragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
			Debug.WriteLine("OnDragEnter");
			string filename;

			_validData=GetFilename(out filename, e);
			if (_validData)
            {
                _lastFilename = filename;
				e.Effect=DragDropEffects.Copy;
			}
            else
            {
				e.Effect=DragDropEffects.None;
			}
		}

		private void OnDragLeave(object sender, System.EventArgs e)
        {
			Debug.WriteLine("OnDragLeave");
		}

		private void OnDragOver(object sender, System.Windows.Forms.DragEventArgs e)
        {
			Debug.WriteLine("OnDragOver");
		}
																			   
		protected bool GetFilename(out string filename, DragEventArgs e)
        {
			bool ret = false;

			filename = String.Empty;

			if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
            {
				Array data = ((IDataObject) e.Data).GetData(DataFormats.FileDrop) as Array;
				if (data != null)
                {
					if ((data.Length == 1) && (data.GetValue(0) is String))
                    {
						filename = ((string[]) data)[0];
                        return true;
					}
				}
			}

			return ret;
		}
	}
}
