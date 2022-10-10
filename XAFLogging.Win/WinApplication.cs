﻿using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.Utils;
using Serilog;
using Serilog.Context;
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
            .Enrich.FromLogContext()
            .Enrich.FromGlobalLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithProcessId()
            .MinimumLevel.Information()
            .WriteTo.MSSqlServer(
                connectionString: ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString,
                columnOptions: ColumnOptions,
                sinkOptions: new MSSqlServerSinkOptions {
                    SchemaName = "dbo",
                    TableName = "LogEvents",
                    AutoCreateSqlTable = true
                })
            .CreateLogger();
    }
    
    private static ColumnOptions ColumnOptions {
        // Add extra columns to Event logging table
        get {
            var columnOptions = new ColumnOptions {
                AdditionalColumns = new Collection<SqlColumn> {
                    new SqlColumn
                        { ColumnName = "User", PropertyName = "User", DataType = SqlDbType.NVarChar, DataLength = 64 },
                    new SqlColumn
                        { ColumnName = "MachineName", PropertyName = "MachineName", DataType = SqlDbType.NVarChar, DataLength = 64 },
                    new SqlColumn
                        { ColumnName = "ProcessId", PropertyName = "ProcessId", DataType = SqlDbType.Int }
                }
            };

            // Remove XML logging
            columnOptions.Store.Remove(StandardColumn.Properties);
            // Add Json logging
            columnOptions.Store.Add(StandardColumn.LogEvent);
            return columnOptions;
        }
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
