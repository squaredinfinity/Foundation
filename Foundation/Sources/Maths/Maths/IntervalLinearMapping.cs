using System;
using SquaredInfinity.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Maths
{
    /// <summary>
    /// Maps values from one interval to another.
    /// For example, mapping values [-100,100] to [0, 1]
    /// </summary>
    public struct IntervalLinearMapping
    {
        Interval Target;
        Interval Source;

        double NormalizationComponent;
        double DenormalizationComponent;

        public IntervalLinearMapping(Interval source, Interval target)
        {
            Source = source;
            Target = target;

            NormalizationComponent = Target.Span / Source.Span;
            DenormalizationComponent = Source.Span / Target.Span;

            if (Source.Flags != Target.Flags)
            {
                var ex = new InvalidOperationException($"Source and Target Intervals must have same edges (e.g. both closed)");
                //ex.TryAddContextData("Source", () => Source);
                //ex.TryAddContextData("Target", () => Target);
                throw ex;
            }

            if (Source.IsEmpty || Target.IsEmpty)
            {
                var ex = new InvalidOperationException($"Source and Target Intervals must be Non Empty");
                //ex.TryAddContextData("Source", () => Source);
                //ex.TryAddContextData("Target", () => Target);
                throw ex;
            }

            if (!Source.IsProper || !Target.IsProper)
            {
                var ex = new InvalidOperationException($"Source and Target Intervals must be Proper");
                //ex.TryAddContextData("Source", () => Source);
                //ex.TryAddContextData("Target", () => Target);
                throw ex;
            }
        }

        public double MapTo(double sourceValue)
        {
            // x * m + b
            return (sourceValue - Source.From) * NormalizationComponent + Target.From;
        }

        public double MapFrom(double targetValue)
        {
            // x * m + b
            return (targetValue - Target.From) * DenormalizationComponent + Source.From;
        }

        public double MapDistanceTo(double sourceDistance)
        {
            if(Target.IsInverted)
                return sourceDistance * -NormalizationComponent;
            else
                return sourceDistance * NormalizationComponent;
        }

        public double MapDistanceFrom(double targetDistance)
        {
            if(Target.IsInverted)
                return targetDistance * -DenormalizationComponent;
            else
                return targetDistance * DenormalizationComponent;
        }
    }
}
