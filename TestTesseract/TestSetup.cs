[SetUpFixture]
public class TestSetup
{
    [OneTimeSetUp]
    public void GlobalSetup()
    {
        // D�finir le r�pertoire de travail pour tous les tests
        //Environment.CurrentDirectory = @"..\..\..\..\POC Tesseract\";
    }
}
