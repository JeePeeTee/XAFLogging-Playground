using System.Configuration;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.XtraEditors;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Utils;
using System.Reflection;
using DevExpress.Persistent.Validation;
using DevExpress.Utils.MVVM;
using Serilog;
using Serilog.Context;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace XAFLogging.Win;

static class Program {
    private static bool ContainsArgument(string[] args, string argument) {
        return args.Any(arg => arg.TrimStart('/').TrimStart('-').ToLower() == argument.ToLower());
    }
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    public static int Main(string[] args) {
        if(ContainsArgument(args, "help") || ContainsArgument(args, "h")) {
            Console.WriteLine("Updates the database when its version does not match the application's version.");
            Console.WriteLine();
            Console.WriteLine($"    {Assembly.GetExecutingAssembly().GetName().Name}.exe --updateDatabase [--forceUpdate --silent]");
            Console.WriteLine();
            Console.WriteLine("--forceUpdate - Marks that the database must be updated whether its version matches the application's version or not.");
            Console.WriteLine("--silent - Marks that database update proceeds automatically and does not require any interaction with the user.");
            Console.WriteLine();
            Console.WriteLine($"Exit codes: 0 - {DBUpdaterStatus.UpdateCompleted}");
            Console.WriteLine($"            1 - {DBUpdaterStatus.UpdateError}");
            Console.WriteLine($"            2 - {DBUpdaterStatus.UpdateNotNeeded}");
            return 0;
        }
        FrameworkSettings.DefaultSettingsCompatibilityMode = FrameworkSettingsCompatibilityMode.Latest;
#if EASYTEST
        DevExpress.ExpressApp.Win.EasyTest.EasyTestRemotingRegistration.Register();
#endif
        WindowsFormsSettings.LoadApplicationSettings();
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
		DevExpress.Utils.ToolTipController.DefaultController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
        if(Tracing.GetFileLocationFromSettings() == DevExpress.Persistent.Base.FileLocation.CurrentUserApplicationDataFolder) {
            Tracing.LocalUserAppDataPath = Application.LocalUserAppDataPath;
        }

        Tracing.CreateCustomTracer += delegate(object s, CreateCustomTracerEventArgs args) {
            args.Tracer = new CustomTracing.CustomTracing();
        };

        
        Tracing.Initialize();

        string connectionString = null;
        if(ConfigurationManager.ConnectionStrings["ConnectionString"] != null) {
            connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        }
#if EASYTEST
        if(ConfigurationManager.ConnectionStrings["EasyTestConnectionString"] != null) {
            connectionString = ConfigurationManager.ConnectionStrings["EasyTestConnectionString"].ConnectionString;
        }
#endif
        ArgumentNullException.ThrowIfNull(connectionString);
        var winApplication = ApplicationBuilder.BuildApplication(connectionString);

        if (ContainsArgument(args, "updateDatabase")) {
            using var dbUpdater = new WinDBUpdater(() => winApplication);
            return dbUpdater.Update(
                forceUpdate: ContainsArgument(args, "forceUpdate"),
                silent: ContainsArgument(args, "silent"));
        }

        try {
            GlobalLogContext.PushProperty("Runtime", "Win");
            Log.Information("Application setup");
            winApplication.Setup();
            Log.Information("Application started");
            winApplication.Start();
            Log.Information("Application finished");
        }
        catch(Exception e) {
            winApplication.StopSplash();
            winApplication.HandleException(e);
            Log.Fatal(e, "Application exception");
        }
        finally {
            Log.CloseAndFlush();
        }

        return 0;
    }
}
