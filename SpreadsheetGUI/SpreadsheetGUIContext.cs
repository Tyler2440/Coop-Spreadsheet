using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetGUI
{
    class SpreadsheetGUIContext : ApplicationContext
    {
        // Number of open forms
        private int formCount = 0;

    // Singleton ApplicationContext
    private static SpreadsheetGUIContext appContext;

    /// <summary>
    /// Private constructor for singleton pattern
    /// </summary>
    private SpreadsheetGUIContext()
    {
    }

    /// <summary>
    /// Returns the one DemoApplicationContext.
    /// </summary>
    public static SpreadsheetGUIContext getAppContext()
    {
        if (appContext == null)
        {
            appContext = new SpreadsheetGUIContext();
        }
        return appContext;
    }

    /// <summary>
    /// Runs the form
    /// </summary>
    public void RunForm(Form form)
    {
        // One more form is running
        formCount++;

        // When this form closes, we want to find out
        form.FormClosed += (o, e) => { if (--formCount <= 0) ExitThread(); };

        // Run the form
        form.Show();
    }
}

static class SpreadsheeetGUIContext
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // Start an application context and run one form inside it
        SpreadsheetGUIContext appContext = SpreadsheetGUIContext.getAppContext();
        appContext.RunForm(new SpreadsheetForm());
        Application.Run(appContext);
    }
}
}
