using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using SpaceSim.Physics;
using VectorMath;
using System.Drawing.Drawing2D;

namespace SpaceSim.Drawing
{
    class Plume
    {
        private Random _random;
        private Color _color;
        private double _throttle;
        private double _angle;
        private double _retrograde;
        private DVector2 _position = new DVector2();

        public Plume(int seed, Color color)
        {
            _random = new Random(seed);
            _color = color;
        }

        public void Update(TimeStep timeStep, DVector2 enginePosition, DVector2 shipVelocity,
                   double rotation, double throttle, double ispMultiplier, double angle = 0)
        {
            if (angle != 0)
                _angle = angle;

            _retrograde = rotation + Math.PI + _angle;
            _throttle = throttle;

            _position = enginePosition.Clone();
        }

        public void Draw(Graphics graphics, Camera camera)
        {
            if (camera.Contains(_position))
            {
                camera.ApplyScreenRotation(graphics);

                PointF flameSource = RenderUtils.WorldToScreen(_position, camera.Bounds);
                flameSource.X -= (float)(40 / camera.Zoom);
                flameSource.Y += (float)(121 / camera.Zoom);

                float scale = (float)(_throttle / camera.Zoom);
                float flameX = (float)(Math.Cos(_retrograde) * scale);
                float flameY = (float)(Math.Sin(_retrograde) * scale);
                PointF flameTip = new PointF(flameSource.X + flameX, flameSource.Y + flameY);

                Color colSupersonic = Color.FromArgb(63, 195, 135, 255);
                //Brush brSupersonic = new SolidBrush(colSupersonic);

                //GraphicsPath graphPath = new GraphicsPath();
                //PointF corner1 = new PointF(flameSource.X - flameX / 10f, flameSource.Y + flameY / 10);
                //graphPath.AddLine(flameSource, corner1);
                //PointF corner2 = new PointF(flameTip.X - flameX / 10f, flameTip.Y + flameY / 10);
                //graphPath.AddLine(flameSource, corner2);
                //PointF corner3 = new PointF(flameTip.X + flameX / 10f, flameTip.Y - flameY / 10);
                //graphPath.AddLine(flameSource, corner3);
                //PointF corner4 = new PointF(flameSource.X + flameX / 10f, flameSource.Y - flameY / 10);
                //graphPath.AddLine(flameSource, corner4);

                //graphics.FillPath(brSupersonic, graphPath);

                graphics.DrawLine(new Pen(colSupersonic, scale / 50), flameSource, flameTip);

                graphics.ResetTransform();
            }
        }
    }
}
