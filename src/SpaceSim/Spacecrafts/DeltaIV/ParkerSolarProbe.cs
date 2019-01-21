﻿using System;
using System.Drawing;
using System.IO;
using SpaceSim.Drawing;
using SpaceSim.Engines;
using SpaceSim.Physics;
using SpaceSim.Spacecrafts.FalconCommon;
using VectorMath;

using SpaceSim.Properties;

namespace SpaceSim.Spacecrafts.DeltaIV
{
    class ParkerSolarProbe : SpaceCraftBase
    {
        public override string CraftName { get { return _craftName; } }
        public override string CommandFileName { get { return "demosat.xml"; } }

        public override double Width { get { return 2.94; } }
        public override double Height { get { return 3.6; } }

        public override double DryMass
        {
            get
            {
                if (!_deployedFairings)
                {
                    return _leftFairing.DryMass + _rightFairing.DryMass;
                }

                return 0;
            }
        }

        public override AeroDynamicProperties GetAeroDynamicProperties { get { return AeroDynamicProperties.ExposedToAirFlow; } }

        public override double FormDragCoefficient
        {
            get
            {
                if (!_deployedFairings)
                {
                    return _leftFairing.FormDragCoefficient + _rightFairing.FormDragCoefficient;
                }

                return 1;
            }
        }

        public override double LiftCoefficient
        {
            get
            {
                if (!_deployedFairings)
                {
                    return _leftFairing.LiftCoefficient + _rightFairing.LiftCoefficient;
                }

                return 1;
            }
        }

        public override double FrontalArea
        {
            get
            {
                if (!_deployedFairings)
                {
                    return _leftFairing.FrontalArea + _rightFairing.FrontalArea;
                }

                return 1;
            }
        }

        public override double ExposedSurfaceArea
        {
            get
            {
                if (!_deployedFairings)
                {
                    return _leftFairing.ExposedSurfaceArea + _rightFairing.ExposedSurfaceArea;
                }

                return 1;
            }
        }

        public override double LiftingSurfaceArea
        {
            get
            {
                if (!_deployedFairings)
                {
                    return _leftFairing.LiftingSurfaceArea + _rightFairing.LiftingSurfaceArea;
                }

                return 1;
            }
        }

        public override Color IconColor { get { return Color.White; } }

        private string _craftName;

        private DIVH5mFairing _leftFairing;
        private DIVH5mFairing _rightFairing;
        private bool _deployedFairings;
        DateTime timestamp = DateTime.Now;

        public ParkerSolarProbe(string craftDirectory, DVector2 position, DVector2 velocity, double payloadMass)
            : base(craftDirectory, position, velocity, payloadMass, 0, "DeltaIV/psp.png")
        {
            _craftName = new DirectoryInfo(craftDirectory).Name;

            Engines = new IEngine[0];
        }

        public override void Release()
        {
            _rightFairing.Release();
            _leftFairing.Release();

            base.Release();
        }

        public void AttachFairings(DIVH5mFairing leftFairing, DIVH5mFairing rightFairing)
        {
            _leftFairing = leftFairing;
            _rightFairing = rightFairing;

            _leftFairing.SetParent(this);
            _rightFairing.SetParent(this);
        }

        public override void Update(double dt)
        {
            base.Update(dt);

            if (!_deployedFairings)
            {
                _leftFairing.UpdateChildren(Position, Velocity);
                _rightFairing.UpdateChildren(Position, Velocity);

                _leftFairing.SetPitch(Pitch);
                _rightFairing.SetPitch(Pitch);
            }
        }

        public override void DeployFairing()
        {
            _deployedFairings = true;

            _leftFairing.Stage();
            _rightFairing.Stage();
        }

        public override void RenderGdi(Graphics graphics, Camera camera)
        {
            base.RenderGdi(graphics, camera);

            _leftFairing.RenderGdi(graphics, camera);
            _rightFairing.RenderGdi(graphics, camera);

            if (Settings.Default.WriteCsv && (DateTime.Now - timestamp > TimeSpan.FromSeconds(1)))
            {
                string filename = MissionName + ".csv";

                if (!File.Exists(filename))
                {
                    File.AppendAllText(filename, "Velocity, Acceleration, Altitude, Throttle\r\n");
                }

                timestamp = DateTime.Now;

                string contents = string.Format("{0}, {1}, {2}, {3}\r\n",
                    this.GetRelativeVelocity().Length(),
                    this.GetRelativeAcceleration().Length() * 100,
                    this.GetRelativeAltitude() / 100,
                    this.Throttle * 10);
                File.AppendAllText(filename, contents);
            }
        }
    }
}