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

using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Blazor.DesignTime;
using DevExpress.ExpressApp.Design;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using Serilog;

#endregion

namespace XAFLogging.Blazor.Server;

public class Program : IDesignTimeApplicationFactory {
    private static bool ContainsArgument(string[] args, string argument) {
        return args.Any(arg => arg.TrimStart('/').TrimStart('-').ToLower() == argument.ToLower());
    }

    public static int Main(string[] args) {
        Tracing.CreateCustomTracer += delegate(object s, CreateCustomTracerEventArgs args) { args.Tracer = new CustomTracing(); };

        Tracing.Initialize();

        if (ContainsArgument(args, "help") || ContainsArgument(args, "h")) {
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
        }
        else {
            FrameworkSettings.DefaultSettingsCompatibilityMode = FrameworkSettingsCompatibilityMode.Latest;
            var host = CreateHostBuilder(args).Build();
            if (ContainsArgument(args, "updateDatabase")) {
                using (var serviceScope = host.Services.CreateScope()) {
                    return serviceScope.ServiceProvider.GetRequiredService<DevExpress.ExpressApp.Utils.IDBUpdater>()
                        .Update(ContainsArgument(args, "forceUpdate"), ContainsArgument(args, "silent"));
                }
            }
            else {
                host.Run();
            }
        }

        return 0;
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            //.UseSerilog()
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });

    XafApplication IDesignTimeApplicationFactory.Create() {
        var hostBuilder = CreateHostBuilder(Array.Empty<string>());
        return DesignTimeApplicationFactoryHelper.Create(hostBuilder);
    }

    private class CustomTracing : Tracing {
        // Hack Not supported by Blazor ???
        public override void LogError(Exception exception) {
            // Implement custom logging for exceptions here.
            switch (exception) {
                case ValidationException validationException:
                    //ToDo Logs too much data...
                    //Removed 
                    //Log.Error(validationException, "Validation exception: {Details}", validationException.Result.Results);
                    Log.Error(validationException, "Validation exception");
                    break;
                default:
                    Log.Error(exception, "System error");
                    break;
            }
        }

        public override void LogWarning(string text, params object[] args) {
            //base.LogWarning(text, args);
            Log.Warning("Warning {Text}, {Args}", text, args);
        }

        public override void LogText(string text, params object[] args) {
            //base.LogText(text, args);
            Log.Debug("Text {Text}, {Args}", text, args);
        }

        public override void LogSetOfStrings(params string[] args) {
            //base.LogSetOfStrings(args);
        }
    }
}