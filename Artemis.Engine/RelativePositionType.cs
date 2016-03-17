
namespace Artemis.Engine
{
    public class RelativePositionType
    {

        public static readonly RelativePositionType TopLeft      = new RelativePositionType(0, 0);
        public static readonly RelativePositionType TopRight     = new RelativePositionType(1, 0);
        public static readonly RelativePositionType BottomLeft   = new RelativePositionType(0, 1);
        public static readonly RelativePositionType BottomRight  = new RelativePositionType(1, 1);
        public static readonly RelativePositionType Center       = new RelativePositionType(.5f, .5f);
        public static readonly RelativePositionType CenterTop    = new RelativePositionType(.5f, 0);
        public static readonly RelativePositionType CenterBottom = new RelativePositionType(.5f, 1);
        public static readonly RelativePositionType CenterLeft   = new RelativePositionType(0, .5f);
        public static readonly RelativePositionType CenterRight  = new RelativePositionType(1, .5f);

        public float xOffset { get; private set; }
        public float yOffset { get; private set; }

        private RelativePositionType(float x, float y)
        {
            xOffset = x;
            yOffset = y;
        }
    }
}
