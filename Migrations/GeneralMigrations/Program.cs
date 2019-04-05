namespace GeneralMigrations
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var master = new GeneralMigrationsMaster();
            master.Start().Wait();
        }
    }
}
