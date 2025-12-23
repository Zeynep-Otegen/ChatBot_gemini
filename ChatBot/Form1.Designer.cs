namespace ChatBot
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.RichTextBox rtbHistory;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Label lblStatus;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.rtbHistory = new System.Windows.Forms.RichTextBox();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();

            this.rtbHistory.Location = new System.Drawing.Point(12, 12);
            this.rtbHistory.Size = new System.Drawing.Size(560, 280);

            this.txtMessage.Location = new System.Drawing.Point(12, 300);
            this.txtMessage.Size = new System.Drawing.Size(460, 22);
            this.txtMessage.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtMessage_KeyDown);

            this.btnSend.Location = new System.Drawing.Point(480, 298);
            this.btnSend.Size = new System.Drawing.Size(92, 26);
            this.btnSend.Text = "Gönder";
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);

            this.lblStatus.Location = new System.Drawing.Point(12, 330);
            this.lblStatus.Size = new System.Drawing.Size(300, 20);

            this.ClientSize = new System.Drawing.Size(584, 361);
            this.Controls.Add(this.rtbHistory);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.lblStatus);
            this.Text = "Gemini ChatBot";

            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
