//TODO: API GANERATION
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TemplateGenerator
{
    public class Processor
    {
        private XElement? _doc;

        public Processor()
        {
            _doc = GetDocTemplate();
        }

        private XElement? GetDocTemplate()
        {
            var templatePath = @"./CodeTemplate.xml";
            return XDocument.Load(templatePath).Root;
        }

        string GetCData(string contents)
        {
            var regex =
                new Regex(@"\<\!\[CDATA\[(?<text>[^\]]*)\]\]\>", RegexOptions.None);
            if (!regex.IsMatch(contents))
                return string.Empty;

            var match = regex.Match(contents);
            return match.Groups["text"].Value;
        }

        public void CreateQuery(string programName, string path)
        {
            StringBuilder sb = new();
            sb.Append(
                GetCData(
                    _doc?.Descendants(Enums.BaseCodeType.new_programId_query.ToString())?
                    .DescendantNodes()?
                    .FirstOrDefault()?
                    .ToString() ?? ""));

            sb.AppendLine(Environment.NewLine);

            sb.AppendFormat(
                GetCData(
                    _doc?.Descendants(Enums.BaseCodeType.add_programId_query.ToString())?
                    .DescendantNodes()?
                    .FirstOrDefault()?
                    .ToString() ?? ""), programName);

            sb.AppendLine(Environment.NewLine);

            sb.Append(
                GetCData(
                    _doc?.Descendants(Enums.BaseCodeType.add_auth_query.ToString())
                    .DescendantNodes()?
                    .FirstOrDefault()?
                    .ToString() ?? ""));

            CreateFile(Path.Combine(path, $"{programName}.sql"), sb.ToString());
        }

        public void CreateJavaContents(string programName, string path)
        {
            var controllerContents =
                GetCData(
                    _doc?.Descendants(Enums.BaseCodeType.controller_code.ToString())?
                    .DescendantNodes()?
                    .FirstOrDefault()?.ToString() ?? "");

            CreateFile(
                Path.Combine(path, $"{programName}Controller.java"),
                string.Format(controllerContents, programName));


            var serviceContents =
                GetCData(
                    _doc?.Descendants(Enums.BaseCodeType.service_code.ToString())?
                    .DescendantNodes()?
                    .FirstOrDefault()?.ToString() ?? "");

            CreateFile(
                Path.Combine(path, $"{programName}Service.java"),
                string.Format(serviceContents, programName));


            var implementContents =
                GetCData(
                    _doc?.Descendants(Enums.BaseCodeType.implement_code.ToString())?
                    .DescendantNodes()?
                    .FirstOrDefault()?.ToString() ?? "");

            CreateFile(
                Path.Combine(path, $"{programName}ServiceImpl.java"),
                string.Format(implementContents, programName));
        }

        public void CreateJsxContents(string programName, string FEPath)
        {
            var FERoot = Path.Combine(FEPath, programName);
            if (!Directory.Exists(FERoot))
                Directory.CreateDirectory(FERoot);

            var componentPath = Path.Combine(FERoot, "components");
            if (!Directory.Exists(componentPath))
                Directory.CreateDirectory(componentPath);

            var servicesPath = Path.Combine(FERoot, "services");
            if (!Directory.Exists(servicesPath))
                Directory.CreateDirectory(servicesPath);

            var searchAreaComponentTemplate =
                _doc?.Descendants(Enums.BaseCodeType.js_search_area.ToString())?
                .DescendantNodes().Where(x => x.NodeType == System.Xml.XmlNodeType.CDATA)?
                .ToList()?.FirstOrDefault()?.Parent?.Value ?? "";

            CreateFile(
                Path.Combine(componentPath, $"SearchAreaComponent.jsx"),
                string.Format(searchAreaComponentTemplate, programName));

            var resultAreaComponentTemplate =
            _doc?.Descendants(Enums.BaseCodeType.js_result_area.ToString())?
                .DescendantNodes().Where(x => x.NodeType == System.Xml.XmlNodeType.CDATA)?
                .ToList()?.FirstOrDefault()?.Parent?.Value ?? "";

            CreateFile(
                Path.Combine(componentPath, $"ResultAreaComponent.jsx"),
                string.Format(resultAreaComponentTemplate, programName));

            var serviceTemplate =
            _doc?.Descendants(Enums.BaseCodeType.js_service.ToString())?
                .DescendantNodes().Where(x => x.NodeType == System.Xml.XmlNodeType.CDATA)?
                .ToList()?.FirstOrDefault()?.Parent?.Value ?? "";

            CreateFile(
                Path.Combine(servicesPath, $"{programName}Service.js"),
                string.Format(serviceTemplate, programName));

            var containerComponentTemplate =
                _doc?.Descendants(Enums.BaseCodeType.js_container.ToString())?
                .DescendantNodes().Where(x => x.NodeType == System.Xml.XmlNodeType.CDATA)?
                .ToList()?.FirstOrDefault()?.Parent?.Value ?? "";

            CreateFile(
                Path.Combine(FERoot, $"{programName}Container.js"),
                string.Format(containerComponentTemplate, programName));
        }

        public void CreateGridCols(string programName, string path, string contentsToApply)
        {
            //{binding: '**',header: langUtils.getLangData(langSet.object,'**','**',),width: 50,visible: false,},
            var gridColTemplate =
                GetCData(
                    _doc?.Descendants(Enums.BaseCodeType.grid_col.ToString())?
                    .DescendantNodes()?
                    .FirstOrDefault()?.ToString() ?? "");

            //DEFECT_RANK Decimal 불량순위 DEFECT_RANK FALSE right

            var lines = string.Join("\n",
                contentsToApply.Split(Environment.NewLine).Where(x => !string.IsNullOrEmpty(x))
                .Select(x => new { Binding = x.Split('\t').FirstOrDefault(), Visible = x.Split('\t')[4].ToLower(), Align = x.Split('\t')[5].ToLower() })
                .Select(x => string.Format(gridColTemplate, x.Binding, x.Visible, x.Align)));

            CreateFile(
                Path.Combine(path, $"{programName}_cols.json"),
                lines);
        }

        public void CreateApi(string programName, string path, string inData, string route)
        {
            //{binding: '**',header: langUtils.getLangData(langSet.object,'**','**',),width: 50,visible: false,},
            var apiTemplate =
                GetCData(
                    _doc?.Descendants(Enums.BaseCodeType.api.ToString())?
                    .DescendantNodes()?
                    .FirstOrDefault()?.ToString() ?? "");

            var apiName = route.Split('/').Last();
            var param = @"Map<String, Object> params";
            var fromDate = string.Empty;
            if (inData.ToUpper().Contains("FROM_DATE"))
            {
                fromDate = @"inData.put(""FROM_DATE"", new SimpleDateFormat(""yyyyMMdd"").parse(params.get(""FROM_DATE"").toString()));";
            }
            var toDate = string.Empty;
            if (inData.ToUpper().Contains("TO_DATE"))
            {
                toDate = @"inData.put(""TO_DATE"", new SimpleDateFormat(""yyyyMMdd"").parse(params.get(""TO_DATE"").toString()));";
            }

            var lines = string.Join("\n",
                inData.Split(Environment.NewLine)
                .Where(x => !string.IsNullOrEmpty(x) && !x.ToUpper().Contains("FROM_DATE") && !x.ToUpper().Contains("FROM_DATE"))
                .Select(x => GetEachParam(x)));

            var result = string.Format(apiTemplate, route, apiName, param, lines);

            CreateFile(
                Path.Combine(path, $"{programName}_api.json"),
                lines);
        }

        string GetEachParam(string paramContent)
        {
            var apiMdc =
                GetCData(
                    _doc?.Descendants(Enums.BaseCodeType.api_MDC_param.ToString())?
                    .DescendantNodes()?
                    .FirstOrDefault()?.ToString() ?? "").Split(',');
            var inName = paramContent.Split("\t".ToCharArray())[0];

            if (apiMdc.Contains(inName))
                return ChangeMdc(inName);

            if (paramContent.ToLower().Contains("null"))
                return @$"if (!StringUtil.isEmpty((String) params.get(""{inName}""))) inData.put(""{inName}"", params.get(""{inName}""));";

            return string.Empty;
        }

        string ChangeMdc(string mdc)
        {
            switch (mdc)
            {
                case "LANGUAGE_ID":
                    return @"inData.put(""LANGUAGE_ID"", MDC.get(""userLangID""));";
                case "USER_ID":
                    return @"inData.put(""USER_ID"", MDC.get(""userId""));";
                case "FACILITY_CODE":
                    return @"inData.put(""FACILITY_CODE"", MDC.get(""userFacility""));";
                case "ORGANIZATION_ID":
                    return @"inData.put(""ORGANIZATION_ID"", MDC.get(""userORGANIZATION_ID_LIST""));";
                case "SCHEDULE_GROUP_ID":
                    return @"inData.put(""LANGUSCHEDULE_GROUP_IDAGE_ID"", MDC.get(""userSCHEDULE_GROUP_ID_LIST""));";
                default:
                    return "";
            }
        }

        void CreateFile(string fileName, string contents)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);

            using var fs = File.Create(fileName);
            byte[] author = new UTF8Encoding(true).GetBytes(contents);
            fs.Write(author, 0, author.Length);
        }
    }
}
