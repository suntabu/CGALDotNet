﻿using System;
using System.Collections.Generic;
using System.Text;

using CGALDotNet.Geometry;

namespace CGALDotNet.Polygons
{

    public abstract class PolygonWithHoles2 : CGALObject
    {

        public PolygonWithHoles2()
        {
      
        }

        internal PolygonWithHoles2(IntPtr ptr) : base(ptr)
        {
            CheckPtr();
        }

        public int HoleCount { get; protected set; }

        public abstract void Clear();

        public abstract void RemoveBoundary();

        public abstract void ReverseBoundary();

        public abstract void AddHole(Polygon2 polygon);

        public abstract void RemoveHole(int index);

        public abstract void ReverseHole(int index);

        public abstract bool FindIfUnbounded();

        public abstract bool FindIfBoundaryIsSimple();

        public abstract bool FindIfBoundaryIsConvex();

        public abstract CGAL_ORIENTATION FindBoundaryOrientation();

        public abstract CGAL_ORIENTED_SIDE BoundaryOrientedSide(Point2d point);

        public abstract double FindBoundarySignedArea();

        public double FindBoundaryArea()
        {
            return Math.Abs(FindBoundarySignedArea());
        }

        public abstract bool FindIfHoleIsSimple(int index);

        public abstract bool FindIfHoleIsConvex(int index);

        public abstract CGAL_ORIENTATION FindHoleOrientation(int index);

        public abstract CGAL_ORIENTED_SIDE HoleOrientedSide(int index, Point2d point);

        public abstract double FindHoleSignedArea(int index);

        public double FindHoleArea(int index)
        {
            return Math.Abs(FindHoleSignedArea(index));
        }

        public void Print()
        {
            var builder = new StringBuilder();
            Print(builder);
            Console.WriteLine(builder.ToString());
        }

        public void Print(StringBuilder builder)
        {
            builder.AppendLine(ToString());

            bool isUnbounded = FindIfUnbounded();
            builder.AppendLine("Is unbounded = " + isUnbounded);

            if (!isUnbounded)
            {
                bool boundaryIsSimple = FindIfBoundaryIsSimple();
                builder.AppendLine("Boundary is simple = " + boundaryIsSimple);

                if (boundaryIsSimple)
                {
                    builder.AppendLine("Boundary is convex = " + FindIfBoundaryIsConvex());
                    builder.AppendLine("Boundary orientation = " + FindBoundaryOrientation());
                    builder.AppendLine("Boundary signed Area = " + FindBoundarySignedArea());
                }
            }

            for (int i = 0; i < HoleCount; i++)
            {

                bool holeIsSimple = FindIfHoleIsSimple(i);
                builder.AppendLine("Hole " + i + " is simple = " + FindIfHoleIsSimple(i));

                if (holeIsSimple)
                {
                    builder.AppendLine("Hole " + i + " is convex = " + FindIfHoleIsConvex(i));
                    builder.AppendLine("Hole " + i + " is orientation = " + FindHoleOrientation(i));
                    builder.AppendLine("Hole " + i + " is signed area = " + FindHoleSignedArea(i));
                }

                builder.AppendLine();
            }
        }

        protected void CheckPolygon(Polygon2 polygon)
        {
            if (!polygon.IsSimple)
                throw new Exception("Polygon must be simple.");
        }

    }
}
