using System;
using System.Threading;

namespace ProdutorConsumidor {

    class Buffer {

        public string[] Array { get; private set; }
        public int Busy { get; private set; }
        public string[] StrProduce { get; private set; }
        public string[] StrConsume { get; private set; }

        private readonly object SyncBuffer;

        private int _TimeProduce;

        public int TimeProduce {
            get { return _TimeProduce; }
            set {
                _TimeProduce = value;

                if (_TimeProduce < 0)
                    _TimeProduce = 0;
            }
        }

        private int _TimeConsume;

        public int TimeConsume {
            get { return _TimeConsume; }
            set {
                _TimeConsume = value;

                if (_TimeConsume < 0)
                    _TimeConsume = 0;
            }
        }

        private int CurrentConsumerPos;
        private int CurrentProducerPos;

        public Buffer(int Tam, char[] Produce, char[] Consume, int BufferTickness = 1) {

            Array = new string[Tam];
            Busy = 0;

            CurrentConsumerPos = 0;
            CurrentProducerPos = 0;

            SyncBuffer = new object();

            StrProduce = new string[Produce.Length];

            for (int i = 0; i < StrProduce.Length; i++)
                StrProduce[i] = new string(Produce[i], BufferTickness);

            StrConsume = new string[Consume.Length];

            for (int i = 0; i < StrConsume.Length; i++)
                StrConsume[i] = new string(Consume[i], BufferTickness);

            TimeProduce = 0;
            TimeConsume = 500;
        }

        public Buffer(int Tam, int BufferTickness = 1) :
            this(Tam, new char[] { '█' }, new char[] { '▓', '▒', '░', ' ' }, BufferTickness) { }

        public void Add() {

            bool TakenLock = false;

            try {

                Monitor.Enter(SyncBuffer, ref TakenLock);

                if (!(Busy >= Array.Length)) {

                    Monitor.Exit(SyncBuffer);
                    TakenLock = false;

                    for (int i = 0; i < StrProduce.Length; i++) {
                        Monitor.Enter(SyncBuffer, ref TakenLock);

                        Array[CurrentProducerPos] = StrProduce[i];

                        Monitor.Exit(SyncBuffer);
                        TakenLock = false;

                        Program.WriteInBuffer(Array[CurrentProducerPos], CurrentProducerPos, true);

                        if (TimeProduce > 0)
                            Thread.Sleep(TimeProduce / StrProduce.Length);
                    }


                    if (CurrentProducerPos > Array.Length - 2)
                        CurrentProducerPos = 0;
                    else
                        CurrentProducerPos++;

                    Monitor.Enter(SyncBuffer, ref TakenLock);
                    Busy++;
                }

                if (Busy >= Array.Length)
                    Program.WriteBufferBusy();
            } finally {

                if (TakenLock)
                    Monitor.Exit(SyncBuffer);
            }
        }

        public void Remove() {

            bool TakenLock = false;

            try {

                Monitor.Enter(SyncBuffer, ref TakenLock);

                if (!(Busy == 0)) {

                    Monitor.Exit(SyncBuffer);
                    TakenLock = false;

                    for (int i = 0; i < StrConsume.Length; i++) {

                        Monitor.Enter(SyncBuffer, ref TakenLock);

                        Array[CurrentConsumerPos] = StrConsume[i];

                        Monitor.Exit(SyncBuffer);
                        TakenLock = false;

                        Program.WriteInBuffer(Array[CurrentConsumerPos], CurrentConsumerPos, false);

                        if (TimeConsume > 0)
                            Thread.Sleep(TimeConsume / StrConsume.Length);
                    }

                    if (CurrentConsumerPos > Array.Length - 2)
                        CurrentConsumerPos = 0;
                    else
                        CurrentConsumerPos++;


                    Monitor.Enter(SyncBuffer, ref TakenLock);
                    Busy--;

                    if (Busy >= Array.Length - 1)
                        Program.WriteBufferDecrease();
                }
            } finally {

                if (TakenLock)
                    Monitor.Exit(SyncBuffer);
            }
        }
    }
}
