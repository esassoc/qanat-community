using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Qanat.API.Services;
using Qanat.Common;

namespace Qanat.API.GeoSpatial
{
    public static class FileStreamHelpers
    {
        public static async Task CreateFileStreamFromRequestBodyReader(PipeReader pipeReader, FileStream fileStream)
        {
            while (true)
            {
                // reading data from a pipe instance
                var result = await pipeReader.ReadAsync();
                var buffer = result.Buffer;

                // We perform calculations with the data obtained.
                await ProcessBytesAsync(buffer, fileStream);

                // indicate to which position the data was processed. In this case, everything is written to the file.
                // in situations where not all data has been processed, you need to create a position manually using the buffer and index
                // in this situation, IBytesProcessor.ProcessBytesAsync can be supplemented by returning this position
                pipeReader.AdvanceTo(buffer.End);

                // if PipeWriter has been completed, reading is no longer necessary
                // this behavior was chosen by me as an example, it depends on business logic
                if (result.IsCompleted)
                {
                    break;
                }
            }

            // complete _pipeReader to complete the entire instance of pipe
            await pipeReader.CompleteAsync();
            fileStream.Position = 0;
        }

        public static async Task CreateStreamFromRequestBodyReader(PipeReader pipeReader, Stream memoryStream)
        {
            while (true)
            {
                // reading data from a pipe instance
                var result = await pipeReader.ReadAsync();
                var buffer = result.Buffer;

                // We perform calculations with the data obtained.
                await ProcessBytesAsync(buffer, memoryStream);

                // indicate to which position the data was processed. In this case, everything is written to the file.
                // in situations where not all data has been processed, you need to create a position manually using the buffer and index
                // in this situation, IBytesProcessor.ProcessBytesAsync can be supplemented by returning this position
                pipeReader.AdvanceTo(buffer.End);

                // if PipeWriter has been completed, reading is no longer necessary
                // this behavior was chosen by me as an example, it depends on business logic
                if (result.IsCompleted)
                {
                    break;
                }
            }

            // complete _pipeReader to complete the entire instance of pipe
            await pipeReader.CompleteAsync();
            memoryStream.Position = 0;
        }

        private static Task ProcessBytesAsync(ReadOnlySequence<byte> bytesSequence, Stream fileStream)
        {
            if (bytesSequence.IsSingleSegment)
            {
                ProcessSingle(bytesSequence.First.Span, fileStream);
            }
            else
            {
                foreach (var segment in bytesSequence)
                {
                    ProcessSingle(segment.Span, fileStream);
                }
            }

            return Task.CompletedTask;
        }

        private static void ProcessSingle(ReadOnlySpan<byte> span, Stream fileStream)
        {
            fileStream.Write(span);
        }


        public static async Task<byte[]> RequestBodyReaderToByteArray(PipeReader reader)
        {
            do
            {
                ReadResult readResult = await reader.ReadAsync();
                if (readResult.IsCompleted || readResult.IsCanceled)
                {
                    return readResult.Buffer.ToArray();
                }

                // consume nothing, keep reading from the pipe reader until all data is there
                reader.AdvanceTo(readResult.Buffer.Start, readResult.Buffer.End);
            } while (true);
        }

        public static async Task StreamToFile(Stream stream, string filePath)
        {
            await File.WriteAllBytesAsync(filePath, await StreamToBytes(stream));
        }

        public static async Task<byte[]> StreamToBytes(Stream stream)
        {
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            return ms.ToArray();
        }

        public static async Task<byte[]> StreamToBytes(IFormFile stream)
        {
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            return ms.ToArray();
        }

        public static async Task<DisposableTempFile> GetUploadedGdbFileContents(FileService _fileService, string canonicalName)
        {
            var uploadedGdbFileContents = await _fileService.GetFileStreamFromBlobStorage(canonicalName);
            var disposableTempFile = DisposableTempFile.MakeDisposableTempFileEndingIn(".gdb.zip");
            await using var fileStream = new FileStream(disposableTempFile.FileInfo.FullName, FileMode.Create);
            await uploadedGdbFileContents.CopyToAsync(fileStream);
            return disposableTempFile;
        }
    }
}