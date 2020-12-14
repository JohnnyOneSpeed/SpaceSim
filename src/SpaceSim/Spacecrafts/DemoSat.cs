﻿using System;
using System.Drawing;
using System.IO;
using SpaceSim.Drawing;
using SpaceSim.Engines;
using SpaceSim.Physics;
using SpaceSim.Spacecrafts.FalconCommon;
using VectorMath;

using SpaceSim.Properties;

namespace SpaceSim.Spacecrafts
{
    class DemoSat : SpaceCraftBase
    {
        public override string CraftName { get { return _craftName; } }
        public override string CommandFileName { get { return "demosat.xml"; } }

        public override double Width { get { return 4; } }
        public override double Height { get { return 8.52; } }

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

        private Fairing _leftFairing;
        private Fairing _rightFairing;
        private bool _deployedFairings;
        DateTime timestamp = DateTime.Now;

        public DemoSat(string craftDirectory, DVector2 position, DVector2 velocity, double payloadMass, string imagePath = "Satellites/default.png")
            : base(craftDirectory, position, velocity, payloadMass, 0, imagePath)
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

        public void AttachFairings(Fairing leftFairing, Fairing rightFairing)
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

            if (Settings.Default.WriteCsv && (DateTime.Now - timestamp > TimeSpan.FromSeconds(1.17)))
            {
                string filename = MissionName + ".csv";

                if (!File.Exists(filename))
                {
                    //File.AppendAllText(filename, "Velocity (m/s), Acceleration (cm/s²), Altitude (hm), Throttle (‰), Heating rate (daW/m²), Dynamic pressure (daPa)\r\n");
                    File.AppendAllText(filename, "Velocity (m/s), Acceleration (cm/s²), Altitude (hm), Throttle (‰), Lateral Acceleration (cm/s²), Heating rate (daW/m²), Dynamic pressure (daPa)\r\n");
                }

                timestamp = DateTime.Now;

                double density = this.GravitationalParent.GetAtmosphericDensity(this.GetRelativeAltitude());
                double velocity = this.GetRelativeVelocity().Length();
                double dynamicPressure = 0.5 * density * velocity * velocity;

                //string contents = string.Format("{0}, {1}, {2}, {3}, {4}, {5}\r\n",
                string contents = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}\r\n",
                    this.GetRelativeVelocity().Length(),
                    this.GetRelativeAcceleration().Length() * 100,
                    this.GetRelativeAltitude() / 100,
                    //this.GetRelativeAltitude() / 1000,
                    this.Throttle * 10,
                    this.GetLateralAcceleration().Length() * 100,
                    this.HeatingRate / 10,
                    dynamicPressure / 10);
                File.AppendAllText(filename, contents);
            }
        }
    }
}
