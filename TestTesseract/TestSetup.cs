[SetUpFixture]
public class TestSetup
{
    [OneTimeSetUp]
    public void GlobalSetup()
    {
        // Définir le répertoire de travail pour tous les tests
        //Environment.CurrentDirectory = @"..\..\..\..\POC Tesseract\";
    }
}
