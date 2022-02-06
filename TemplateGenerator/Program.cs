using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using TemplateGenerator;

Start();

static void Start()
{
    var type = ReceiveType();

    if (type == "1")
    {
        var programName = ReceiveName();

        var processor = new Processor();

        //기본 경로 지정
        var root = "MenuTemplate";
        var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        var desktopRoot = Path.Combine(path, root);
        if (!Directory.Exists(desktopRoot))
            Directory.CreateDirectory(desktopRoot);

        var programNamePath = Path.Combine(desktopRoot, programName);
        if (!Directory.Exists(programNamePath))
            Directory.CreateDirectory(programNamePath);

        processor.CreateQuery(programName, programNamePath);
        processor.CreateJavaContents(programName, programNamePath);
    }

    if (type == "2")
    {

    }
}

static string ReceiveType()
{
    Console.WriteLine("1: Template, 2: API");
    var type = Console.ReadLine();
    if (type != "1" && type != "2")
    {
        Console.WriteLine("똑바로 입력해..");
        return ReceiveType();
    }

    return type;
}

static string ReceiveName()
{
    Console.WriteLine("program name 입력");
    var programName = Console.ReadLine();

    if (string.IsNullOrEmpty(programName))
    {
        Console.WriteLine("입력해..");
        programName = Console.ReadLine();
    }

    return programName;
}
