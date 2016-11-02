﻿using System.Drawing;
using SpaceSim.Kernels;
using SpaceSim.Orbits;
using SpaceSim.Physics;
using VectorMath;

namespace SpaceSim.SolarSystem.Planets
{
    class Mars : MassiveBodyBase
    {
        public override double Mass
        {
            get { return 0.64174e24; }
        }

        public override double SurfaceRadius
        {
            get { return SymbolKernel.MARS_RADIUS; }
        }

        public override double AtmosphereHeight
        {
            get { return SymbolKernel.MARS_ATMOSPHERE; }
        }

        public override double RotationRate
        {
            get { return -7.077658089896e-5; }
        }

        public override double RotationPeriod
        {
            get { return 88774.92; }
        }

        public override Color IconColor { get { return Color.Orange; } }
        public override Color IconAtmopshereColor { get { return Color.LightSalmon; } }

        public Mars()
            : base(OrbitHelper.FromJplEphemeris(1.186183757284019E+08, -1.722428244606592E+08),
                   OrbitHelper.FromJplEphemeris(2.091501229864822E+01, 1.576687031430872E+01), new MarsKernel())
        {
        }

        public override double GetAtmosphericDensity(double altitude)
        {
            return base.GetAtmosphericDensity(altitude) * 0.02;
        }

        public override string ToString()
        {
            return "Mars";
        }
    }
}
