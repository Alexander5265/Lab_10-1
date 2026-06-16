using System;
using System.IO;

namespace Compiler
{
    internal class Program
    {
        private static void Main()
        {
            string inputFilePath = "TEST.txt";
            string outputFilePath = "Tokens.txt";

            if (!File.Exists(inputFilePath))
            {
                Console.WriteLine($"Ошибка: Файл '{inputFilePath}' не найден!");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("--- ТЕСТ ЛЕКСИЧЕСКОГО АНАЛИЗАТОРА ---\n");

            // Запускаем модуль ввода-вывода
            InputOutput.Init(inputFilePath);
            InputOutput.NextCh(); 

            // Создаем объект анализатора
            LexicalAnalyzer lexer = new LexicalAnalyzer();

            try
            {
                using (StreamWriter tokenFile = new StreamWriter(outputFilePath))
                {
                    Console.WriteLine("Чтение кода и поиск ошибок...\n");
                    
                    while (!InputOutput.IsEoF)
                    {
                        // Получаем код через функцию из шаблона
                        byte tokenCode = lexer.NextSym();
                        
                        if (tokenCode == 0) break; // Файл закончился

                        // Выводим код в консоль и записываем в файл Tokens.txt
                        tokenFile.Write(tokenCode + " ");
                    }
                }
                Console.WriteLine($"\n\nГотово! Коды успешно записаны в {outputFilePath}");
            }
            catch (Exception e)
            {
                Console.WriteLine("\nОшибка: " + e.Message);
            }

            Console.ReadLine();
        }
    }
}