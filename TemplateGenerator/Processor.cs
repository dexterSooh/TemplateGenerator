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
