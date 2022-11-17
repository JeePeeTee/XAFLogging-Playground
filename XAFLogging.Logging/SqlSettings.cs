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

using System.Collections.ObjectModel;
using System.Data;
using Serilog.Sinks.MSSqlServer;

#endregion

namespace XAFLogging.Logging;

public class SqlSettings {
    public static ColumnOptions ColumnOptions {
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
}