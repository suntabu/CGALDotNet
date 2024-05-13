﻿using System;
using System.Collections.Generic;
using System.Linq;

using CGALDotNetGeometry.Numerics;

namespace CGALDotNetGeometry.Nurbs
{
	/// <summary>
	/// Param struct to hold surface params.
	/// </summary>
	public struct NurbsSurfaceParams3d
	{
		public int degreeU, degreeV;
		public List<double> knotsU, knotsV;
		public HPoint3d[,] controlPoints;
	}

	/// <summary>
	/// Class for representing a non-rational NURBS surface
	/// </summary>
	public abstract class BaseNurbsSurface3d
	{

		/// <summary>
		/// The min degree a surface can have.
		/// </summary>
		public const int MIN_DEGREE = 1;


		/// <summary>
		/// The max degree a surface can have.
		/// </summary>
		public const int MAX_DEGREE = 9;

		/// <summary>
		/// Is the surface rational.
		/// </summary>
		public abstract bool IsRational { get; }

		/// <summary>
		/// The degree on the first dimension.
		/// </summary>
		public abstract int DegreeU { get; }

		/// <summary>
		/// The degree on the second dimension.
		/// </summary>
		public abstract int DegreeV { get; }

		/// <summary>
		/// The knots on the first dimension.
		/// </summary>
		public double[] KnotsU { get; protected set; }

		/// <summary>
		/// The knots on the second dimension.
		/// </summary>
		public double[] KnotsV { get; protected set; }

		/// <summary>
		/// The surfaces width.
		/// </summary>
		public abstract int Width { get; }

		/// <summary>
		/// The surfaces height.
		/// </summary>
		public abstract int Height { get; }

		/// <summary>
		/// Is the surface valid.
		/// Checks the degree, knots and controal points.
		/// </summary>
		public bool IsValid => NurbsCheck.SurfaceIsValid(this);

		/// <summary>
		/// Is the surface closed in the u axis.
		/// </summary>
		public bool IsClosedU => NurbsCheck.SurfaceIsClosedU(this);

		/// <summary>
		/// Is the surface closed in the v axis.
		/// </summary>
		public bool IsClosedV => NurbsCheck.SurfaceIsClosedV(this);

		/// <summary>
		/// Get the control point.
		/// </summary>
		/// <param name="i">The points first index.</param>
		/// <param name="j">The points second index.</param>
		/// <returns>The control point in cartesian coordinates.</returns>
		public abstract Point3d GetCartesianControlPoint(int i, int j);

		/// <summary>
		/// Get the control point.
		/// </summary>
		/// <param name="i">The points first index.</param>
		/// <param name="j">The points second index.</param>
		/// <returns>The control point in cartesian coordinates.</returns>
		public abstract HPoint3d GetHomogeneousControlPoint(int i, int j);

		/// <summary>
		/// Get the control points
		/// </summary>
		/// <param name="points">The list to copy the points into.</param>
		public abstract void GetCartesianControlPoints(List<Point3d> points);

		/// <summary>
		/// Set the control point.
		/// </summary>
		/// <param name="i">The points first index.</param>
		/// <param name="j">The points second index.</param>
		/// <param name="point">The control point in cartesian coordinates.</param>
		public abstract void SetCartesianControlPoint(int i, int j, Point3d point);

		/// <summary>
		/// Get the point at parameter u,v.
		/// </summary>
		/// <param name="u">The u parameter.</param>
		/// <param name="v">The v parameter.</param>
		/// <returns>The point at u,v.</returns>
		public Point3d CartesianPoint(double u, double v)
		{
			return NurbsEval.CartesianSurfacePoint(this, u, v);
		}

		/// <summary>
		/// Get a array of points.
		/// </summary>
		/// <param name="samples">The number of samples.</param>
		/// <param name="points">The points array that has a width 
		/// and height of the number of samples.</param>
		public void GetCartesianPoints(int samples, Point3d[,] points)
		{
			NurbsTess.GetCartesianPoints(this, points, 0, 1, samples, 0, 1, samples);
		}

		/// <summary>
		/// Get the tangent at parameter u,v.
		/// </summary>
		/// <param name="u">The v parameter.</param>
		/// <param name="v">The v parameter.</param>
		/// <param name="tu">The u parameters tangent.</param>
		/// <param name="tv">The v parameters tangent.</param>
		/// <returns>The tanget at u,v.</returns>
		public void Tangent(double u, double v, out Vector3d tu, out Vector3d tv)
		{
			NurbsEval.SurfaceTangent(this, u, v, out tu, out tv);
		}

