using System.Drawing;
using SpaceSim.SolarSystem;

namespace SpaceSim.Structures
{
    class StarshipTower : StructureBase
    {
        public override double Width { get { return 70; } }
        public override double Height { get { return 160; } }

        public override Color IconColor { get { return Color.White; } }

        public StarshipTower(double surfaceAngle, double height, IMassiveBody parent)
            : base(surfaceAngle, height, "Textures/Structures/StarshipTower.png", parent)
        {
        }
    }
}
