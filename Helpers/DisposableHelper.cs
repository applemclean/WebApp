using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    public static class DisposableHelper
    {
        /// <summary>
        /// Throws an exception if this object is disposed
        /// </summary>
        /// <param name="disposable">Disposable object</param>
        /// <param name="isDisposed">Is this object disposed</param>
        public static void GuardNotDisposed(this IDisposable disposable, bool isDisposed)
        {
            if (isDisposed)
            {
                throw new InvalidOperationException(
                    $"{disposable.GetType().FullName} is disposed");
            }
        }

        /// <summary>
        /// Provides standard dispose behavior
        /// </summary>
        /// <param name="disposable">Disposable object</param>
        /// <param name="isDisposed">Is this object disposed</param>
        /// <param name="freeResources">An action of disposing resources</param>
        public static void StandardDispose(this IDisposable disposable, ref bool isDisposed, Action freeResources)
        {
            if (!isDisposed)
            {
                isDisposed = true;
                freeResources();
                GC.SuppressFinalize(disposable);
            }
        }

        /// <summary>
        /// Provides a warning about non-disposed disposable object
        /// </summary>
        /// <param name="disposable">Disposable object</param>
        public static void WarnAboutUsingDispose(this IDisposable disposable)
        {
            Debugger.Log(0, disposable.GetType().FullName,
                         "Dispose should be called");
        }
    }
}
