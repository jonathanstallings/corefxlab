﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System.IO.Pipelines
{
    internal class ThrowHelper
    {
        public static void ThrowArgumentOutOfRangeException(ExceptionArgument argument)
        {
            throw GetArgumentOutOfRangeException(argument);
        }

        public static void ThrowInvalidOperationException(ExceptionResource resource, string location = null)
        {
            throw GetInvalidOperationException(resource, location);
        }

        public static void ThrowArgumentNullException(ExceptionArgument argument)
        {
            throw GetArgumentNullException(argument);
        }

        public static void ThrowNotSupportedException()
        {
            throw GetNotSupportedException();
        }

        public static void ThrowArgumentOutOfRangeException_BufferRequestTooLarge(int maxSize)
        {
            throw GetArgumentOutOfRangeException_BufferRequestTooLarge(maxSize);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static ArgumentOutOfRangeException GetArgumentOutOfRangeException(ExceptionArgument argument)
        {
            return new ArgumentOutOfRangeException(GetArgumentName(argument));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static InvalidOperationException GetInvalidOperationException(ExceptionResource resource, string location = null)
        {
            return new InvalidOperationException(GetResourceString(resource, location));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static NotSupportedException GetNotSupportedException()
        {
            return new NotSupportedException();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static ArgumentNullException GetArgumentNullException(ExceptionArgument argument)
        {
            return new ArgumentNullException(GetArgumentName(argument));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static ArgumentOutOfRangeException GetArgumentOutOfRangeException_BufferRequestTooLarge(int maxSize)
        {
            return new ArgumentOutOfRangeException(GetArgumentName(ExceptionArgument.size),
                $"Cannot allocate more than {maxSize} bytes in a single buffer");
        }

        private static string GetArgumentName(ExceptionArgument argument)
        {
            Debug.Assert(Enum.IsDefined(typeof(ExceptionArgument), argument),
                "The enum value is not defined, please check the ExceptionArgument Enum.");

            return argument.ToString();
        }

        private static string GetResourceString(ExceptionResource argument, string location = null)
        {
            Debug.Assert(Enum.IsDefined(typeof(ExceptionResource), argument),
                "The enum value is not defined, please check the ExceptionResource Enum.");

            // Should be look up with enviorment resources
            string resourceString = null;
            switch (argument)
            {
                case ExceptionResource.AlreadyWriting:
                    resourceString = "Already writing.";
                    break;
                case ExceptionResource.NotWritingNoAlloc:
                    resourceString = "No writing operation. Make sure Alloc() was called.";
                    break;
                case ExceptionResource.NoWriteToComplete:
                    resourceString = "No writing operation to complete.";
                    break;
                case ExceptionResource.AlreadyReading:
                    resourceString = "Already reading.";
                    break;
                case ExceptionResource.NoReadToComplete:
                    resourceString = "No reading operation to complete.";
                    break;
                case ExceptionResource.NoConcurrentOperation:
                    resourceString = "Concurrent reads or writes are not supported.";
                    break;
                case ExceptionResource.GetResultNotCompleted:
                    resourceString = "Can't GetResult unless completed";
                    break;
                case ExceptionResource.NoWritingAllowed:
                    resourceString = "Writing is not allowed after writer was completed";
                    break;
                case ExceptionResource.NoReadingAllowed:
                    resourceString = "Reading is not allowed after reader was completed";
                    break;
                case ExceptionResource.CompleteWriterActiveWriter:
                    resourceString = "Can't complete writer while writing.";
                    break;
                case ExceptionResource.CompleteReaderActiveReader:
                    resourceString = "Can't complete reader while reading.";
                    break;
            }

            resourceString = resourceString ?? $"Error ResourceKey not defined {argument}.";

            if (location != null)
            {
                resourceString += Environment.NewLine;
                resourceString += "From: " + location.Replace("at ", ">> ");
            }

            return resourceString;
        }
    }

    internal enum ExceptionArgument
    {
        minimumSize,
        bytesWritten,
        destination,
        offset,
        length,
        data,
        size
    }

    internal enum ExceptionResource
    {
        AlreadyWriting,
        NotWritingNoAlloc,
        NoWriteToComplete,
        AlreadyReading,
        NoReadToComplete,
        NoConcurrentOperation,
        GetResultNotCompleted,
        NoWritingAllowed,
        NoReadingAllowed,
        CompleteWriterActiveWriter,
        CompleteReaderActiveReader
    }
}
