﻿using System;
using System.Drawing;
using SpaceSim.Engines;
using SpaceSim.Physics;
using SpaceSim.Spacecrafts.FalconCommon;
using VectorMath;

namespace SpaceSim.Spacecrafts.Falcon9SSTO
{
    sealed class F9SSTO : F9S1Base
    {
        public override string CraftName { get { return "F9 S1"; } }
        public override string CommandFileName { get { return "F9SSTO.xml"; } }

        public override double DryMass { get { return 21600; } }

        public override double Width { get { return 4.11; } }
        public override double Height { get { return 47.812188; } }

        public F9SSTO(string craftDirectory, DVector2 position, DVector2 velocity)
            : base(craftDirectory, position, velocity, 409500, "Textures/f9ssto.png", null)
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
