using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl;
using Microsoft.Msagl.Splines;
using Microsoft.Msagl.Drawing;
namespace AdventureWorksSchema
{
    internal static class Program
    {


        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}
