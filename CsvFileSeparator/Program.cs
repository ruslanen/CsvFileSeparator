﻿using System;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace CsvFileSeparator
{
    public class CsvFileSeparator
    {
        /// <summary>
        /// Путь до файла
        /// </summary>
        private readonly string _filePath;
        
        /// <summary>
        /// Количество файлов, на которое будет разделен исходный файл
        /// </summary>
        private readonly int _filesCount;

        /// <summary>
        /// Признак наличия строки заголовков в исходном файле
        /// </summary>
        private readonly bool _hasHeaders;

        /// <summary>
        /// Признак необходимости добавления строки заголовков в разделенные файлы
        /// </summary>
        private readonly bool _needHeadersInOutput;

        public CsvFileSeparator(string filePath, int filesCount, bool hasHeaders, bool needHeadersInOutput)
        {
            _filePath = filePath;
            _filesCount = filesCount;
            _hasHeaders = hasHeaders;
            _needHeadersInOutput = needHeadersInOutput;
        }
        
        public void Process(BackgroundWorker backgroundWorker = null)
        {
            var fileName = Path.GetFileNameWithoutExtension(_filePath);
            using (var streamReader = new StreamReader(_filePath, new UTF8Encoding(false)))
            {
                var linesCount = 0;
                while (streamReader.ReadLine() != null)
                {
                    linesCount++;
                }

                streamReader.BaseStream.Seek(0, SeekOrigin.Begin);

                if (_hasHeaders)
                {
                    linesCount--;
                }

                // После этого чтения не сбрасываем указатель позиции в потоке,
                // т.к. потом заголовки явно будут дописываться в новый поток  
                var headersLine = streamReader.ReadLine();

                var filePartLines = linesCount / _filesCount; // limit
                var lastFileLines = linesCount % _filesCount == 0
                    ? filePartLines
                    : filePartLines + linesCount % _filesCount;

                var startAtLine = 0;
                for (var i = 1; i <= _filesCount; i++)
                {
                    if (i == _filesCount)
                    {
                        filePartLines = lastFileLines;
                    }

                    BuildFile(streamReader, i, startAtLine, filePartLines, headersLine, fileName);
                    startAtLine *= i;

                    backgroundWorker?.ReportProgress(i);
                }
            }
        }
        
        void BuildFile(StreamReader streamReader, int fileNumber, int offset, int limit, string header, string fileName)
        {
            using (var memoryStream = new MemoryStream())
            using (var textWriter = new StreamWriter(memoryStream, new UTF8Encoding(false), 4096, true))
            {
                if (_needHeadersInOutput)
                {
                    textWriter.WriteLine(header);
                }

                for (var i = offset; i < limit; i++)
                {
                    textWriter.WriteLine(streamReader.ReadLine());
                }

                textWriter.Flush();
                memoryStream.Seek(0, SeekOrigin.Begin);

                using (var fileStream = new FileStream($"{fileName}_part_{fileNumber}.csv", FileMode.Create))
                {
                    memoryStream.CopyTo(fileStream);
                    fileStream.Flush();
                }
            }
        }
    }
}