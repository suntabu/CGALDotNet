#pragma once

#include "CGALWrapper.h"
#include "Points.h"

#include <CGAL/Polygon_2.h>
#include <CGAL/enum.h> 

template<class K>
int Polygon2_Count(void* ptr)
{
	auto polygon = (CGAL::Polygon_2<K>*)ptr;
	return polygon->size();
}

template<class K>
void* Polygon2_Copy(void* ptr)
{
	auto polygon = (CGAL::Polygon_2<K>*)ptr;
	return new CGAL::Polygon_2<K>(*polygon);
}

template<class K>
void Polygon2_Clear(void* ptr)
{
	auto polygon = (CGAL::Polygon_2<K>*)ptr;
	polygon->clear();
}

template<class K>
void* Polygon2_CreateFromPoints(Point2d* points, int startIndex, int count)
{
	auto polygon = new CGAL::Polygon_2<K>();

	for (auto i = 0; i < count; i++)
		polygon->push_back(points[startIndex + i].To<K>());

	return polygon;
}

template<class K>
Point2d Polygon2_GetPoint(void* ptr, int index)
{
	auto polygon = (CGAL::Polygon_2<K>*)ptr;
	auto point = polygon->vertex(index);

	return { CGAL::to_double(point.x()), CGAL::to_double(point.y()) };
}

template<class K>
void Polygon2_GetPoints(void* ptr, Point2d* points, int startIndex, int count)
{
	auto polygon = (CGAL::Polygon_2<K>*)ptr;

	for (auto i = 0; i < count; i++)
	{
		auto point = polygon->vertex(i);
		points[startIndex + i].x = CGAL::to_double(point.x());
		points[startIndex + i].y = CGAL::to_double(point.y());
	}
}

template<class K>
void Polygon2_SetPoint(void* ptr, int index, Point2d point)
{
	auto polygon = (CGAL::Polygon_2<K>*)ptr;
	(*polygon)[index] = CGAL::Point_2<K>(point.x, point.y);
}

template<class K>
void Polygon2_SetPoints(void* ptr, Point2d* points, int startIndex, int count)
{
	auto polygon = (CGAL::Polygon_2<K>*)ptr;

	for (auto i = 0; i < count; i++)
		(*polygon)[i] = points[startIndex + i].To<K>();
}

template<class K>
void Polygon2_Reverse(void* ptr)
{
	auto polygon = (CGAL::Polygon_2<K>*)ptr;
	polygon->reverse_orientation();
}

template<class K>
bool Polygon2_IsSimple(void* ptr)
{
	auto polygon = (CGAL::Polygon_2<K>*)ptr;
	return polygon->is_simple();
}

template<class K>
bool Polygon2_IsConvex(void* ptr)
{
	auto polygon = (CGAL::Polygon_2<K>*)ptr;
	return polygon->is_convex();
}

template<class K>
CGAL::Orientation Polygon2_Orientation(void* ptr)
{
	auto polygon = (CGAL::Polygon_2<K>*)ptr;
	return polygon->orientation();
}

template<class K>
CGAL::Oriented_side Polygon2_OrientedSide(void* ptr, Point2d point)
{
	auto polygon = (CGAL::Polygon_2<K>*)ptr;
	return polygon->oriented_side(point.To<K>());
}

template<class K>
double Polygon2_SignedArea(void* ptr)
{
	auto polygon = (CGAL::Polygon_2<K>*)ptr;
	return CGAL::to_double(polygon->area());
}



