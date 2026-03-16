namespace Core
{
    public static class Input
    {
        public static Inputs Get { get; private set; }
        public static void Reset() { Get = new Inputs(); }
    }
}