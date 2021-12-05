using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

float ComputeSizeInMB(long size)
{
    return (float)size / 1024 / 1024;
}

string fileToCompress = "image.bmp";
byte[] uncompressedBytes = File.ReadAllBytes(fileToCompress);

Stopwatch timer = new Stopwatch();

long uncompressedFileSize = uncompressedBytes.LongLength;
Console.WriteLine($"{fileToCompress} is {ComputeSizeInMB(uncompressedFileSize)}");


//Deflate Optimal
using (MemoryStream compressedStream = new MemoryStream())
{
    using DeflateStream deflateStream = new DeflateStream(compressedStream, CompressionLevel.Optimal, true);
    timer.Start();
    deflateStream.Write(uncompressedBytes, 0, uncompressedBytes.Length);
    timer.Stop();

    long compressedFileSize = compressedStream.Length;
    Console.WriteLine($"Compressed using DeflateStream (Optimal): {ComputeSizeInMB(compressedFileSize)}MB [{100f * (float)compressedFileSize / (float)uncompressedFileSize}%] in {timer.ElapsedMilliseconds}ms");
    timer.Reset();
}

//Deflate Fast
using (MemoryStream compressedStream = new MemoryStream())
{
    using DeflateStream deflateStream = new DeflateStream(compressedStream, CompressionLevel.Fastest, true);
    timer.Start();
    deflateStream.Write(uncompressedBytes, 0, uncompressedBytes.Length);
    timer.Stop();

    long compressedFileSize = compressedStream.Length;
    Console.WriteLine($"Compressed using DeflateStream (Fastest): {ComputeSizeInMB(compressedFileSize)}MB [{100f * (float)compressedFileSize / (float)uncompressedFileSize}%] in {timer.ElapsedMilliseconds}ms");
    timer.Reset();
}

string archiveToSave = fileToCompress + ".gz";
using (MemoryStream compressedStream = new MemoryStream())
{
    using GZipStream gZipStream = new GZipStream(compressedStream, CompressionMode.Compress);
    timer.Start();
    gZipStream.Write(uncompressedBytes, 0, uncompressedBytes.Length);
    timer.Stop();

    long compressedFileSize = compressedStream.Length;
    Console.WriteLine($"Compressed using GZipStream: {ComputeSizeInMB(compressedFileSize)}MB [{100f * (float)compressedFileSize / (float)uncompressedFileSize}%] in {timer.ElapsedMilliseconds}ms");
    timer.Reset();

    using FileStream saveStream = new FileStream(archiveToSave, FileMode.Create);
    compressedStream.Position = 0;
    compressedStream.CopyTo(saveStream);   
}