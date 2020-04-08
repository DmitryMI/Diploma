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
** File: PointHandler.cs 
** 
** Copyright (c) Complex Solution Group. 
**
** Description: Handling Point objects of the Shape-file
**
=============================================================================*/

namespace MapAround.IO.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using MapAround.Geometry;
    using MapAround.IO;

    /// <summary>
    /// ���������� �����.
    /// </summary>
    internal class PointHandler : ShapeHandler
    {
        /// <summary> ��� ������</summary>
        public override ShapeType ShapeType
        {
            get { return ShapeType.Point;}// ShapeGeometryType.Point; }
        }	
        

        /// <summary>
        /// ������ ������ �������������� �����.
        /// </summary>
        /// <param name="file">������� �����</param>
        /// <param name="record">������ Shape-����� � ������� ����� �������� ����������� ����������</param>
        /// <param name="bounds">�������������� �������������, � ������� ������ ������������ �������������� ������������� ������</param>
        public override bool Read(/*BigEndianBinaryReader*/Stream file, BoundingRectangle bounds, ShapeFileRecord record)
        {

            ICoordinate p = PlanimetryEnvironment.NewCoordinate(0, 0);
            p.X = file.ReadDouble();// ShapeFile.ReadDouble64_LE(stream);
            p.Y = file.ReadDouble();// ShapeFile.ReadDouble64_LE(stream);

            if (bounds != null && !bounds.IsEmpty() && !bounds.ContainsPoint(p))
                return false;

            record.Points.Add(p);

            record.MinX = p.X;
            record.MinY = p.Y;
            record.MaxX = record.MinX;
            record.MaxY = record.MinY;

            return true;
        }


        /// <summary>
        /// Writes to the given stream the equilivent shape file record given a Geometry object.
        /// </summary>
        /// <param name="geometry">The geometry object to write.</param>
        /// <param name="file">The stream to write to.</param>
        ///// <param name="geometryFactory">The geometry factory to use.</param>
        public override void Write(IGeometry geometry, BinaryWriter file)//, IGeometryFactory geometryFactory)
        {
            file.Write(int.Parse(Enum.Format(typeof(ShapeType), this.ShapeType, "d")));
            ICoordinate[] coords = geometry.ExtractCoordinates();
            if (coords.Length > 0)
            {
                ICoordinate external = coords[0];// geometry.Coordinates[0];
                file.Write(external.X);
                file.Write(external.Y);
            }
        }

        /// <summary>
        /// �������� ����� � ������ ��������������� ������� (��� ������ � ����)
        /// </summary>
        /// <param name="geometry">�������������� ������ </param>
        /// <returns>
        /// ����� ��� 16�������� ������� </returns>
        public override int GetLength(IGeometry geometry)
        {
            return 10; // 10 => shapetyppe(2)+ xy(4*2)
        }		
    }
}