
namespace Artemis.Engine.Graphics
{
    public class ResolutionScaleRules
    {

        /// <summary>
        /// This indicates which factor an image gets scaled by.
        /// 
        /// For more information, see ResolutionScaleType docs.
        /// </summary>
        public ResolutionScaleType ScaleType { get; set; }

        /// <summary>
        /// Whether or not the coordinates of the object are relative positions
        /// or absolute positions.
        /// </summary>
        public bool RelativePositioning { get; set; }

        /// <summary>
        /// Whether or not the aspect ratio of the object should be maintained
        /// upon scaling to match the current resolution.
        /// </summary>
        public bool MaintainAspectRatio { get; set; }

        public ResolutionScaleRules( ResolutionScaleType scaleType = ResolutionScaleType.BY_MIN
                                        , bool relativePositioning      = true
                                        , bool maintainAspectRatio      = true )
        {
            ScaleType = scaleType;
            RelativePositioning = relativePositioning;
            MaintainAspectRatio = maintainAspectRatio;
        }

    }
}
