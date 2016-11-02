using SpaceSim.Engines;
using SpaceSim.Spacecrafts.FalconCommon;
using VectorMath;

namespace SpaceSim.Spacecrafts.Falcon9
{
    sealed class F9S1 : F9S1Base
    {
        public override string CraftName { get { return "F9 S1"; } }
        public override string CommandFileName { get { return "F9S1.xml"; } }

        public override double DryMass { get { return 25600; } }

        public override double Width { get { return 4.11; } }
        public override double Height { get { return 47.812188; } }

        public F9S1(string craftDirectory, DVector2 position, DVector2 velocity, double propellantMass = 398887)
            : base(craftDirectory, position, velocity, propellantMass, "Textures/f9S1.png")
        {
            StageOffset = new DVector2(0, 25.5);

            Engines = new IEngine[9];

            for (int i=0; i < 9; i++)
            {
                double engineOffsetX = (i - 4.0) / 4.0;

                var offset = new DVector2(engineOffsetX * Width * 0.3, Height * 0.45);

                Engines[i] = new Merlin1D(i, this, offset);
            }
        }
    }
}
