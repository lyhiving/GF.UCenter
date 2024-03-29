﻿

namespace GF.UCenter.Common
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Text;

    ////////////////////////////////////////////////////////////////
    ///
    /// This class copied from ObjectDumper project inorder to add
    /// DumpToAttribute support.
    /// This attribute will hidden the sensitive inforation like 
    /// 'Password', 'SupperPassword' in log.
    ///
    ////////////////////////////////////////////////////////////////

    /// <summary>
    ///     This class adds extension methods to all types to facilitate dumping of
    ///     object contents to various outputs.
    /// </summary>
    public static class ObjectDumperExtensions
    {
        /// <summary>
        ///     Dumps the contents of the specified <paramref name="value" /> to the
        ///     <see cref="Debug" /> output.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of object to dump.
        /// </typeparam>
        /// <param name="value">
        ///     The object to dump.
        /// </param>
        /// <param name="name">
        ///     The name to give to the object in the dump;
        ///     or <c>null</c> to use a generated name.
        /// </param>
        /// <returns>
        ///     The <paramref name="value" />, to facilitate easy usage in expressions and method calls.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <para><paramref name="name" /> is <c>null</c> or empty.</para>
        /// </exception>
        public static T Dump<T>(this T value, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            using (var writer = new DebugWriter())
            {
                return Dump(value, name, writer);
            }
        }

        /// <summary>
        ///     Dumps the contents of the specified <paramref name="value" /> to a file
        ///     with the specified <paramref name="filename" />.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of object to dump.
        /// </typeparam>
        /// <param name="value">
        ///     The object to dump.
        /// </param>
        /// <param name="name">
        ///     The name to give to the object in the dump;
        ///     or <c>null</c> to use a generated name.
        /// </param>
        /// <param name="filename">
        ///     The full path to and name of the file to dump the object contents to.
        /// </param>
        /// <returns>
        ///     The <paramref name="value" />, to facilitate easy usage in expressions and method calls.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <para><paramref name="name" /> is <c>null</c> or empty.</para>
        ///     <para>- or -</para>
        ///     <para><paramref name="filename" /> is <c>null</c> or empty.</para>
        /// </exception>
        public static T Dump<T>(this T value, string name, string filename)
        {
            // Error-checking in called method

            return Dump(value, filename, name, Encoding.Default);
        }

        /// <summary>
        ///     Dumps the contents of the specified <paramref name="value" /> to a file
        ///     with the specified <paramref name="filename" />.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of object to dump.
        /// </typeparam>
        /// <param name="value">
        ///     The object to dump.
        /// </param>
        /// <param name="name">
        ///     The name to give to the object in the dump;
        ///     or <c>null</c> to use a generated name.
        /// </param>
        /// <param name="filename">
        ///     The full path to and name of the file to dump the object contents to.
        /// </param>
        /// <param name="encoding">
        ///     The <see cref="Encoding" /> to use for the file.
        /// </param>
        /// <returns>
        ///     The <paramref name="value" />, to facilitate easy usage in expressions and method calls.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <para><paramref name="name" /> is <c>null</c> or empty.</para>
        ///     <para>- or -</para>
        ///     <para><paramref name="filename" /> is <c>null</c> or empty.</para>
        ///     <para>- or -</para>
        ///     <para><paramref name="encoding" /> is <c>null</c></para>
        /// </exception>
        public static T Dump<T>(this T value, string name, string filename, Encoding encoding)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");
            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentNullException("filename");
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            using (var writer = new StreamWriter(filename, false, encoding))
            {
                return Dump(value, name, writer);
            }
        }

        /// <summary>
        ///     Dumps the contents of the specified <paramref name="value" /> to
        ///     the specified <paramref name="writer" />.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of object to dump.
        /// </typeparam>
        /// <param name="value">
        ///     The object to dump.
        /// </param>
        /// <param name="name">
        ///     The name to give to the object in the dump;
        ///     or <c>null</c> to use a generated name.
        /// </param>
        /// <param name="writer">
        ///     The <see cref="TextWriter" /> to dump the object contents to.
        /// </param>
        /// <returns>
        ///     The <paramref name="value" />, to facilitate easy usage in expressions and method calls.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <para><paramref name="name" /> is <c>null</c> or empty.</para>
        ///     <para>- or -</para>
        ///     <para><paramref name="writer" /> is <c>null</c></para>
        /// </exception>
        public static T Dump<T>(this T value, string name, TextWriter writer)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");
            if (writer == null)
                throw new ArgumentNullException("writer");

            Dumper.Dump(value, name, writer);

            return value;
        }

        /// <summary>
        ///     Dumps the contents of the specified <paramref name="value" /> and
        ///     returns the dumped contents as a string.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of object to dump.
        /// </typeparam>
        /// <param name="value">
        ///     The object to dump.
        /// </param>
        /// <param name="name">
        ///     The name to give to the object in the dump;
        ///     or <c>null</c> to use a generated name.
        /// </param>
        /// <returns>
        ///     The dumped contents of the object.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <para><paramref name="name" /> is <c>null</c> or empty.</para>
        /// </exception>
        public static string DumpToString<T>(this T value, string name)
        {
            // Error-checking in called method

            using (var writer = new StringWriter(CultureInfo.InvariantCulture))
            {
                Dump(value, name, writer);
                return writer.ToString();
            }
        }
    }
}