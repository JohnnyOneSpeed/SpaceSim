﻿using System;
using System.Drawing;
using SpaceSim.Drawing;
using SpaceSim.Particles;
using SpaceSim.Physics;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Engines
{
    abstract class EngineBase : IEngine
    {
        public bool IsActive { get; protected set; }

        public double Throttle { get; protected set; }

        public double Cant { get; protected set; }

        protected ISpaceCraft Parent;
        protected DVector2 Offset;

        private double _offsetLength;
        private double _offsetRotation;

        private EngineFlame _engineFlame;
        private Plume _plume;

        protected EngineBase(ISpaceCraft parent, DVector2 offset, EngineFlame flame)
        {
            Parent = parent;
            Offset = offset;

            _offsetLength = offset.Length();
            _offsetRotation = (offset.Angle() - Math.PI / 2.0);

            _engineFlame = flame;
        }

        protected EngineBase(ISpaceCraft parent, DVector2 offset, Plume plume)
        {
            Parent = parent;
            Offset = offset;

            _offsetLength = offset.Length();
            _offsetRotation = offset.Angle() - Math.PI / 2.0;

            _plume = plume;
        }

        public virtual void Startup()
        {
            IsActive = true;
        }

        public virtual void Shutdown()
        {
            AdjustThrottle(0);

            IsActive = false;
        }

        public void AdjustThrottle(double targetThrottle)
        {
            Throttle = targetThrottle;
        }

        public void AdjustCant(double targetAngle)
        {
            Cant = targetAngle;
        }

        public abstract double Thrust(double ispMultiplier);

        public abstract double MassFlowRate(double ispMultiplier);

        public abstract IEngine Clone();

        public void Update(TimeStep timeStep, double ispMultiplier)
        {
            double rotation = Parent.Pitch -_offsetRotation;

            DVector2 offset = new DVector2(Math.Cos(rotation), Math.Sin(rotation)) * _offsetLength;

            double throttle = (IsActive && Parent.PropellantMass > 0) ? Throttle : 0;

            if(_engineFlame != null)
                _engineFlame.Update(timeStep, Parent.Position - offset, Parent.Velocity, Parent.Pitch, throttle, ispMultiplier, Cant * Offset.X / 3);
            if (_plume != null)
                _plume.Update(timeStep, Parent.Position - offset, Parent.Velocity, Parent.Pitch, throttle, ispMultiplier, Cant * Offset.X / 3);
        }

        public void Draw(Graphics graphics, Camera camera)
        {
            if (_engineFlame != null)
                _engineFlame.Draw(graphics, camera);
            if (_plume != null)
                _plume.Draw(graphics, camera);
        }
    }
}
