using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using TemplateGenerator;

Start();

static void Start()
{
    var type = ReceiveType();

    //기본 경로 지정
    var root = "MenuTemplate";
    var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    if (type == "1")
    {
        var programName = ReceiveName();

        var processor = new Processor();

        var desktopRoot = Path.Combine(path, root);
        if (!Directory.Exists(desktopRoot))
            Directory.CreateDirectory(desktopRoot);

        var programNamePath = Path.Combine(desktopRoot, programName);
        if (!Directory.Exists(programNamePath))
            Directory.CreateDirectory(programNamePath);

        var BEPath = Path.Combine(programNamePath, "BE");
        if (!Directory.Exists(BEPath))
            Directory.CreateDirectory(BEPath);

        var FEPath = Path.Combine(programNamePath, "FE");
        if (!Directory.Exists(FEPath))
            Directory.CreateDirectory(FEPath);

        var SQLPath = Path.Combine(programNamePath, "SQL");
        if (!Directory.Exists(SQLPath))
            Directory.CreateDirectory(SQLPath);

        processor.CreateQuery(programName, SQLPath);
        processor.CreateJavaContents(programName, BEPath);
        processor.CreateJsxContents(programName, FEPath);

        if (Directory.Exists(programNamePath))
        {
            var startInfo = new ProcessStartInfo
            {
                Arguments = programNamePath,
                FileName = "explorer.exe"
            };

            Process.Start(startInfo);
        }
    }

    if (type == "2")
    {
        var programName = Guid.NewGuid().ToString();
        var desktopRoot = Path.Combine(path, root);
        if (!Directory.Exists(desktopRoot))
            Directory.CreateDirectory(desktopRoot);

        // var programNamePath = Path.Combine(desktopRoot, programName);
        // if (!Directory.Exists(programNamePath))
        //     Directory.CreateDirectory(programNamePath);

        // var FEPath = Path.Combine(programNamePath, "FE");
        // if (!Directory.Exists(FEPath))
        //     Directory.CreateDirectory(FEPath);

        var contentsToApply = ReceiveMultiLineInput("컬럼 정보 입력");
        var processor = new Processor();
        processor.CreateGridCols(programName, desktopRoot, contentsToApply);

        var resultFile = Path.Combine(desktopRoot, $"{programName}_cols.json");
        Console.WriteLine(resultFile);
        if (File.Exists(resultFile))
        {
            Process.Start(new ProcessStartInfo { FileName = resultFile, UseShellExecute = true });
        }
    }

    if (type == "3")
    {
        var routingName = ReceiveName("Route name", "ex)/api/masterData/generateMbomByBOM");

        var inData = ReceiveMultiLineInput("IN_DATA", "'Name | Type | Desc | 조회조건' 컬럼 포함");

        var programName = Guid.NewGuid().ToString();
        var desktopRoot = Path.Combine(path, root);
        if (!Directory.Exists(desktopRoot))
            Directory.CreateDirectory(desktopRoot);

        var processor = new Processor();
        processor.CreateApi(programName, desktopRoot, inData, routingName);

        var resultFile = Path.Combine(desktopRoot, $"{programName}_api.json");
        Console.WriteLine(resultFile);
        if (File.Exists(resultFile))
        {
            Process.Start(new ProcessStartInfo { FileName = resultFile, UseShellExecute = true });
        }

    }
}

static string ReceiveType()
{
    Console.WriteLine("1: Template, 2: Grid Cols, 3: Api");
    //todo: 조회 조건 생성 모드도 추가 하면 좋을듯
    //CommonSearchComponent에 CommonSearchItemComponent 레벨로 추가
    //체크박스, 체크박스 그룹, 라디오 버튼그룹, 콤보박스, 멀티 콤보박스에 해당하는 CommonSearchItemComponent 생성하고
    //또 그에 해당하는 state구문 생성, currentParam, applyComSrchCompParameters에 추가되는 구문 생성 그리고 ResultAreaComponent.retrieveData에 추가

    //2: Grid Cols 모드에서 check 컬럼 추가 여부 구분하여 생성
    var type = Console.ReadLine();
    if (type != "1" && type != "2" && type != "3")
    {
        Console.WriteLine("똑바로 입력해..");
        return ReceiveType();
    }

    return type;
}

static string ReceiveName(string name = "program name", string ex = null)
{
    Console.WriteLine($"{name} 입력", ex ?? "");
    var programName = Console.ReadLine();

    if (string.IsNullOrEmpty(programName))
    {
        Console.WriteLine("입력해..");
        programName = Console.ReadLine();
    }

    return programName;
}

static string ReceiveMultiLineInput(string question, string extra = null)
{
    Console.WriteLine($"{question} 입력 {extra}");
    //var inputContents = Console.ReadLine();
    var input = Console.In.ReadToEnd();

    return input;
}
