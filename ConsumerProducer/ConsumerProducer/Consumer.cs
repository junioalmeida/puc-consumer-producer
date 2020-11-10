namespace ProdutorConsumidor {
    class Consumer {

        private readonly Buffer MyBuffer;

        public Consumer(Buffer Buffer) {
            MyBuffer = Buffer;
        }

        public void Consume() {
            while (true)
                MyBuffer.Remove();
        }

    }
}
