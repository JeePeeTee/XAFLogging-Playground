using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Blazor;
using DevExpress.Persistent.Validation;
using Serilog;
using Serilog.Context;
using Serilog.Sinks.MSSqlServer;

namespace XAFLogging.Blazor.Server;

public class XAFLoggingBlazorApplication : BlazorApplication {
    
    private IDisposable _logUser;
    protected override void OnLoggedOn(LogonEventArgs args) {
        base.OnLoggedOn(args);
        
        _logUser = GlobalLogContext.PushProperty("User", SecuritySystem.CurrentUserName);
    }
    protected override void OnLoggedOff() {
        _logUser.Dispose();
        base.OnLoggedOff();
    }

    public XAFLoggingBlazorApplication() {
        ApplicationName = "XAFLogging";
        CheckCompatibilityType = CheckCompatibilityType.DatabaseSchema;
        DatabaseVersionMismatch += XAFLoggingBlazorApplication_DatabaseVersionMismatch;
        
        GlobalLogContext.PushProperty("Runtime", "Blazor");
        Log.Information("Application launched");
    }
    protected override void OnSetupStarted() {
        base.OnSetupStarted();
#if DEBUG
        if(System.Diagnostics.Debugger.IsAttached && CheckCompatibilityType == CheckCompatibilityType.DatabaseSchema) {
            DatabaseUpdateMode = DatabaseUpdateMode.UpdateDatabaseAlways;
        }
#endif
        
    }
    private void XAFLoggingBlazorApplication_DatabaseVersionMismatch(object sender, DatabaseVersionMismatchEventArgs e) {
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
                "because the database doesn't exist, its version is older " +
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
