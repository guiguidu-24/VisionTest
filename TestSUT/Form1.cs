namespace TestSUT
{
    public partial class Form1 : Form
    {
        public static string textBoxContent { get; private set; } = String.Empty;
        public static bool ButtonClicked { get;private set; } = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

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
