#region Using Statements

using Artemis.Engine.Utilities.Dynamics;

#endregion

namespace Artemis.Engine.Persistence
{
    public static class UserOptions
    {
        public static class Builtins
        {
            public const string Fullscreen = "Fullscreen";
            public const string MouseVisible = "MouseVisible";
            public const string Borderless = "Borderless";
            public const string VSync = "VSync";
            public const string FrameRate = "FrameRate";
            public const string Resolution = "Resolution";
        }

        public const string DefaultOptionsFileName = "user_options.xml";

        public static string OptionFileName { get; private set; }

        private static OptionRecordService optionRecordService = new OptionRecordService();

        static UserOptions()
        {
            OptionFileName = DefaultOptionsFileName;
        }

        public static void SetFileName(string fileName)
        {
            OptionFileName = fileName;
        }

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

        public static void AddOption(AbstractOptionRecord record)
        {
            optionRecordService.AddOptionRecord(record);
        }

        public static void AddOption(AbstractOptionRecord record, Getter getter, Setter setter)
        {
            optionRecordService.AddOptionRecord(record, getter, setter);
        }

        public static void AddOptionChangedEventHandler(string record, OptionChangedEventHandler handler)
        {
            optionRecordService.AddOptionChangedEventHandler(record, handler);
        }

        // Should this really be public?
        public static void Read()
        {
            optionRecordService.Read(OptionFileName);
        }

        public static void Write()
        {
            optionRecordService.Write(OptionFileName);
        }
    }
}
