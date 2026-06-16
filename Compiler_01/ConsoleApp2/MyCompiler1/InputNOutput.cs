using System;
using System.Collections.Generic;
using System.IO;

namespace Compiler
{
    // Структура для хранения позиции (номер строки и номер символа)
    public struct TextPosition
    {
        public uint LineNumber;
        public byte CharNumber;
        public TextPosition(uint line, byte ch) { LineNumber = line; CharNumber = ch; }
    }

    // Структура для хранения самой ошибки (где произошла и какой у нее код)
    public struct Err
    {
        public TextPosition Position;
        public byte Code;
        public Err(TextPosition pos, byte code) { Position = pos; Code = code; }
    }

    public class InputOutput
    {
        public static char Ch { get; private set; } // Символ, который мы читаем прямо сейчас
        public static TextPosition PositionNow;     // Где мы сейчас находимся в тексте
        public static bool IsEoF { get; private set; } // Флаг: закончился ли файл?

        private static StreamReader fileReader;
        private static string currentLine = "";
        private static int lastInLine = 0;
        private static List<Err> errors = new List<Err>();
        private static int totalErrors = 0;

        // Метод для подготовки: открываем файл и сбрасываем счетчики
        public static void Init(string filePath)
        {
            fileReader = new StreamReader(filePath);
            PositionNow = new TextPosition(0, 0);
            IsEoF = false;
            totalErrors = 0;
        }

        // Главный метод, который "двигает" чтение на один символ вперед
        public static void NextCh()
        {
            if (IsEoF) return; // Если файл кончился, ничего не делаем

            // Если мы только запустились или дошли до конца текущей строки
            if (PositionNow.LineNumber == 0 || PositionNow.CharNumber >= lastInLine)
            {
                // Печатаем предыдущую строку и все ошибки, которые в ней нашли
                if (PositionNow.LineNumber > 0)
                {
                    Console.WriteLine(currentLine);
                    PrintErrors();
                }

                // Проверяем, не закончился ли файл
                if (fileReader.EndOfStream)
                {
                    IsEoF = true;
                    Console.WriteLine($"\nКомпиляция завершена. Всего ошибок: {totalErrors}");
                    fileReader.Close();
                    return;
                }

                // Считываем новую строку. Пробел в конце нужен, чтобы не выйти за границы массива
                currentLine = fileReader.ReadLine() + " ";
                lastInLine = currentLine.Length - 1;
                errors.Clear(); // Строка новая — ошибок пока нет

                PositionNow.LineNumber++;
                PositionNow.CharNumber = 0;
            }
            else
            {
                // Если мы посередине строки, просто шагаем на один символ вправо
                PositionNow.CharNumber++;
            }

            // Записываем текущий символ в свойство Ch, чтобы Program.cs мог его проверить
            Ch = currentLine[PositionNow.CharNumber];
        }

        // Метод для регистрации ошибки
        public static void Error(byte errorCode)
        {
            if (errors.Count < 9) // Ограничение количества ошибок на одну строку
            {
                // Запоминаем текущую позицию и код ошибки
                errors.Add(new Err(PositionNow, errorCode));
                totalErrors++;
            }
        }

        // Метод для вывода ошибок (рисует стрелочки ^ под нужными символами)
        private static void PrintErrors()
        {
            foreach (var err in errors)
            {
                int padding = err.Position.CharNumber;
                string prefix = "** ";
        
                if (padding >= prefix.Length)
                {
                    // Вычитаем 3 символа префикса из отступа
                    string spaces = new string(' ', padding - prefix.Length);
                    Console.WriteLine($"{prefix}{spaces}^ ошибка код {err.Code}");
                }
                else
                {
                    // Если ошибка в самом начале строки
                    string spaces = new string(' ', padding);
                    Console.WriteLine($"{spaces}^ ошибка код {err.Code}");
                }
            }
        }
    }
}