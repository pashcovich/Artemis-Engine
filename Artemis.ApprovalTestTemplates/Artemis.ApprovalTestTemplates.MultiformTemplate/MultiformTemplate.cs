using Artemis.Engine;
using Artemis.Engine.Fixins;
using Artemis.Engine.Multiforms;

namespace Artemis.ApprovalTestTemplates.MultiformTemplate
{
    public class MultiformTemplate : Multiform
    {
        public MultiformTemplate() : base() { }
        public MultiformTemplate(string name) : base(name) { }

        public override void Construct(MultiformConstructionArgs args)
        {
            SetUpdater(MainUpdate);
            SetRenderer(MainRender);
        }

        private void MainUpdate()
        {
            // Updating code here...
        }

        private void MainRender()
        {
            // Rendering code here...
        }
    }
}
