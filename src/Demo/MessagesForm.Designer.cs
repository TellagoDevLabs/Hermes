namespace Demo
{
    partial class MessagesForm
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
            this.label2 = new System.Windows.Forms.Label();
            this.lbHeaders = new System.Windows.Forms.ListBox();
            this.btAddHeader = new System.Windows.Forms.Button();
            this.btRemoveHeader = new System.Windows.Forms.Button();
            this.cbContentType = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtContent = new System.Windows.Forms.TextBox();
            this.btCancel = new System.Windows.Forms.Button();
            this.btOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Others headers";
            // 
            // lbHeaders
            // 
            this.lbHeaders.FormattingEnabled = true;
            this.lbHeaders.Location = new System.Drawing.Point(12, 26);
            this.lbHeaders.Name = "lbHeaders";
            this.lbHeaders.Size = new System.Drawing.Size(371, 134);
            this.lbHeaders.TabIndex = 1;
            // 
            // btAddHeader
            // 
            this.btAddHeader.Location = new System.Drawing.Point(227, 166);
            this.btAddHeader.Name = "btAddHeader";
            this.btAddHeader.Size = new System.Drawing.Size(75, 23);
            this.btAddHeader.TabIndex = 2;
            this.btAddHeader.Text = "Add";
            this.btAddHeader.UseVisualStyleBackColor = true;
            this.btAddHeader.Click += new System.EventHandler(this.btAddHeader_Click);
            // 
            // btRemoveHeader
            // 
            this.btRemoveHeader.Location = new System.Drawing.Point(308, 166);
            this.btRemoveHeader.Name = "btRemoveHeader";
            this.btRemoveHeader.Size = new System.Drawing.Size(75, 23);
            this.btRemoveHeader.TabIndex = 3;
            this.btRemoveHeader.Text = "Remove";
            this.btRemoveHeader.UseVisualStyleBackColor = true;
            this.btRemoveHeader.Click += new System.EventHandler(this.btRemoveHeader_Click);
            // 
            // cbContentType
            // 
            this.cbContentType.FormattingEnabled = true;
            this.cbContentType.Location = new System.Drawing.Point(445, 6);
            this.cbContentType.Name = "cbContentType";
            this.cbContentType.Size = new System.Drawing.Size(306, 21);
            this.cbContentType.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(398, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Content:";
            // 
            // txtContent
            // 
            this.txtContent.Location = new System.Drawing.Point(401, 33);
            this.txtContent.Multiline = true;
            this.txtContent.Name = "txtContent";
            this.txtContent.Size = new System.Drawing.Size(350, 270);
            this.txtContent.TabIndex = 11;
            // 
            // btCancel
            // 
            this.btCancel.Location = new System.Drawing.Point(676, 309);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(75, 23);
            this.btCancel.TabIndex = 13;
            this.btCancel.Text = "Cancel";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // btOK
            // 
            this.btOK.Location = new System.Drawing.Point(595, 308);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(75, 23);
            this.btOK.TabIndex = 12;
            this.btOK.Text = "OK";
            this.btOK.UseVisualStyleBackColor = true;
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // MessagesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(763, 340);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btOK);
            this.Controls.Add(this.txtContent);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cbContentType);
            this.Controls.Add(this.btRemoveHeader);
            this.Controls.Add(this.btAddHeader);
            this.Controls.Add(this.lbHeaders);
            this.Controls.Add(this.label2);
            this.Name = "MessagesForm";
            this.Text = "Message";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox lbHeaders;
        private System.Windows.Forms.Button btAddHeader;
        private System.Windows.Forms.Button btRemoveHeader;
        private System.Windows.Forms.ComboBox cbContentType;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtContent;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.Button btOK;
    }
}