		/// <summary>
		/// Get the normal at parameter u,v.
		/// </summary>
		/// <param name="u">The u parameter.</param>
		/// <param name="v">The v parameter.</param>
		/// <returns>The normal at u,v.</returns>
		public Vector3d Normal(double u, double v)
		{
			return NurbsEval.SurfaceNormal(this, u, v);
		}

		/// <summary>
		/// Get a array of normals.
		/// </summary>
		/// <param name="samples">The number of samples</param>
		/// <param name="normals">The normal array that is same size as the number of samples.</param>
		public void GetNormals(int samples, Vector3d[,] normals)
		{
			NurbsTess.GetNormals(this, normals, 0, 1, samples, 0, 1, samples);
		}

		/// <summary>
		/// Translate the triangulation.
		/// </summary>
		/// <param name="translation">The amount to translate.</param>
		public void Translate(Point3d translation)
		{
			Transform(translation, Quaternion3d.Identity, Point3d.One);
		}

		/// <summary>
		/// Rotate the triangulation.
		/// </summary>
		/// <param name="rotation">The amount to rotate in radians.</param>
		public void Rotate(Quaternion3d rotation)
		{
			Transform(Point3d.Zero, rotation, Point3d.One);
		}

		/// <summary>
		/// Scale the triangulation.
		/// </summary>
		/// <param name="scale">The amount to scale.</param>
		public void Scale(Point3d scale)
		{
			Transform(Point3d.Zero, Quaternion3d.Identity, scale);
		}

		/// <summary>
		/// Transform the triangulation with a TRS matrix.
		/// </summary>
		/// <param name="translation">The amount to translate.</param>
		/// <param name="rotation">The amount to rotate.</param>
		/// <param name="scale">The amount to scale.</param>
		public abstract void Transform(Point3d translation, Quaternion3d rotation, Point3d scale);

		/// <summary>
		/// Normlize the surfaces u knots so the first knot starts at 0
		/// and the last knot ends at 1.
		/// </summary>
		public void NormalizeKnotsU()
		{
			double fisrt = KnotsU[0];
			double last = KnotsU.Last();

			for (int i = 0; i < KnotsU.Length; i++)
				KnotsU[i] = MathUtil.Normalize(KnotsU[i], fisrt, last);
		}

		/// <summary>
		/// Normlize the surfaces v knots so the first knot starts at 0
		/// and the last knot ends at 1.
		/// </summary>
		public void NormalizeKnotsV()
		{
			double fisrt = KnotsV[0];
			double last = KnotsV.Last();

			for (int i = 0; i < KnotsV.Length; i++)
				KnotsV[i] = MathUtil.Normalize(KnotsV[i], fisrt, last);
		}

		/// <summary>
		/// Insert a new knot into the surface and return as a new surface.
		/// </summary>
		/// <param name="srf">The surface to insert the knot into.</param>
		/// <param name="u">The parameter to insert the knot at.</param>
		/// <param name="repeat">The number of times to repeat the knot.</param>
		/// <returns>A new surface with the inserted knots.</returns>
		public static NurbsSurface3d InsertKnotU(NurbsSurface3d srf, double u, int repeat = 1)
		{
			var param = NurbsModify.SurfaceKnotInsertU(srf, u, repeat);
			return new NurbsSurface3d(param.degreeU, param.degreeV, param.knotsU, param.knotsV, param.controlPoints);
		}

		/// <summary>
		/// Insert a new knot into the surface and return as a new surface.
		/// </summary>
		/// <param name="srf">The surface to insert the knot into.</param>
		/// <param name="v">The parameter to insert the knot at.</param>
		/// <param name="repeat">The number of times to repeat the knot.</param>
		/// <returns>A new surface with the inserted knots.</returns>
		public static NurbsSurface3d InsertKnotV(NurbsSurface3d srf, double v, int repeat = 1)
		{
			var param = NurbsModify.SurfaceKnotInsertV(srf, v, repeat);
			return new NurbsSurface3d(param.degreeU, param.degreeV, param.knotsU, param.knotsV, param.controlPoints);
		}

	}

	/// <summary>
	/// Class for representing a non-rational NURBS surface
	/// </summary>
	public class NurbsSurface3d : BaseNurbsSurface3d
	{
		private int m_degreeU, m_degreeV;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public NurbsSurface3d()
		{

		}

