using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Controls;


namespace TableauDefinitionImportUtility
{
    public static class Common
    {
    


    /// <summary>
    /// writes to the console
    /// </summary>
    /// <param name="txt">log text message</param>
    /// <param name="txtLog">the actual log window textbox control</param>
    public static void logger(string txt, TextBox txtLog)
    {
        txtLog.Dispatcher.Invoke(() =>
        {
            txtLog.AppendText(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + " - " + txt + Environment.NewLine);
            txtLog.ScrollToEnd();
        });
    }


    }
}
