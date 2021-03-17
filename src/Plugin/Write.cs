using System;
using System.Runtime.CompilerServices;

namespace Ollio.Plugin
{
    public class Write {
        public static void Debug(string message, [CallerMemberName] string caller = null) =>
            Ollio.Common.Write.Debug(message, caller);

        public static void Error(Exception exception, Guid? reference = null, [CallerMemberName] string caller = null) =>
            Ollio.Common.Write.Error(exception, reference, caller);

        public static void Info(string message, [CallerMemberName] string caller = null) =>
            Ollio.Common.Write.Info(message, caller);

        public static void Success(string message, [CallerMemberName] string caller = null) =>
            Ollio.Common.Write.Success(message, caller);

        public static void Warning(string message, [CallerMemberName] string caller = null) =>
            Ollio.Common.Write.Warning(message, caller);

        public static void Reset() => Ollio.Common.Write.Reset();
    }
}