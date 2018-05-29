using Meziantou.Framework.Utilities;
using Microsoft.Extensions.CommandLineUtils;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Parsing;

namespace PdfToolds.InsertImage
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new CommandLineApplication();
            var inputArg = app.Argument("[input pdf]", "");
            var imageArg = app.Argument("[image]", "");
            var pageOption = app.Option("-p", "page number", CommandOptionType.SingleValue);
            var xOption = app.Option("-x", "horizontal position", CommandOptionType.SingleValue);
            var yOption = app.Option("-y", "vertical position", CommandOptionType.SingleValue);
            var outputOption = app.Option("--output", "output file", CommandOptionType.SingleValue);
            app.OnExecute(() =>
            {
                // "input.pdf" "Signature50.png" 4 150 580 "output.pdf"
                var inputFile = inputArg.Value;
                var imageFile = imageArg.Value;
                var pageNumber = ConvertUtilities.ChangeType(pageOption.Value(), 0);
                var positionX = ConvertUtilities.ChangeType(xOption.Value(), 0);
                var positionY = ConvertUtilities.ChangeType(yOption.Value(), 0);
                var outputFile = outputOption.Value();

                using (var pdfFile = new PdfLoadedDocument(inputFile))
                {
                    var page = pdfFile.Pages[pageNumber];
                    var graphics = page.Graphics;
                    var image = new PdfBitmap(imageFile);
                    graphics.DrawImage(image, positionX, positionY);
                    pdfFile.Save(outputFile);
                }

                return 0;
            });

            app.Execute(args);
        }
    }
}