		/// <summary>
		/// Create a new surface.
		/// </summary>
		/// <param name="degree_u">The degree on the first dimension.</param>
		/// <param name="degree_v">The degree on the second dimension.</param>
		/// <param name="knots_u">The knots on the first dimension.</param>
		/// <param name="knots_v">The knots on the second dimension.</param>
		/// <param name="control_points">The control points in cartesion coordinates.</param>
		public NurbsSurface3d(int degree_u, int degree_v, IList<double> knots_u, IList<double> knots_v, Point3d[,] control_points)
		{
			m_degreeU = degree_u;
			m_degreeV = degree_v;
			KnotsU = knots_u.ToArray();
			KnotsV = knots_v.ToArray();

			int width = control_points.GetLength(0);
			int height = control_points.GetLength(1);

			CartesianControlPoints = new Point3d[width, height];

			for (int j = 0; j < height; j++)
				for (int i = 0; i < width; i++)
					CartesianControlPoints[i, j] = control_points[i, j];
		}

		/// <summary>
		/// Create a new surface.
		/// </summary>
		/// <param name="degree_u">The degree on the first dimension.</param>
		/// <param name="degree_v">The degree on the second dimension.</param>
		/// <param name="knots_u">The knots on the first dimension.</param>
		/// <param name="knots_v">The knots on the second dimension.</param>
		/// <param name="control_points">The control points in cartesion coordinates.</param>
		internal NurbsSurface3d(int degree_u, int degree_v, IList<double> knots_u, IList<double> knots_v, HPoint3d[,] control_points)
		{
			m_degreeU = degree_u;
			m_degreeV = degree_v;
			KnotsU = knots_u.ToArray();
			KnotsV = knots_v.ToArray();

			int width = control_points.GetLength(0);
			int height = control_points.GetLength(1);

			CartesianControlPoints = new Point3d[width, height];

			for (int j = 0; j < height; j++)
				for (int i = 0; i < width; i++)
					CartesianControlPoints[i, j] = control_points[i, j].Cartesian;
		}

		internal Point3d[,] CartesianControlPoints { get; set; }

		/// <summary>
		/// Is the surface rational.
		/// </summary>
		public override bool IsRational => false;

		/// <summary>
		/// The degree on the first dimension.
		/// </summary>
		public override int DegreeU => m_degreeU;

		/// <summary>
		/// The degree on the second dimension.
		/// </summary>
		public override int DegreeV => m_degreeV;

		public override int Width => CartesianControlPoints.GetLength(0);

		public override int Height => CartesianControlPoints.GetLength(1);

		/// <summary>
		/// Get the control point.
		/// </summary>
		/// <param name="i">The points first index.</param>
		/// <param name="j">The points second index.</param>
		/// <returns>The control point in cartesian coordinates.</returns>
		public override Point3d GetCartesianControlPoint(int i, int j)
		{
			return CartesianControlPoints[i, j];
		}

		/// <summary>
		/// Get the control point.
		/// </summary>
		/// <param name="i">The points first index.</param>
		/// <param name="j">The points second index.</param>
		/// <returns>The control point in cartesian coordinates.</returns>
		public override HPoint3d GetHomogeneousControlPoint(int i, int j)
        {
			return CartesianControlPoints[i, j].Homogenous;
		}

		/// <summary>
		/// Get the control points
		/// </summary>
		/// <param name="points">The list to copy the points into.</param>
		public override void GetCartesianControlPoints(List<Point3d> points)
		{
			for (int j = 0; j < Height; j++)
				for (int i = 0; i < Width; i++)
					points.Add(CartesianControlPoints[i, j]);
		}

		/// <summary>
		/// Set the control point.
		/// </summary>
		/// <param name="i">The points first index.</param>
		/// <param name="j">The points second index.</param>
		/// <param name="point">The control point in cartesian coordinates.</param>
		public override void SetCartesianControlPoint(int i, int j, Point3d point)
		{
			CartesianControlPoints[i, j] = point;
		}

