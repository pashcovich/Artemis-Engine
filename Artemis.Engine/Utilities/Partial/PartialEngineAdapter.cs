#region Using Statements

using Artemis.Engine.Utilities.Dynamics;

#endregion

namespace Artemis.Engine.Utilities.Partial
{
    /// <summary>
    /// An object that adapts to a partial engine environment.
    /// 
    /// The term "partial engine environment" refers to a runtime environment where
    /// the ArtemisEngine is not fully running. This is useful because it allows certain
    /// objects to still be able to function even when the engine is not itself functional.
    /// 
    /// For example, PartialEngineAdapter allows us to disable functionality that requires
    /// the global GameTimer (ArtemisEngine.GameTimer) because in certain runtime environments 
    /// that value may be null.
    /// </summary>
    public class PartialEngineAdapter : DynamicPropertyCollection
    {

        private static AttributeMemoService<PartialEngineAdapter> attrMemoService
            = new AttributeMemoService<PartialEngineAdapter>();

        static PartialEngineAdapter()
        {
            var handler = new AttributeMemoService<PartialEngineAdapter>.AttributeHandler(o => o.IsPartial = true);
            attrMemoService.RegisterHandler<PartialEngineAttribute>(handler);
        }

        /// <summary>
        /// Whether or not this object is running in a partial engine.
        /// </summary>
        protected bool IsPartial { get; private set; }

        public PartialEngineAdapter()
            : base()
        {
            attrMemoService.Handle(this);
        }

    }
}
