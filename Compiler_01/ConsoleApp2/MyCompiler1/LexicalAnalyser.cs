using System;
using System.Collections.Generic;

namespace Compiler
{
    public class LexicalAnalyzer
    {
        // Все константы из твоего шаблона
        public const byte
            star = 21, slash = 60, equal = 16, comma = 20, semicolon = 14,
            colon = 5, point = 61, arrow = 62, leftpar = 9, rightpar = 4,
            lbracket = 11, rbracket = 12, flpar = 63, frpar = 64, later = 65,
            greater = 66, laterequal = 67, greaterequal = 68, latergreater = 69,
            plus = 70, minus = 71, lcomment = 72, rcomment = 73, assign = 51,
            twopoints = 74, ident = 2, floatc = 82, intc = 15, casesy = 31,
            elsesy = 32, filesy = 57, gotosy = 33, thensy = 52, typesy = 34,
            untilsy = 53, dosy = 54, withsy = 37, ifsy = 56, insy = 100,
            ofsy = 101, orsy = 102, tosy = 103, endsy = 104, varsy = 105,
            divsy = 106, andsy = 107, notsy = 108, forsy = 109, modsy = 110,
            nilsy = 111, setsy = 112, beginsy = 113, whilesy = 114, arraysy = 115,
            constsy = 116, labelsy = 117, downtosy = 118, packedsy = 119,
            recordsy = 120, repeatsy = 121, programsy = 122, functionsy = 123,
            procedurensy = 124;

        public byte symbol; // код текущего символа/токена
        public TextPosition token; // позиция начала токена
        public string addrName; // строка с именем идентификатора
        public int nmb_int; // значение целой константы

        // Таблица для быстрого поиска ключевых слов (Задание 1)
        private Dictionary<string, byte> keywords = new Dictionary<string, byte>(StringComparer.OrdinalIgnoreCase)
        {
            { "program", programsy },
            { "var", varsy },
            { "begin", beginsy },
            { "end", endsy },
            { "if", ifsy },
            { "then", thensy }
        };

        public byte NextSym()
        {
            // Пропускаем пробелы, табы и переносы строк
            while (!InputOutput.IsEoF && char.IsWhiteSpace(InputOutput.Ch))
            {
                InputOutput.NextCh();
            }

            if (InputOutput.IsEoF) return 0; // 0 означает конец работы

            // Фиксируем позицию начала токена
            token.LineNumber = InputOutput.PositionNow.LineNumber;
            token.CharNumber = InputOutput.PositionNow.CharNumber;

            // 1. Реализация <цифра> из шаблона
            if (char.IsDigit(InputOutput.Ch))
            {
                int maxint = 32767; // Предел для integer в Паскале
                nmb_int = 0;
                
                while (!InputOutput.IsEoF && char.IsDigit(InputOutput.Ch))
                {
                    byte digit = (byte)(InputOutput.Ch - '0');
                    
                    // Проверка на выход за пределы
                    if (nmb_int < maxint / 10 || (nmb_int == maxint / 10 && digit <= maxint % 10))
                    {
                        nmb_int = 10 * nmb_int + digit;
                    }
                    else
                    {
                        InputOutput.Error(203); // Код ошибки превышения лимита из шаблона
                        nmb_int = 0;
                        // Пропускаем остальные цифры этого длинного числа
                        while (!InputOutput.IsEoF && char.IsDigit(InputOutput.Ch)) InputOutput.NextCh();
                        break; 
                    }
                    InputOutput.NextCh();
                }
                symbol = intc;
                return symbol;
            }

            // 2. Реализация <буква> из шаблона (Идентификаторы и ключевые слова)
            if (char.IsLetter(InputOutput.Ch))
            {
                addrName = "";
                while (!InputOutput.IsEoF && char.IsLetterOrDigit(InputOutput.Ch))
                {
                    addrName += InputOutput.Ch;
                    InputOutput.NextCh();
                }

                // Ищем в словаре: если это ключевое слово, берем его код, иначе это обычный идентификатор
                if (keywords.ContainsKey(addrName))
                {
                    symbol = keywords[addrName];
                }
                else
                {
                    symbol = ident;
                }
                return symbol;
            }

            // 3. Сканирование символов
            switch (InputOutput.Ch)
            {
                case '<':
                    InputOutput.NextCh();
                    if (InputOutput.Ch == '=') { symbol = laterequal; InputOutput.NextCh(); }
                    else if (InputOutput.Ch == '>') { symbol = latergreater; InputOutput.NextCh(); }
                    else symbol = later;
                    break;
                case '>':
                    InputOutput.NextCh();
                    if (InputOutput.Ch == '=') { symbol = greaterequal; InputOutput.NextCh(); }
                    else symbol = greater;
                    break;
                case ':':
                    InputOutput.NextCh();
                    if (InputOutput.Ch == '=') { symbol = assign; InputOutput.NextCh(); }
                    else symbol = colon;
                    break;
                case ';': symbol = semicolon; InputOutput.NextCh(); break;
                case '.':
                    InputOutput.NextCh();
                    if (InputOutput.Ch == '.') { symbol = twopoints; InputOutput.NextCh(); }
                    else symbol = point;
                    break;
                case '=': symbol = equal; InputOutput.NextCh(); break;
                case '+': symbol = plus; InputOutput.NextCh(); break;
                case '-': symbol = minus; InputOutput.NextCh(); break;
                case '*': symbol = star; InputOutput.NextCh(); break;
                case '/': symbol = slash; InputOutput.NextCh(); break;
                case ',': symbol = comma; InputOutput.NextCh(); break;
                case '(': symbol = leftpar; InputOutput.NextCh(); break;
                case ')': symbol = rightpar; InputOutput.NextCh(); break;
                case '[': symbol = lbracket; InputOutput.NextCh(); break;
                case ']': symbol = rbracket; InputOutput.NextCh(); break;
                case '{': symbol = flpar; InputOutput.NextCh(); break;
                case '}': symbol = frpar; InputOutput.NextCh(); break;
                default:
                    // Если символ не распознан
                    InputOutput.Error(99); 
                    InputOutput.NextCh();
                    return NextSym(); // Запрашиваем следующий символ рекурсивно
            }

            return symbol;
        }
    }
}