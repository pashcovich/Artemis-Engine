#region Using Statements

using Artemis.Engine.Utilities;

#endregion

namespace Artemis.Engine
{
    public class ResolutionRelativeObject : ArtemisObject
    {

        private static AttributeMemoService<ResolutionRelativeObject> attrMemoService
            = new AttributeMemoService<ResolutionRelativeObject>();

        static ResolutionRelativeObject()
        {
            attrMemoService.RegisterHandler<ResolutionChangeListener>(t =>
                {
                    ArtemisEngine.DisplayManager.RegisterResolutionChangeListener(t);
                    t.ListeningToResolutionChanges = true;
                });
        }

        public bool ListeningToResolutionChanges { get; private set; }

        public bool UseScreenRelativePositioning { get; set; } // CURRENTLY NOT IN USE

        /// <summary>
        /// Whether or not to maintain the aspect ratio of this object upon dynamically
        /// scaling it to match the current resolution.
        /// </summary>
        public bool MaintainAspectRatio { get; set; }

        /// <summary>
        /// Determines how to dynamically scale the object to match the current resolution.
        /// For more information on the definition of individual values, see ResolutionScaleType.
        /// </summary>
        public ResolutionScaleType ScaleType { get; set; }

        /// <summary>
        /// The ResolutionScaleRules as a single object.
        /// </summary>
        public ResolutionScaleRules ScaleRules
        {
            get
            {
                var rules = new ResolutionScaleRules();
                rules.UseScreenRelativePositioning = UseScreenRelativePositioning;
                rules.MaintainAspectRatio = MaintainAspectRatio;
                rules.ScaleType = ScaleType;
                return rules;
            }
            set
            {
                UseScreenRelativePositioning = value.UseScreenRelativePositioning;
                MaintainAspectRatio = value.MaintainAspectRatio;
                ScaleType = value.ScaleType;
             
            }
        }

        /// <summary>
        /// The event fired when the resolution is changed (assuming the object
        /// is decorated with a `ResolutionChangedListener` attribute).
        /// </summary>
        public ResolutionChangedDelegate OnResolutionChanged;

        public ResolutionRelativeObject() : base() { }

        public override void Kill()
        {
            base.Kill();

            if (ListeningToResolutionChanges)
            {
                ArtemisEngine.DisplayManager.RemoveResolutionChangeListener(this);
            }
        }
    }
}
