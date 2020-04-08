//  MapAround - .NET tools for developing web and desktop mapping applications 

//  Copyright (coffee) 2009-2012 OOO "GKR"
//  This program is free software; you can redistribute it and/or 
//  modify it under the terms of the GNU General Public License 
//   as published by the Free Software Foundation; either version 3 
//  of the License, or (at your option) any later version. 
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with this program; If not, see <http://www.gnu.org/licenses/>



/*===========================================================================
** 
** File: PolygonHandler.cs 
** 
** Copyright (c) Complex Solution Group. 
**
** Description: Handling Shape-file objects
**
=============================================================================*/


namespace MapAround.IO.Handlers
{
    using System.IO;

    using MapAround.Geometry;
    using MapAround.IO;

    /// <summary>
    /// ������� ����� ������������ �������������� ������ 
    /// (��� ������ �� ������ � ������ ������ � �����)
    /// </summary>
    internal abstract class ShapeHandler
    {
        /// <summary> ��� ������</summary>
        public abstract ShapeType ShapeType { get; }

        /// <summary>
        /// ������ ������ ������ �� ��������������� ������� � ��������� ������ shape-����� 
        /// </summary>
        /// <param name="file">������� ����� ��� ������</param>
        /// <param name="bounds">�������������� �������������, � ������� ������ ������������ �������������� ������������� ������</param>
        /// <param name="Record">������ Shape-����� � ������� ����� �������� ����������� ����������</param>
        /// <returns>���������� ��������</returns>
        public abstract bool Read(Stream file, BoundingRectangle bounds, ShapeFileRecord Record);

        /// <summary>
        /// �������� ������ ���������������  ������� � ��������� ����� 
        /// </summary>
        /// <param name="geometry">�������������� ������ ��� ������</param>
        /// <param name="file">����� ������</param>
        public abstract void Write(IGeometry geometry, BinaryWriter file);

        /// <summary>
        /// �������� ����� � ������ ��������������� ������� (��� ������ � ����)
        /// </summary>
        /// <param name="geometry">�������������� ������ </param>
        /// <returns>
        /// ����� ��� 16�������� ������� </returns>
        public abstract int GetLength(IGeometry geometry);

        

        /// <summary>�������� ������ �� ���������� ������ ������ � ��������� �������</summary>
        /// <param name="bounds">������� �������</param>
        /// <param name="record">������ shape-�����</param>
        /// <returns></returns>
        protected static bool IsRecordInView(BoundingRectangle bounds, ShapeFileRecord record)
        {
            if (bounds != null && !bounds.IsEmpty())
            {
                if (!bounds.Intersects(
                    new BoundingRectangle(PlanimetryEnvironment.NewCoordinate(record.MinX, record.MinY),
                                          PlanimetryEnvironment.NewCoordinate(record.MaxX, record.MaxY))))
                    return false;
            }
            return true;
        }

        #region Deprecated

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="shapeType"></param>
        ///// <returns></returns>
        //[System.Obsolete]
        //public static bool IsPoint(ShapeType shapeType)
        //{
        //    return shapeType == ShapeType.Point;
        //    //||
        //    //       shapeType == ShapeGeometryType.PointZ ||
        //    //       shapeType == ShapeGeometryType.PointM ||
        //    //       shapeType == ShapeGeometryType.PointZM;
        //}


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="shapeType"></param>
        ///// <returns></returns>
        //[System.Obsolete]
        //public static bool IsMultiPoint(ShapeType shapeType)
        //{
        //    return shapeType == ShapeType.Multipoint;
        //    //||
        //    //       shapeType == ShapeGeometryType.MultiPointZ ||
        //    //       shapeType == ShapeGeometryType.MultiPointM ||
        //    //       shapeType == ShapeGeometryType.MultiPointZM;
        //}


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="shapeType"></param>
        ///// <returns></returns>
        //[System.Obsolete]
        //public static bool IsLineString(ShapeType shapeType)
        //{
        //    return shapeType == ShapeType.Polyline;//.LineString;
        //    //||
        //    //       shapeType == ShapeGeometryType.LineStringZ ||
        //    //       shapeType == ShapeGeometryType.LineStringM ||
        //    //       shapeType == ShapeGeometryType.LineStringZM;            
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="shapeType"></param>
        ///// <returns></returns>
        //[System.Obsolete]
        //public static bool IsPolygon(ShapeType shapeType)
        //{
        //    return shapeType == ShapeType.Polygon;
        //    //||
        //    //   shapeType == ShapeGeometryType.PolygonZ ||
        //    //   shapeType == ShapeGeometryType.PolygonM ||
        //    //   shapeType == ShapeGeometryType.PolygonZM;
        //}

        #endregion
    }
}