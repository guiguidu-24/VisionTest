using POC_Tesseract;


var appli = new Appli("notepad");
appli.Open();
appli.Wait(5000);
appli.Close();
Console.WriteLine("Notepad opened and closed after 5 seconds.");

