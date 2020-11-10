namespace ProdutorConsumidor {
    class Producer {

        private readonly Buffer MyBuffer;

        public Producer(Buffer Buffer) {
            MyBuffer = Buffer;
        }

        public void Produce() {
            MyBuffer.Add();
        }
    }
}
