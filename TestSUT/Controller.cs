using System.Net.Security;

namespace TestSUT
{
    public static class Controller
    {
        private static Form1 form;
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        public static Form1 Run()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            form = new Form1();
            var thread = new Thread(() =>
            {
                Application.Run(new Form1());
            });

            thread.SetApartmentState(ApartmentState.STA); // OBLIGATOIRE
            thread.Start();

            return form;
        }

        [STAThread]
        static void Main()
        {
            //ApplicationConfiguration.Initialize();

            //var form = new Form1();

            // Lancer le formulaire dans un thread séparé
            //var task = new Task(() =>
            //{
            //    
            //});

            //task.Start();
            //Task.Run(() => Application.Run(form));

            //var thread = new Thread(() =>
            //{
            //    Application.Run(new Form1());
            //});
            //
            //thread.SetApartmentState(ApartmentState.STA); // OBLIGATOIRE
            //thread.Start();
            //
            //// Attendre un peu ou faire un traitement
            //Task.Delay(1000).Wait();
            //
            //// Fermer le formulaire depuis le thread UI
            //form.Invoke(() => form.Close());

            Run();
            Task.Delay(1000).Wait();
            Close();
        }


        public static void Close()
        {
            form.Invoke(() => form.Close());
        }
    }
}