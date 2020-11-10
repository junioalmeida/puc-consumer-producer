using System;
using System.Text;
using System.Threading;

namespace ProdutorConsumidor {
    static class Program {

        private static bool ConsumerSelected = true;
        private static int RowStartBuffer;
        private static int BufferTickness;
        private static int SizeBuffer;

        private static Buffer Buffer;
        private static readonly object SyncConsole = new object();

        static void Main(string[] args) {

            try {
                RowStartBuffer = int.Parse(args[0]);
            } catch (Exception) {
                RowStartBuffer = 6;
            }

            try {
                BufferTickness = int.Parse(args[1]);
            } catch (Exception) {
                BufferTickness = 3;
            }

            try {
                SizeBuffer = int.Parse(args[2]);
            } catch (Exception) {
                SizeBuffer = 15;
            }

            try {
                //Configurações Iniciais essenciais para que a aplicação funcione em qualquer ambiente em várias condições diferentes
                Console.OutputEncoding = Encoding.Unicode;
                Console.WindowHeight = 30;
                Console.WindowWidth = 130;
                Console.BufferWidth = 130;
                Console.BufferHeight = 30;
                Console.CursorVisible = false;
                Console.Title = "Produtor x Consumidor";

                Console.ForegroundColor = ConsoleColor.White;

                Buffer = new Buffer(SizeBuffer, BufferTickness);

                Console.Clear();

                Console.WriteLine("ESC -> Sair do Programa");

                WriteOptions();

                WriteBuffer();

                Console.SetCursorPosition(0, 20);
                Console.WriteLine("Pressione qualquer tecla para produiz...\n");
                Console.WriteLine("Pressione F1 para alterar o tempo do produtor...\n");
                Console.WriteLine("Pressione F2 para alterar o tempo do consumidor...");


                Producer Producer = new Producer(Buffer);
                Thread Consumer = new Thread(new Consumer(Buffer).Consume) {
                    Name = "Consumidor"
                };

                Consumer.Start();

                while (true) {

                    ConsoleKey Key = Console.ReadKey(true).Key;

                    Console.CancelKeyPress += new ConsoleCancelEventHandler(MyHandler);

                    if (Key == ConsoleKey.F1) {

                        ConsumerSelected = false;
                        WriteOptions();

                    } else if (Key == ConsoleKey.F2) {

                        ConsumerSelected = true;
                        WriteOptions();

                    } else if (Key == ConsoleKey.Add) {

                        if (ConsumerSelected)
                            Buffer.TimeConsume += 10;
                        else
                            Buffer.TimeProduce += 10;

                        WriteOptions();

                    } else if (Key == ConsoleKey.Subtract) {

                        if (ConsumerSelected)
                            Buffer.TimeConsume -= 10;
                        else
                            Buffer.TimeProduce -= 10;

                        WriteOptions();
                    } else if (Key == ConsoleKey.Escape) {
                        Console.Clear();
                        Environment.Exit(0);
                    } else
                        Producer.Produce();
                }
            } catch (Exception E) {
                Console.Clear();
                Console.WriteLine("Erro durante a execução...\n\n" +
                    "Message: {0}\n\n" +
                    "- StackTrace\n\n{1}\n\n" +
                    "- Help: {2}", E.Message, E.StackTrace, E.HelpLink);
                Console.ReadKey(true);
            }
        }

        public static void WriteBufferBusy() {
            lock (SyncConsole) {
                Console.SetCursorPosition(5, RowStartBuffer + 10);
                Console.Write("A fila Está cheia...");
            }
        }

        public static void WriteBufferDecrease() {

            lock (SyncConsole) {
                Console.SetCursorPosition(5, RowStartBuffer + 10);
                Console.Write("                    ");
            }
        }

