using System.Drawing;
using SpaceSim.SolarSystem;

namespace SpaceSim.Structures
{
    class LaunchTower : StructureBase
    {
        public override double Width { get { return 100; } }
        public override double Height { get { return 150; } }

        public override Color IconColor { get { return Color.White; } }

        public LaunchTower(double surfaceAngle, double height, IMassiveBody parent)
            : base(surfaceAngle, height, "Textures/Structures/LaunchTower.png", parent)
        {
        }
    }
}
