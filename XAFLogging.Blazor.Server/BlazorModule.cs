using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.BaseImpl;
using Serilog;
using Serilog.Sinks.MSSqlServer;

namespace XAFLogging.Blazor.Server;

[ToolboxItemFilter("Xaf.Platform.Blazor")]
// For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.ModuleBase.
public sealed class XAFLoggingBlazorModule : ModuleBase {
    //private void Application_CreateCustomModelDifferenceStore(object sender, CreateCustomModelDifferenceStoreEventArgs e) {
    //    e.Store = new ModelDifferenceDbStore((XafApplication)sender, typeof(ModelDifference), true, "Blazor");
    //    e.Handled = true;
    //}
    private void Application_CreateCustomUserModelDifferenceStore(object sender, CreateCustomModelDifferenceStoreEventArgs e) {
        e.Store = new ModelDifferenceDbStore((XafApplication)sender, typeof(ModelDifference), false, "Blazor");
        e.Handled = true;
    }
    public XAFLoggingBlazorModule() {
    }
    public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
        return ModuleUpdater.EmptyModuleUpdaters;
    }
    public override void Setup(XafApplication application) {
        base.Setup(application);
        //application.CreateCustomModelDifferenceStore += Application_CreateCustomModelDifferenceStore;
        application.CreateCustomUserModelDifferenceStore += Application_CreateCustomUserModelDifferenceStore;

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
                connectionString: @"Integrated Security=SSPI;Pooling=false;Data Source=(localdb)\mssqllocaldb;Initial Catalog=XAFLogging",
                columnOptions: Logging.SqlSettings.ColumnOptions,
                sinkOptions: new MSSqlServerSinkOptions {
                    SchemaName = "dbo",
                    TableName = "LogEvents",
                    AutoCreateSqlTable = true
                })
            .CreateLogger();
    }
}
