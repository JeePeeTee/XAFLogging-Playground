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

using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.BaseImpl;
using Serilog;
using Serilog.Sinks.MSSqlServer;

#endregion

namespace XAFLogging.Blazor.Server;

[ToolboxItemFilter("Xaf.Platform.Blazor")]
// For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.ModuleBase.
public sealed class XAFLoggingBlazorModule : ModuleBase {
    public XAFLoggingBlazorModule() { }

    //private void Application_CreateCustomModelDifferenceStore(object sender, CreateCustomModelDifferenceStoreEventArgs e) {
    //    e.Store = new ModelDifferenceDbStore((XafApplication)sender, typeof(ModelDifference), true, "Blazor");
    //    e.Handled = true;
    //}
    private void Application_CreateCustomUserModelDifferenceStore(object sender, CreateCustomModelDifferenceStoreEventArgs e) {
        e.Store = new ModelDifferenceDbStore((XafApplication)sender, typeof(ModelDifference), false, "Blazor");
        e.Handled = true;
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