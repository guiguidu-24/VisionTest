namespace POC_Tesseract.UserInterface
{
    public static class Screen
    {
        public static Rectangle Bounds { get; private set; }
        public static int Width => Bounds.Width;
        public static int Height => Bounds.Height;

        static Screen()
        {
            // Crée un formulaire invisible pour déterminer la taille de l'écran
            using (var form = new Form())
            {
                form.FormBorderStyle = FormBorderStyle.None; // Supprime les bordures
                form.StartPosition = FormStartPosition.Manual; // Positionne la fenêtre manuellement
                form.Location = new Point(0, 0); // Place la fenêtre en haut à gauche
                form.ShowInTaskbar = false; // Ne pas afficher dans la barre des tâches
                form.Opacity = 0; // Rendre le formulaire invisible

                // Affiche et maximise la fenêtre
                form.Show();
                form.WindowState = FormWindowState.Maximized;

                // Récupère la taille de la fenêtre maximisée
                Bounds = new Rectangle(0, 0, form.Width, form.Height);
            }
        }
    }
}
