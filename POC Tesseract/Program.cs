using System.Threading.Tasks;
using WindowsInput;
using WindowsInput.Events;

namespace POC_Tesseract
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Simulate.Events().ClickChord(KeyCode.LWin, KeyCode.R).Wait(1000).Invoke();
            Simulate.Events().Click("notepad").Wait(1000);
        }

        public static async Task RunNotepad()
        {
            await Simulate.Events()
                //Hold Windows Key+R
                .ClickChord(KeyCode.LWin, KeyCode.R).Wait(1000)

                //Type "notepad"
                .Click("notepad").Wait(1000)

                //Press Enter
                .Click(KeyCode.Return).Wait(1000)

                //Type out our message.
                .Click("These are your orders if you choose to accept them...")
                .Click("This message will self destruct in 5 seconds.").Wait(5000)

                //Hold Alt+F4
                .ClickChord(KeyCode.Alt, KeyCode.F4).Wait(1000)

                //Press Tab then Enter.
                .Click(KeyCode.Tab, KeyCode.Return)

                //Do it!
                .Invoke()
                ;
        }
    }
}