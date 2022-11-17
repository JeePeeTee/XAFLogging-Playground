#region MIT License

// ==========================================================
// 
// XAFLogging project - Copyright (c) 2022 JeePeeTee
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// 
// ===========================================================

#endregion

#region usings

using System.Configuration;
using Serilog;
using Serilog.Context;
using Serilog.Sinks.MSSqlServer;

#endregion

namespace XAFLogging.Win;

// For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Win.WinApplication._members
public class XAFLoggingWindowsFormsApplication : WinApplication {
    private IDisposable _logUser;

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

    protected override void OnLoggedOn(LogonEventArgs args) {
        base.OnLoggedOn(args);

        _logUser = GlobalLogContext.PushProperty("User", SecuritySystem.CurrentUserName);
    }

    protected override void OnLoggedOff() {
        _logUser.Dispose();
        base.OnLoggedOff();
    }


    private void XAFLoggingWindowsFormsApplication_CustomizeLanguagesList(object sender, CustomizeLanguagesListEventArgs e) {
        var userLanguageName = Thread.CurrentThread.CurrentUICulture.Name;
        if (userLanguageName != "en-US" && e.Languages.IndexOf(userLanguageName) == -1) {
            e.Languages.Add(userLanguageName);
        }
    }

    private void XAFLoggingWindowsFormsApplication_DatabaseVersionMismatch(object sender, DatabaseVersionMismatchEventArgs e) {
#if EASYTEST
        e.Updater.Update();
        e.Handled = true;
#else
        if (System.Diagnostics.Debugger.IsAttached) {
            e.Updater.Update();
            e.Handled = true;
        }
        else {
            var message = "The application cannot connect to the specified database, " +
                          "because the database doesn't exist, its version is older " +
                          "than that of the application or its schema does not match " +
                          "the ORM data model structure. To avoid this error, use one " +
                          "of the solutions from the https://www.devexpress.com/kb=T367835 KB Article.";

            if (e.CompatibilityError != null && e.CompatibilityError.Exception != null) {
                message += "\r\n\r\nInner exception: " + e.CompatibilityError.Exception.Message;
            }

            throw new InvalidOperationException(message);
        }
#endif
    }
}