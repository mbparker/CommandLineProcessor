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
            this.textBox_CommandLine = new System.Windows.Forms.TextBox();
            this.textBox_CommandHistory = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBox_CommandLine
            // 
            this.textBox_CommandLine.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.textBox_CommandLine.Location = new System.Drawing.Point(0, 532);
            this.textBox_CommandLine.Name = "textBox_CommandLine";
            this.textBox_CommandLine.Size = new System.Drawing.Size(1067, 22);
            this.textBox_CommandLine.TabIndex = 2;
            this.textBox_CommandLine.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_CommandLine_KeyDown);
            // 
            // textBox_CommandHistory
            // 
            this.textBox_CommandHistory.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.textBox_CommandHistory.Location = new System.Drawing.Point(0, 384);
            this.textBox_CommandHistory.Multiline = true;
            this.textBox_CommandHistory.Name = "textBox_CommandHistory";
            this.textBox_CommandHistory.ReadOnly = true;
            this.textBox_CommandHistory.Size = new System.Drawing.Size(1067, 148);
            this.textBox_CommandHistory.TabIndex = 3;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1067, 554);
            this.Controls.Add(this.textBox_CommandHistory);
            this.Controls.Add(this.textBox_CommandLine);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FormMain";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.Shown += new System.EventHandler(this.FormMain_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_CommandLine;
        private System.Windows.Forms.TextBox textBox_CommandHistory;
    }
}

