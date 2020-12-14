﻿using System;
using System.Drawing;
using SpaceSim.Engines;
using SpaceSim.Physics;
using VectorMath;
using System.IO;
using SpaceSim.Common;
using SpaceSim.Drawing;
using SpaceSim.Properties;

namespace SpaceSim.Spacecrafts.ITS
{
    class StarshipSN4 : SpaceCraftBase
    {
        public override string CraftName { get { return "StarshipSN4"; } }
        public override string CommandFileName { get { return "StarshipSN4.xml"; } }

        public override double DryMass { get { return 45000; } }
        public override double Width { get { return 9; } }
        public override double Height { get { return 30.3; } }

        public override AeroDynamicProperties GetAeroDynamicProperties { get { return AeroDynamicProperties.ExposedToAirFlow; } }

        DateTime timestamp = DateTime.Now;
        double payloadMass = 0;

        public override double LiftingSurfaceArea { get { return Math.Abs(Width * Height * Math.Cos(GetAlpha())); } }

        public override double LiftCoefficient
        {
            get
            {
                double baseCd = GetBaseCd(0.6);
                double alpha = GetAlpha();

                return baseCd * Math.Sin(alpha * 2);
            }
        }

        public override double FrontalArea
        {
            get
            {
                double alpha = GetAlpha();
                double crossSectionalArea = Math.PI * Math.Pow(Width / 2, 2);
                double sideArea = Width * Height;

                return Math.Abs(crossSectionalArea * Math.Cos(alpha)) + Math.Abs(sideArea * Math.Sin(alpha));
            }
        }

        public override double ExposedSurfaceArea
        {
            get
            {
                // A = 2πrh + πr2
                return 2 * Math.PI * (Width / 2) * Height + FrontalArea;
            }
        }

        public override Color IconColor
        {
            get { return Color.White; }
        }

        public override double FormDragCoefficient
        {
            get
            {
                double alpha = GetAlpha();
                double baseCd = GetBaseCd(0.5);
                bool isRetrograde = false;

                if (alpha > Constants.PiOverTwo || alpha < -Constants.PiOverTwo)
                {
                    baseCd = GetBaseCd(0.7);
                    isRetrograde = true;
                }

                double dragCoefficient = Math.Abs(baseCd * Math.Sin(alpha));

                double dragPreservation = 1.0;

                if (isRetrograde)
                {
                    // if retrograde
                    if (Throttle > 0 && MachNumber > 1.5 && MachNumber < 20.0)
                    {
                        double throttleFactor = Throttle / 50;
                        double cantFactor = Math.Sin(Engines[0].Cant * 2);
                        dragPreservation += throttleFactor * cantFactor;
                        dragCoefficient *= dragPreservation;
                    }
                }

                return Math.Abs(dragCoefficient);
            }
        }

        //private SpriteSheet _spriteSheet;

        public StarshipSN4(string craftDirectory, DVector2 position, DVector2 velocity, double payloadMass = 0, double propellantMass = 400000)
            : base(craftDirectory, position, velocity, payloadMass, propellantMass, null)
        {
            StageOffset = new DVector2(0, 0);

            Engines = new IEngine[1];
            double engineOffsetX = 0;
            var offset = new DVector2(engineOffsetX * Width * 0.22, Height * 0.32);
            Engines[0] = new RaptorSL2019(0, this, offset);

            //_spriteSheet = new SpriteSheet("Textures/Spacecrafts/Its/scaledShip.png", 12, 12);

            string texturePath = "Its/StarshipSN4.png";
            string fullPath = Path.Combine("Textures/Spacecrafts", texturePath);
            this.Texture = new Bitmap(fullPath);

            this.payloadMass = payloadMass;
        }

        protected override void RenderShip(Graphics graphics, Camera camera, RectangleF screenBounds)
        {
            double drawingRotation = Pitch + Math.PI * 0.5;

            var offset = new PointF(screenBounds.X + screenBounds.Width * 0.5f,
                                    screenBounds.Y + screenBounds.Height * 0.5f);

            camera.ApplyScreenRotation(graphics);
            camera.ApplyRotationMatrix(graphics, offset, drawingRotation);

            // Normalize the angle to [0,360]
            int rollAngle = (int)(Roll * MathHelper.RadiansToDegrees) % 360;

            if(rollAngle <= 90)
                graphics.DrawImage(this.Texture, screenBounds.X, screenBounds.Y, screenBounds.Width * 1.0f, screenBounds.Height);
            else
                graphics.DrawImage(this.Texture, screenBounds.X + screenBounds.Width * 0.9f, screenBounds.Y, -screenBounds.Width * 1.0f, screenBounds.Height);

            // Index into the sprite
            //int ships = _spriteSheet.Cols * _spriteSheet.Rows;
            //int spriteIndex = (rollAngle * ships) / 360;
            //while (spriteIndex < 0)
            //    spriteIndex += ships;

            //_spriteSheet.Draw(spriteIndex, graphics, screenBounds);

            graphics.ResetTransform();

            if (Settings.Default.WriteCsv && (DateTime.Now - timestamp > TimeSpan.FromSeconds(1.14)))
            {
                string filename = MissionName + ".csv";

                if (!File.Exists(filename))
                {
                    File.AppendAllText(filename, "Velocity (m/s), Acceleration (dm/s²), Altitude (m), Throttle (%)\r\n");
                }

                timestamp = DateTime.Now;

                double density = this.GravitationalParent.GetAtmosphericDensity(this.GetRelativeAltitude());
                double velocity = this.GetRelativeVelocity().Length();
                double dynamicPressure = 0.5 * density * velocity * velocity;

                string contents = string.Format("{0}, {1}, {2}, {3}\r\n",
                    this.GetRelativeVelocity().Length() * 10,
                    this.GetRelativeAcceleration().Length() * 10,
                    this.GetRelativeAltitude() - 93.8,
                    //this.GetDownrangeDistance(new DVector2(110173478023.81982, 102548302161.23631)),
                    //this.GetDownrangeDistance(new DVector2(0, 0)),
                    this.Throttle);
                File.AppendAllText(filename, contents);
            }
        }
    }
}