        public static void WriteInBuffer(string Str, int Pos, bool IsProducer) {

            lock (SyncConsole) {
                if (IsProducer) {

                    Console.SetCursorPosition((Pos * (BufferTickness + 3)) + 2, RowStartBuffer + 3);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("{0}", Str);
                    Console.ForegroundColor = ConsoleColor.White;

                    if (Pos == 0) {
                        Console.SetCursorPosition((SizeBuffer - 1) * (BufferTickness + 3) + BufferTickness + 1, RowStartBuffer + 5);
                        Console.Write(" ");

                        Console.SetCursorPosition((SizeBuffer - 1) * (BufferTickness + 3) + BufferTickness + 1, RowStartBuffer + 6);
                        Console.Write(" ");

                    } else {
                        Console.SetCursorPosition((Pos - 1) * (BufferTickness + 3) + BufferTickness + 1, RowStartBuffer + 5);
                        Console.Write(" ");

                        Console.SetCursorPosition((Pos - 1) * (BufferTickness + 3) + BufferTickness + 1, RowStartBuffer + 6);
                        Console.Write(" ");
                    }

                    Console.SetCursorPosition(Pos * (BufferTickness + 3) + BufferTickness + 1, RowStartBuffer + 5);
                    Console.Write("▲");

                    Console.SetCursorPosition(Pos * (BufferTickness + 3) + BufferTickness + 1, RowStartBuffer + 6);
                    Console.Write("P");
                } else {

                    Console.SetCursorPosition((Pos * (BufferTickness + 3)) + 2, RowStartBuffer + 3);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("{0}", Str);
                    Console.ForegroundColor = ConsoleColor.White;

                    if (Pos == 0) {
                        Console.SetCursorPosition((SizeBuffer - 1) * (BufferTickness + 3) + BufferTickness + 1, RowStartBuffer);
                        Console.Write(" ");

                        Console.SetCursorPosition((SizeBuffer - 1) * (BufferTickness + 3) + BufferTickness + 1, RowStartBuffer + 1);
                        Console.Write(" ");

                    } else {
                        Console.SetCursorPosition((Pos - 1) * (BufferTickness + 3) + BufferTickness + 1, RowStartBuffer);
                        Console.Write(" ");

                        Console.SetCursorPosition((Pos - 1) * (BufferTickness + 3) + BufferTickness + 1, RowStartBuffer + 1);
                        Console.Write(" ");
                    }

                    Console.SetCursorPosition(Pos * (BufferTickness + 3) + BufferTickness + 1, RowStartBuffer);
                    Console.Write("C");

                    Console.SetCursorPosition(Pos * (BufferTickness + 3) + BufferTickness + 1, RowStartBuffer + 1);
                    Console.Write("▼");

                }
            }
        }

        public static void WriteBuffer() {

            lock (SyncConsole) {
                Console.SetCursorPosition(0, RowStartBuffer + 2);
                Console.Write("┌");
                Console.SetCursorPosition(0, RowStartBuffer + 3);
                Console.Write("│");
                Console.SetCursorPosition(0, RowStartBuffer + 4);
                Console.Write("└");

                for (int i = 1; i < SizeBuffer * (BufferTickness + 3); i += BufferTickness + 3) {
                    //218)┌  196)─  191)┐  179)│  192)└  217)┘  194)┬  193)┴

                    Console.SetCursorPosition(i, RowStartBuffer + 2);
                    Console.Write("{0}┬", new string('─', BufferTickness + 2));
                    Console.SetCursorPosition(i, RowStartBuffer + 3);
                    Console.Write("{0}│", new string(' ', BufferTickness + 2));
                    Console.SetCursorPosition(i, RowStartBuffer + 4);
                    Console.Write("{0}┴", new string('─', BufferTickness + 2));
                }

                Console.SetCursorPosition(SizeBuffer * (BufferTickness + 3), RowStartBuffer + 2);
                Console.Write("┐");
                Console.SetCursorPosition(SizeBuffer * (BufferTickness + 3), RowStartBuffer + 3);
                Console.Write("│");
                Console.SetCursorPosition(SizeBuffer * (BufferTickness + 3), RowStartBuffer + 4);
                Console.Write("┘");
            }
        }

        static void WriteOptions() {

            lock (SyncConsole) {
                if (ConsumerSelected) {
                    Console.ForegroundColor = ConsoleColor.White;

                    Console.SetCursorPosition(0, 1);
                    Console.Write("\nF1 - Tempo do Produtor....: {0,5} ms     [Use (+)Incrementar (-)Decrementar]", Buffer.TimeProduce);

                    Console.ForegroundColor = ConsoleColor.Blue;

                    Console.SetCursorPosition(0, 2);
                    Console.Write("\nF2 - Tempo do Consumidor..: {0,5} ms     [Use (+)Incrementar (-)Decrementar]", Buffer.TimeConsume);
                } else {
                    Console.ForegroundColor = ConsoleColor.Blue;

                    Console.SetCursorPosition(0, 1);
                    Console.Write("\nF1 - Tempo do Produtor....: {0,5} ms     [Use (+)Incrementar (-)Decrementar]", Buffer.TimeProduce);

                    Console.ForegroundColor = ConsoleColor.White;

                    Console.SetCursorPosition(0, 2);
                    Console.Write("\nF2 - Tempo do Consumidor..: {0,5} ms     [Use (+)Incrementar (-)Decrementar]", Buffer.TimeConsume);
                }

                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        private static void MyHandler(object sender, ConsoleCancelEventArgs args) {
            args.Cancel = true;
        }
    }
}
