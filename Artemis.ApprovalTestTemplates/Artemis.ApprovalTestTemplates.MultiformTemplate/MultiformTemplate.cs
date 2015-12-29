using Artemis.Engine;

namespace Artemis.ApprovalTestTemplates.MultiformTemplate
{
    public class MultiformTemplate : Multiform
    {
        public MultiformTemplate() : base() { }
        public MultiformTemplate(string name) : base(name) { }

        public override void Construct()
        {
            SetUpdater(Update);
            SetRenderer(Render);
        }

        private void Update()
        {
            // Updating code here...
        }

        private void Render()
        {
            // Rendering code here...
        }
    }
}
