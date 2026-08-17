namespace desktopPet
{
    partial class mainForm
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
        private void InitializeComponent()
        {
            textBox1 = new TextBox();
            mainpanel = new Panel();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.BorderStyle = BorderStyle.FixedSingle;
            textBox1.Font = new Font("Cascadia Mono", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            textBox1.Location = new Point(1, 164);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(158, 21);
            textBox1.TabIndex = 1;
            // 
            // mainpanel
            // 
            mainpanel.Location = new Point(1, 1);
            mainpanel.Name = "mainpanel";
            mainpanel.Size = new Size(158, 155);
            mainpanel.TabIndex = 2;
            // 
            // mainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaption;
            ClientSize = new Size(161, 197);
            Controls.Add(mainpanel);
            Controls.Add(textBox1);
            Font = new Font("Cascadia Mono", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.None;
            Name = "mainForm";
            Text = "Form1";
            TransparencyKey = SystemColors.ActiveCaption;
            Load += mainForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TextBox textBox1;
        private Panel mainpanel;
    }
}
