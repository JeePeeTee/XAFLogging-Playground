using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.Utils;
using Serilog;
using Serilog.Context;
using Serilog.Exceptions;
using Serilog.Sinks.MSSqlServer;

namespace XAFLogging.Win;

// For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Win.WinApplication._members
public class XAFLoggingWindowsFormsApplication : WinApplication {
    
    private IDisposable _logUser;
    
    protected override void OnLoggedOn(LogonEventArgs args) {
        base.OnLoggedOn(args);

        _logUser = GlobalLogContext.PushProperty("User", SecuritySystem.CurrentUserName);
    }

    protected override void OnLoggedOff() {
        _logUser.Dispose();
        base.OnLoggedOff();
    }

    public XAFLoggingWindowsFormsApplication() {
		SplashScreen = new DXSplashScreen(typeof(XafSplashScreen), new DefaultOverlayFormOptions());
        ApplicationName = "XAFLogging";
        CheckCompatibilityType = CheckCompatibilityType.DatabaseSchema;
        UseOldTemplates = false;
        DatabaseVersionMismatch += XAFLoggingWindowsFormsApplication_DatabaseVersionMismatch;
        CustomizeLanguagesList += XAFLoggingWindowsFormsApplication_CustomizeLanguagesList;

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Seq("http://localhost:5341")
            .Enrich.FromLogContext()
            .Enrich.FromGlobalLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithProcessId()
            //Hack Log tons of data during exceptions!
            //.Enrich.WithExceptionDetails()
            .MinimumLevel.Information()
            .WriteTo.MSSqlServer(
                connectionString: ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString,
                columnOptions: Logging.SqlSettings.ColumnOptions,
                sinkOptions: new MSSqlServerSinkOptions {
                    SchemaName = "dbo",
                    TableName = "LogEvents",
                    AutoCreateSqlTable = true
                })
            .CreateLogger();
    }
    

    private void XAFLoggingWindowsFormsApplication_CustomizeLanguagesList(object sender, CustomizeLanguagesListEventArgs e) {
        string userLanguageName = Thread.CurrentThread.CurrentUICulture.Name;
        if(userLanguageName != "en-US" && e.Languages.IndexOf(userLanguageName) == -1) {
            e.Languages.Add(userLanguageName);
        }
    }
    private void XAFLoggingWindowsFormsApplication_DatabaseVersionMismatch(object sender, DatabaseVersionMismatchEventArgs e) {
#if EASYTEST
        e.Updater.Update();
        e.Handled = true;
#else
        if(System.Diagnostics.Debugger.IsAttached) {
            e.Updater.Update();
            e.Handled = true;
        }
        else {
			string message = "The application cannot connect to the specified database, " +
				"because the database doesn't exist, its version is older " +
				"than that of the application or its schema does not match " +
				"the ORM data model structure. To avoid this error, use one " +
				"of the solutions from the https://www.devexpress.com/kb=T367835 KB Article.";

			if(e.CompatibilityError != null && e.CompatibilityError.Exception != null) {
				message += "\r\n\r\nInner exception: " + e.CompatibilityError.Exception.Message;
			}
			throw new InvalidOperationException(message);
        }
#endif
    }
}
