using System.Runtime.CompilerServices;

namespace TestSUT
{
    public partial class Form1 : Form
    {
        public string textBoxContent { get; private set; } = String.Empty;
        public bool ButtonClicked { get;private set; } = false;

        public Form1()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            ButtonClicked = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBoxContent = textBox.Text;
        }
    }
}
