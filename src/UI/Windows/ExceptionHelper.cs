﻿using System.Reflection;

namespace TerminalGuiDesigner.UI.Windows;

/// <summary>
/// Helper for unwrapping Exception.InnerExceptions and ReflectionTypeLoadExceptions.LoaderExceptions into a single flat message string of all errors.
/// </summary>
public static class ExceptionHelper
{
    /// <summary>
    /// Unpacks inner exceptions into a single <see cref="string"/>.
    /// </summary>
    /// <param name="e"><see cref="Exception"/> to unpack.</param>
    /// <param name="includeStackTrace">True to include <see cref="Exception.StackTrace"/> in result.</param>
    /// <returns>Exception and all inner exceptions in user consumable string.</returns>
    public static string ExceptionToListOfInnerMessages(Exception e, bool includeStackTrace = false)
    {
        string message = e.Message;
        if (includeStackTrace)
        {
            message += Environment.NewLine + e.StackTrace;
        }

        if (e is ReflectionTypeLoadException)
        {
            foreach (Exception? loaderException in ((ReflectionTypeLoadException)e).LoaderExceptions)
            {
                if (loaderException != null)
                {
                    message += Environment.NewLine + ExceptionToListOfInnerMessages(loaderException, includeStackTrace);
                }
            }
        }

        if (e.InnerException != null)
        {
            message += Environment.NewLine + ExceptionToListOfInnerMessages(e.InnerException, includeStackTrace);
        }

        return message;
    }

    /// <summary>
    /// Returns the first InnerException of type T in the Exception or null.
    ///
    /// <para>If e is T then e is returned directly</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="e"></param>
    /// <returns></returns>
    public static T? GetExceptionIfExists<T>(this Exception e)
        where T : Exception
    {
        if (e is T)
        {
            return (T)e;
        }

        if (e.InnerException != null)
        {
            return GetExceptionIfExists<T>(e.InnerException);
        }

        return null;
    }
}
