using POC_Tesseract;
using System.Diagnostics;


var appli = new Appli("notepad");
appli.Open();
Thread.Sleep(2000);

appli.CloseWindow();

