using System;
using System.Collections.Generic;
using System.Windows.Forms;
//
//Author : JuLius Lucero Ramos, ECE, QA
//Email  : juliusLramos@hotmail.com
//
namespace QXplorer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()

        {
            GlobalVariables oVariables = new GlobalVariables();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmQXplorer(oVariables));
        }
    }
}