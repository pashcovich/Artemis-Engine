
namespace Artemis.Engine
{
    public class ResolutionRelativeObject : ArtemisObject
    {
        public bool UseScreenRelativePositioning { get; set; }

        public bool MaintainAspectRatio { get; set; }

        public ResolutionScaleType ScaleType { get; set; }

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

        public ResolutionRelativeObject() : base() { }
    }
}
