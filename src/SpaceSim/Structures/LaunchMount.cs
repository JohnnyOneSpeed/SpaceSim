using System.Drawing;
using SpaceSim.SolarSystem;

namespace SpaceSim.Structures
{
    class LaunchMount : StructureBase
    {
        //public override double Width { get { return 17; } }
        //public override double Height { get { return 30; } }
      public override double Width { get { return 12; } }
      public override double Height { get { return 7; } }

      public override Color IconColor { get { return Color.White; } }

        public LaunchMount(double surfaceAngle, double height, IMassiveBody parent)
            //: base(surfaceAngle, height, "Textures/Structures/LaunchMount.png", parent)
            : base(surfaceAngle, height, "Textures/Structures/TestMount.png", parent)
        {
        }
    }
}