		/// <summary>
		/// Transform the triangulation with a TRS matrix.
		/// </summary>
		/// <param name="translation">The amount to translate.</param>
		/// <param name="rotation">The amount to rotate.</param>
		/// <param name="scale">The amount to scale.</param>
		public override void Transform(Point3d translation, Quaternion3d rotation, Point3d scale)
		{
			Matrix4x4d T = Matrix4x4d.Translate(translation);
			Matrix4x4d R = rotation.ToMatrix4x4d();
			Matrix4x4d S = Matrix4x4d.Scale(scale);

			var M = T * R * S;

			for (int i = 0; i < Width; i++)
				for (int j = 0; j < Height; j++)
					CartesianControlPoints[i,j] = M * CartesianControlPoints[i,j];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="srf"></param>
		/// <param name="u"></param>
		/// <param name="left"></param>
		/// <param name="right"></param>
		public static void SplitU(NurbsSurface3d srf, double u, out NurbsSurface3d left, out NurbsSurface3d right)
		{
			NurbsSurfaceParams3d leftParam, rightParam;
			NurbsModify.SurfaceSplitU(srf, u, out leftParam, out rightParam);

			left = new NurbsSurface3d(leftParam.degreeU, leftParam.degreeV, leftParam.knotsU, leftParam.knotsV, leftParam.controlPoints);
			right = new NurbsSurface3d(rightParam.degreeU, rightParam.degreeV, rightParam.knotsU, rightParam.knotsV, rightParam.controlPoints);

			left.NormalizeKnotsU();
			right.NormalizeKnotsU();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="srf"></param>
		/// <param name="v"></param>
		/// <param name="left"></param>
		/// <param name="right"></param>
		public static void SplitV(NurbsSurface3d srf, double v, out NurbsSurface3d left, out NurbsSurface3d right)
		{
			NurbsSurfaceParams3d leftParam, rightParam;
			NurbsModify.SurfaceSplitV(srf, v, out leftParam, out rightParam);

			left = new NurbsSurface3d(leftParam.degreeU, leftParam.degreeV, leftParam.knotsU, leftParam.knotsV, leftParam.controlPoints);
			right = new NurbsSurface3d(rightParam.degreeU, rightParam.degreeV, rightParam.knotsU, rightParam.knotsV, rightParam.controlPoints);

			left.NormalizeKnotsV();
			right.NormalizeKnotsV();
		}

	}

	/// <summary>
	/// Class for representing a non-rational NURBS surface
	/// </summary>
	public class RationalNurbsSurface3d : BaseNurbsSurface3d
	{
		private int m_degreeU, m_degreeV;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public RationalNurbsSurface3d()
		{

		}

		/// <summary>
		/// Create a new surface.
		/// </summary>
		/// <param name="degree_u">The degree on the first dimension.</param>
		/// <param name="degree_v">The degree on the second dimension.</param>
		/// <param name="knots_u">The knots on the first dimension.</param>
		/// <param name="knots_v">The knots on the second dimension.</param>
		/// <param name="control_points">The control points in cartesion coordinates.</param>
		public RationalNurbsSurface3d(int degree_u, int degree_v, IList<double> knots_u, IList<double> knots_v, Point3d[,] control_points)
		{
			m_degreeU = degree_u;
			m_degreeV = degree_v;
			KnotsU = knots_u.ToArray();
			KnotsV = knots_v.ToArray();

			int width = control_points.GetLength(0);
			int height = control_points.GetLength(1);

			HomogeneousControlPoints = new HPoint3d[width, height];

			for (int j = 0; j < height; j++)
				for (int i = 0; i < width; i++)
					HomogeneousControlPoints[i, j] = control_points[i, j].Homogenous;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="degree_u"></param>
		/// <param name="degree_v"></param>
		/// <param name="knots_u"></param>
		/// <param name="knots_v"></param>
		/// <param name="control_points"></param>
		/// <param name="weights"></param>
		public RationalNurbsSurface3d(int degree_u, int degree_v, IList<double> knots_u, IList<double> knots_v, Point3d[,] control_points, double[,] weights)
		{
			m_degreeU = degree_u;
			m_degreeV = degree_v;
			KnotsU = knots_u.ToArray();
			KnotsV = knots_v.ToArray();

			int width = control_points.GetLength(0);
			int height = control_points.GetLength(1);

			HomogeneousControlPoints = new HPoint3d[width, height];

			for (int j = 0; j < height; j++)
				for (int i = 0; i < width; i++)
					HomogeneousControlPoints[i, j] = control_points[i, j].ToHomogenous(weights[i, j]);
		}

		/// <summary>
		/// Create a new surface.
		/// </summary>
		/// <param name="degree_u">The degree on the first dimension.</param>
		/// <param name="degree_v">The degree on the second dimension.</param>
		/// <param name="knots_u">The knots on the first dimension.</param>
		/// <param name="knots_v">The knots on the second dimension.</param>
		/// <param name="control_points">The control points in cartesion coordinates.</param>
		internal RationalNurbsSurface3d(int degree_u, int degree_v, IList<double> knots_u, IList<double> knots_v, HPoint3d[,] control_points)
		{
			m_degreeU = degree_u;
			m_degreeV = degree_v;
			KnotsU = knots_u.ToArray();
			KnotsV = knots_v.ToArray();

			int width = control_points.GetLength(0);
			int height = control_points.GetLength(1);

			HomogeneousControlPoints = new HPoint3d[width, height];

			for (int j = 0; j < height; j++)
				for (int i = 0; i < width; i++)
					HomogeneousControlPoints[i, j] = control_points[i, j];
		}

		internal HPoint3d[,] HomogeneousControlPoints { get; set; }

		/// <summary>
		/// Is the surface rational.
		/// </summary>
		public override bool IsRational => true;

		/// <summary>
		/// The degree on the first dimension.
		/// </summary>
		public override int DegreeU => m_degreeU;

		/// <summary>
		/// The degree on the second dimension.
		/// </summary>
		public override int DegreeV => m_degreeV;

		public override int Width => HomogeneousControlPoints.GetLength(0);

		public override int Height => HomogeneousControlPoints.GetLength(1);

		/// <summary>
		/// Get the control point.
		/// </summary>
		/// <param name="i">The points first index.</param>
		/// <param name="j">The points second index.</param>
		/// <returns>The control point in cartesian coordinates.</returns>
		public override Point3d GetCartesianControlPoint(int i, int j)
		{
			return HomogeneousControlPoints[i, j].Cartesian;
		}

		/// <summary>
		/// Get the control point.
		/// </summary>
		/// <param name="i">The points first index.</param>
		/// <param name="j">The points second index.</param>
		/// <returns>The control point in cartesian coordinates.</returns>
		public override HPoint3d GetHomogeneousControlPoint(int i, int j)
		{
			return HomogeneousControlPoints[i, j];
		}

		/// <summary>
		/// Get the control points
		/// </summary>
		/// <param name="points">The list to copy the points into.</param>
		public override void GetCartesianControlPoints(List<Point3d> points)
		{
			for (int j = 0; j < Height; j++)
				for (int i = 0; i < Width; i++)
					points.Add(HomogeneousControlPoints[i, j].Cartesian);
		}

		/// <summary>
		/// Set the control point.
		/// </summary>
		/// <param name="i">The points first index.</param>
		/// <param name="j">The points second index.</param>
		/// <param name="point">The control point in cartesian coordinates.</param>
		public override void SetCartesianControlPoint(int i, int j, Point3d point)
		{
			HomogeneousControlPoints[i, j] = point.Homogenous;
		}

		/// <summary>
		/// Transform the triangulation with a TRS matrix.
		/// </summary>
		/// <param name="translation">The amount to translate.</param>
		/// <param name="rotation">The amount to rotate.</param>
		/// <param name="scale">The amount to scale.</param>
		public override void Transform(Point3d translation, Quaternion3d rotation, Point3d scale)
		{
			Matrix4x4d T = Matrix4x4d.Translate(translation);
			Matrix4x4d R = rotation.ToMatrix4x4d();
			Matrix4x4d S = Matrix4x4d.Scale(scale);

			var M = T * R * S;

			for (int i = 0; i < Width; i++)
				for (int j = 0; j < Height; j++)
                {
					var pw = HomogeneousControlPoints[i, j];
					var p = M * pw.Cartesian;

					HomogeneousControlPoints[i, j] = p.ToHomogenous(pw.w);
				}
					
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="srf"></param>
		/// <param name="u"></param>
		/// <param name="left"></param>
		/// <param name="right"></param>
		public static void SplitU(RationalNurbsSurface3d srf, double u, out RationalNurbsSurface3d left, out RationalNurbsSurface3d right)
		{
			NurbsSurfaceParams3d leftParam, rightParam;
			NurbsModify.SurfaceSplitU(srf, u, out leftParam, out rightParam);

			left = new RationalNurbsSurface3d(leftParam.degreeU, leftParam.degreeV, leftParam.knotsU, leftParam.knotsV, leftParam.controlPoints);
			right = new RationalNurbsSurface3d(rightParam.degreeU, rightParam.degreeV, rightParam.knotsU, rightParam.knotsV, rightParam.controlPoints);

			left.NormalizeKnotsU();
			right.NormalizeKnotsU();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="srf"></param>
		/// <param name="v"></param>
		/// <param name="left"></param>
		/// <param name="right"></param>
		public static void SplitV(RationalNurbsSurface3d srf, double v, out RationalNurbsSurface3d left, out RationalNurbsSurface3d right)
		{
			NurbsSurfaceParams3d leftParam, rightParam;
			NurbsModify.SurfaceSplitV(srf, v, out leftParam, out rightParam);

			left = new RationalNurbsSurface3d(leftParam.degreeU, leftParam.degreeV, leftParam.knotsU, leftParam.knotsV, leftParam.controlPoints);
			right = new RationalNurbsSurface3d(rightParam.degreeU, rightParam.degreeV, rightParam.knotsU, rightParam.knotsV, rightParam.controlPoints);

			left.NormalizeKnotsV();
			right.NormalizeKnotsV();
		}

	}

}

