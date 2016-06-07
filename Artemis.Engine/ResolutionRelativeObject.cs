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

        /// <summary>
        /// Whether or not this object is the listening to changes in the game's resolution
        /// (which will trigger the OnResolutionChanged event).
        /// </summary>
        public bool ListeningToResolutionChanges { get; private set; }

        /// <summary>
        /// Whether or not the position of this object is a Target coordinate, as opposed to a
        /// World coordinate.
        /// </summary>
        public bool UseTargetRelativePositioning;

        /// <summary>
        /// Whether or not to maintain the aspect ratio of this object upon dynamically
        /// scaling it to match the current resolution.
        /// </summary>
        public bool MaintainAspectRatio;

        /// <summary>
        /// Determines how to dynamically scale the object to match the current resolution.
        /// For more information on the definition of individual values, see ResolutionScaleType.
        /// </summary>
        public ResolutionScaleType ScaleType;

        /// <summary>
        /// The ResolutionScaleRules as a single object.
        /// </summary>
        public ResolutionScaleRules ScaleRules
        {
            get
            {
                var rules = new ResolutionScaleRules();
                rules.UseTargetRelativePositioning = UseTargetRelativePositioning;
                rules.MaintainAspectRatio = MaintainAspectRatio;
                rules.ScaleType = ScaleType;
                return rules;
            }
            set
            {
                UseTargetRelativePositioning = value.UseTargetRelativePositioning;
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
