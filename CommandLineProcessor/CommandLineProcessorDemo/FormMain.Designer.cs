namespace CommandLineProcessorDemo
{
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textEdit_CommandLine = new DevExpress.XtraEditors.TextEdit();
            this.memoEdit_CommandHistory = new DevExpress.XtraEditors.MemoEdit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit_CommandLine.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.memoEdit_CommandHistory.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // textEdit_CommandLine
            // 
            this.textEdit_CommandLine.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.textEdit_CommandLine.Location = new System.Drawing.Point(0, 430);
            this.textEdit_CommandLine.Name = "textEdit_CommandLine";
            this.textEdit_CommandLine.Size = new System.Drawing.Size(800, 20);
            this.textEdit_CommandLine.TabIndex = 0;
            this.textEdit_CommandLine.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textEdit_CommandLine_KeyUp);
            // 
            // memoEdit_CommandHistory
            // 
            this.memoEdit_CommandHistory.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.memoEdit_CommandHistory.Location = new System.Drawing.Point(0, 334);
            this.memoEdit_CommandHistory.Name = "memoEdit_CommandHistory";
            this.memoEdit_CommandHistory.Properties.ReadOnly = true;
            this.memoEdit_CommandHistory.Size = new System.Drawing.Size(800, 96);
            this.memoEdit_CommandHistory.TabIndex = 1;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.memoEdit_CommandHistory);
            this.Controls.Add(this.textEdit_CommandLine);
            this.Name = "FormMain";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.FormMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.textEdit_CommandLine.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.memoEdit_CommandHistory.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.TextEdit textEdit_CommandLine;
        private DevExpress.XtraEditors.MemoEdit memoEdit_CommandHistory;
    }
}

