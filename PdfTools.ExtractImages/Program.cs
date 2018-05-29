using System.Drawing.Imaging;
using System.IO;
using Microsoft.Extensions.CommandLineUtils;
using Syncfusion.Pdf.Parsing;

namespace PdfTools.ExtractImages
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new CommandLineApplication();
            app.HelpOption("--help");

            var inputArg = app.Argument("[input pdf]", "");
            var formatOption = app.Option("--format", "output file format", CommandOptionType.SingleValue);
            var outputOption = app.Option("--output", "output folder", CommandOptionType.SingleValue);
            app.OnExecute(() =>
            {
                var input = inputArg.Value;
                var output = outputOption.Value();

                var fileName = Path.GetFileNameWithoutExtension(input);
                var fileNameTemplate = formatOption.HasValue() ? formatOption.Value() : "{filename} ({page}).{extension}";
                if (string.IsNullOrEmpty(output))
                {
                    output = Path.GetDirectoryName(input);
                }

                using (var doc = new PdfLoadedDocument(input))
                {
                    for (var i = 0; i < doc.Pages.Count; i++)
                    {
                        var page = doc.Pages[i];
                        var images = page.ExtractImages();
                        try
                        {
                            foreach (var image in images)
                            {
                                var outputFileName = fileNameTemplate
                                  .Replace("{filename}", fileName)
                                  .Replace("{page}", i.ToString())
                                  .Replace("{pagecount}", doc.Pages.Count.ToString())
                                  .Replace("{extension}", "png");

                                var outputPath = Path.Combine(output, outputFileName);
                                image.Save(outputPath, ImageFormat.Png);
                            }
                        }
                        finally
                        {
                            if (images != null)
                            {
                                foreach (var image in images)
                                {
                                    image?.Dispose();
                                }
                            }
                        }
                    }
                }

                return 0;
            });

            app.Execute(args);
        }
    }
}
}
