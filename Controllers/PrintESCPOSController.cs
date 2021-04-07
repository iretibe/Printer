using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Neodynamic.SDK.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Printer.Controllers
{
    public class PrintESCPOSController : Controller
    {
        [Obsolete]
        private readonly IHostingEnvironment _hostEnvironment;

        [Obsolete]
        public PrintESCPOSController(IHostingEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            ViewData["WCPScript"] = WebClientPrint.CreateScript(Url.Action("ProcessRequest", 
                "WebClientPrintAPI", null, Url.ActionContext.HttpContext.Request.Scheme), 
                Url.Action("PrintCommands", "PrintESCPOS", null, Url.ActionContext.HttpContext.Request.Scheme), 
                Url.ActionContext.HttpContext.Session.Id);

            return View();
        }

        public IActionResult PrintCommands(string useDefaultPrinter, string printerName)
        {
            //Create ESC/POS commands for sample receipt
            string ESC = "0x1B"; //ESC byte in hex notation
            string NewLine = "0x0A"; //LF byte in hex notation

            string cmds = ESC + "@"; //Initializes the printer (ESC @)
            cmds += ESC + "!" + "0x38"; //Emphasized + Double-height + Double-width mode selected (ESC ! (8 + 16 + 32)) 56 dec => 38 hex
            cmds += "BEST DEAL STORES"; //text to print
            cmds += NewLine + NewLine;
            cmds += ESC + "!" + "0x00"; //Character font A selected (ESC ! 0)
            cmds += "COOKIES                   5.00";
            cmds += NewLine;
            cmds += "MILK 65 Fl oz             3.78";
            cmds += NewLine + NewLine;
            cmds += "SUBTOTAL                  8.78";
            cmds += NewLine;
            cmds += "TAX 5%                    0.44";
            cmds += NewLine;
            cmds += "TOTAL                     9.22";
            cmds += NewLine;
            cmds += "CASH TEND                10.00";
            cmds += NewLine;
            cmds += "CASH DUE                  0.78";
            cmds += NewLine + NewLine;
            cmds += ESC + "!" + "0x18"; //Emphasized + Double-height mode selected (ESC ! (16 + 8)) 24 dec => 18 hex
            cmds += "# ITEMS SOLD 2";
            cmds += ESC + "!" + "0x00"; //Character font A selected (ESC ! 0)
            cmds += NewLine + NewLine;
            cmds += "11/03/13  19:53:17";


            //Create a ClientPrintJob and send it back to the client!
            ClientPrintJob cpj = new ClientPrintJob();
            //set  ESCPOS commands to print...
            cpj.PrinterCommands = cmds;
            cpj.FormatHexValues = true;

            //set client printer...
            if (useDefaultPrinter == "checked" || printerName == "null")
                cpj.ClientPrinter = new DefaultPrinter();
            else
                cpj.ClientPrinter = new InstalledPrinter(printerName);


            return File(cpj.GetContent(), "application/octet-stream");
        }
    }
}
