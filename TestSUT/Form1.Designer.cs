namespace TestSUT
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
        private void InitializeComponent()
        {
            button1 = new Button();
            label1 = new Label();
            textBox = new TextBox();
            label2 = new Label();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(150, 414);
            button1.Name = "button1";
            button1.Size = new Size(185, 45);
            button1.TabIndex = 0;
            button1.Text = "Button";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(80, 222);
            label1.Name = "label1";
            label1.Size = new Size(209, 25);
            label1.TabIndex = 1;
            label1.Text = "Label                    next To\r\n";
            // 
            // textBox
            // 
            textBox.AcceptsTab = true;
            textBox.Location = new Point(662, 187);
            textBox.Name = "textBox";
            textBox.Size = new Size(150, 31);
            textBox.TabIndex = 2;
            textBox.TextChanged += textBox1_TextChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(676, 157);
            label2.Name = "label2";
            label2.Size = new Size(54, 25);
            label2.TabIndex = 3;
            label2.Text = "Input";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1258, 686);
            Controls.Add(label2);
            Controls.Add(textBox);
            Controls.Add(label1);
            Controls.Add(button1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private Label label1;
        private TextBox textBox;
        private Label label2;
    }
}
