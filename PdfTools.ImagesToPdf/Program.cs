using System.IO;
using System.Linq;
using Microsoft.Extensions.CommandLineUtils;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;

namespace PdfTools.ImagesToPdf
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new CommandLineApplication();
            app.HelpOption("--help");
            var outputArg = app.Argument("[pdf file]", "", multipleValues: false);
            var inputArg = app.Argument("[input images]", "", multipleValues: true);

            app.OnExecute(() =>
            {
                using (var doc = new PdfDocument())
                {
                    foreach (var arg in inputArg.Values)
                    {
                        if (Directory.Exists(arg))
                        {
                            foreach (var file in Directory.GetFiles(arg).OrderBy(_ => _))
                            {
                                AddPage(file);
                            }
                        }
                        else
                        {
                            AddPage(arg);
                        }
                    }

                    doc.Save(outputArg.Value);

                    void AddPage(string imagePath)
                    {
                        var page = doc.Pages.Add();
                        var graphics = page.Graphics;

                        PdfImage image = new PdfBitmap(imagePath);
                        float PageWidth = page.Graphics.ClientSize.Width;
                        float PageHeight = page.Graphics.ClientSize.Height;
                        float myWidth = image.Width;
                        float myHeight = image.Height;

                        if (myWidth > PageWidth)
                        {
                            float shrinkFactor = myWidth / PageWidth;
                            myWidth = PageWidth;
                            myHeight = myHeight / shrinkFactor;
                        }

                        if (myHeight > PageHeight)
                        {
                            var shrinkFactor = myHeight / PageHeight;
                            myHeight = PageHeight;
                            myWidth = myWidth / shrinkFactor;
                        }

                        float XPosition = 0;
                        float YPosition = 0;
                        graphics.DrawImage(image, XPosition, YPosition, myWidth, myHeight);
                    }
                }
                return 0;
            });

            app.Execute(args);
        }
    }
}
