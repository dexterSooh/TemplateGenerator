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
                .Select(x => new { Binding = x.Split('\t').FirstOrDefault(), Visible = x.Split('\t')[4].ToLower() })
                .Select(x => string.Format(gridColTemplate, x.Binding, x.Visible)));

            CreateFile(
                Path.Combine(path, $"{programName}_cols.json"),
                lines);
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
