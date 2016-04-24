
namespace Artemis.Engine.Persistence
{
    public static class UserOptions
    {
        private static OptionRecordService optionRecordService = new OptionRecordService();

        public static T Get<T>(string name)
        {
            return optionRecordService.Get<T>(name);
        }

        public static object Get(string name)
        {
            return optionRecordService.Get(name);
        }

        public static void Set<T>(string name, T val)
        {
            optionRecordService.Set(name, val);
        }

        public static void Set(string name, object val)
        {
            optionRecordService.Set(name, val);
        }

        public static void AddRecord(AbstractOptionRecord record)
        {
            optionRecordService.AddOptionRecord(record);
        }

        public static void Read(string fileName)
        {
            optionRecordService.Read(fileName);
        }

        public static void Write(string fileName)
        {
            optionRecordService.Write(fileName);
        }
    }
}
