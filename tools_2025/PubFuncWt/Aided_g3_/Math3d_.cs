using g3;
using System.Collections;
using System;
using PubFuncWt;

public static class Math3d_
{

    //Find the line of intersection between two planes. The planes are defined by a normal and a point on that plane.
    //The outputs are a point on the line and a vector which indicates it's direction. If the planes are not parallel, 
    //the function outputs true, otherwise false.
    public static bool PlanePlaneIntersection(out Vector3d linePoint, out Vector3d lineVec, Vector3d plane1Normal, Vector3d plane1Position, Vector3d plane2Normal, Vector3d plane2Position)
    {
        linePoint = Vector3d.Zero;
        lineVec = Vector3d.Zero;

        //We can get the direction of the line of intersection of the two planes by calculating the 
        //cross product of the normals of the two planes. Note that this is just a direction and the line
        //is not fixed in space yet. We need a point for that to go with the line vector.
        lineVec = Vector3d.Cross(plane1Normal, plane2Normal);

        //Next is to calculate a point on the line to fix it's position in space. This is done by finding a vector from
        //the plane2 location, moving parallel to it's plane, and intersecting plane1. To prevent rounding
        //errors, this vector also has to be perpendicular to lineDirection. To get this vector, calculate
        //the cross product of the normal of plane2 and the lineDirection.      
        Vector3d ldir = Vector3d.Cross(plane2Normal, lineVec);

        double denominator = Vector3d.Dot(plane1Normal, ldir);

        //Prevent divide by Zero and rounding errors by requiring about 5 degrees angle between the planes.
        if (Math.Abs(denominator) > 0.006f)
        {
            Vector3d plane1ToPlane2 = plane1Position - plane2Position;
            double t = Vector3d.Dot(plane1Normal, plane1ToPlane2) / denominator;
            linePoint = plane2Position + t * ldir;

            return true;
        }

        //output not valid
        else
        {
            return false;
        }
    }

    //Get the intersection between a line and a plane. 
    //If the line and plane are not parallel, the function outputs true, otherwise false.
    public static bool LinePlaneIntersection(out Vector3d intersection, Vector3d linePoint, Vector3d lineVec, Vector3d planeNormal, Vector3d planePoint)
    {
        double length;
        double dotNumerator;
        double dotDenominator;
        Vector3d vector;
        intersection = Vector3d.Zero;

        //calculate the distance between the linePoint and the line-plane intersection point
        dotNumerator = Vector3d.Dot((planePoint - linePoint), planeNormal);
        dotDenominator = Vector3d.Dot(lineVec, planeNormal);

        //line and plane are not parallel
        if (dotDenominator != 0.0f)
        {
            length = dotNumerator / dotDenominator;

            //create a vector from the linePoint to the intersection point
            vector = lineVec.Normalized * length;

            //get the coordinates of the line-plane intersection point
            intersection = linePoint + vector;

            return true;
        }

        //output not valid
        else
        {
            return false;
        }
    }

    //Calculate the intersection point of two lines. Returns true if lines intersect, otherwise false.
    //Note that in 3d, two lines do not intersect most of the time. So if the two lines are not in the 
    //same plane, use ClosestPointsOnTwoLines() instead.
    public static bool LineLineIntersection(out Vector3d intersection, Vector3d linePoint1, Vector3d lineVec1, Vector3d linePoint2, Vector3d lineVec2)
    {
        intersection = Vector3d.Zero;

        Vector3d lineVec3 = linePoint2 - linePoint1;
        Vector3d crossVec1and2 = Vector3d.Cross(lineVec1, lineVec2);
        if (crossVec1and2.LengthSquared == 0)
            return false;

        Vector3d crossVec3and2 = Vector3d.Cross(lineVec3, lineVec2);

        double planarFactor = Vector3d.Dot(lineVec3, crossVec1and2);

        //Lines are not coplanar. Take into account rounding errors.
        if ((planarFactor >= 0.00001f) || (planarFactor <= -0.00001f))
        {
            return false;
        }

        //Note: sqrMagnitude does x*x+y*y+z*z on the input vector.
        double s = Vector3d.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.LengthSquared;
        intersection = linePoint1 + (lineVec1 * s);

        return true;
    }

    //Two non-parallel lines which may or may not touch each other have a point on each line which are closest
    //to each other. This function finds those two points. If the lines are not parallel, the function 
    //outputs true, otherwise false.
    public static bool ClosestPointsOnTwoLines(out Vector3d closestPointLine1, out Vector3d closestPointLine2, Vector3d linePoint1, Vector3d lineVec1, Vector3d linePoint2, Vector3d lineVec2)
    {
        closestPointLine1 = Vector3d.Zero;
        closestPointLine2 = Vector3d.Zero;

        double a = Vector3d.Dot(lineVec1, lineVec1);
        double b = Vector3d.Dot(lineVec1, lineVec2);
        double e = Vector3d.Dot(lineVec2, lineVec2);

        double d = a * e - b * b;

        //lines are not parallel
        if (d != 0.0f)
        {

            Vector3d r = linePoint1 - linePoint2;
            double c = Vector3d.Dot(lineVec1, r);
            double f = Vector3d.Dot(lineVec2, r);

            double s = (b * f - c * e) / d;
            double t = (a * f - c * b) / d;

            closestPointLine1 = linePoint1 + lineVec1 * s;
            closestPointLine2 = linePoint2 + lineVec2 * t;

            return true;
        }

        else
        {
            return false;
        }
    }

    //This function returns a point which is a projection from a point to a line.
    //The line is regarded infinite. If the line is finite, use ProjectPointOnLineSegment() instead.
    public static Vector3d ProjectPointOnLine(Vector3d linePoint, Vector3d lineVec, Vector3d point)
    {
        //get vector from point on line to point in space
        Vector3d linePointToPoint = point - linePoint;
        double t = Vector3d.Dot(linePointToPoint, lineVec);
        return linePoint + lineVec * t;
    }

    //This function returns a point which is a projection from a point to a line segment.
    //If the projected point lies outside of the line segment, the projected point will 
    //be clamped to the appropriate line edge.
    //If the line is infinite instead of a segment, use ProjectPointOnLine() instead.
    public static Vector3d ProjectPointOnLineSegment(Vector3d linePoint1, Vector3d linePoint2, Vector3d point)
    {
        Vector3d vector = linePoint2 - linePoint1;

        Vector3d projectedPoint = ProjectPointOnLine(linePoint1, vector.Normalized, point);

        int side = PointOnWhichSideOfLineSegment(linePoint1, linePoint2, projectedPoint);

        //The projected point is on the line segment
        if (side == 0)
        {
            return projectedPoint;
        }

        if (side == 1)
        {

            return linePoint1;
        }

        if (side == 2)
        {

            return linePoint2;
        }
        //output is invalid
        return Vector3d.Zero;
    }

    //This function returns a point which is a projection from a point to a plane.
    public static Vector3d ProjectPointOnPlane(Vector3d planeNormal, Vector3d planePoint, Vector3d point)
    {

        double distance;
        Vector3d translationVector;

        //First calculate the distance from the point to the plane:
        distance = SignedDistancePlanePoint(planeNormal, planePoint, point);

        //Reverse the sign of the distance
        distance *= -1;

        //Get a translation vector
        translationVector = planeNormal.Normalized * distance;

        //Translate the point to form a projection
        return point + translationVector;
    }

    //Projects a vector onto a plane. The output is not normalized.
    public static Vector3d ProjectVectorOnPlane(Vector3d planeNormal, Vector3d vector)
    {
        return vector - (Vector3d.Dot(vector, planeNormal) * planeNormal);
    }

    //Get the shortest distance between a point and a plane. The output is signed so it holds information
    //as to which side of the plane normal the point is.
    public static double SignedDistancePlanePoint(Vector3d planeNormal, Vector3d planePoint, Vector3d point)
    {
        return Vector3d.Dot(planeNormal, (point - planePoint));
    }

    //This function calculates a signed (+ or - sign instead of being ambiguous) dot product. It is basically used
    //to figure out whether a vector is positioned to the left or right of another vector. The way this is done is
    //by calculating a vector perpendicular to one of the vectors and using that as a reference. This is because
    //the result of a dot product only has signed information when an angle is transitioning between more or less
    //then 90 degrees.
    public static double SignedDotProduct(Vector3d vectorA, Vector3d vectorB, Vector3d normal)
    {
        Vector3d perpVector;
        double dot;

        //Use the geometry object normal and one of the input vectors to calculate the perpendicular vector
        perpVector = Vector3d.Cross(normal, vectorA);

        //Now calculate the dot product between the perpendicular vector (perpVector) and the other input vector
        dot = Vector3d.Dot(perpVector, vectorB);

        return dot;
    }

    //Calculate the angle between a vector and a plane. The plane is made by a normal vector.
    //Output is in radians.
    public static double AngleVectorPlane(Vector3d vector, Vector3d normal)
    {

        double dot;
        double angle;

        //calculate the the dot product between the two input vectors. This gives the cosine between the two vectors
        dot = Vector3d.Dot(vector, normal);

        //this is in radians
        angle = (double)Math.Acos(dot);

        return 1.570796326794897f - angle; //90 degrees - angle
    }

    //Convert a plane defined by 3 points to a plane defined by a vector and a point. 
    //The plane point is the middle of the triangle defined by the 3 points.
    public static void PlaneFrom3Points(out Vector3d planeNormal, out Vector3d planePoint, Vector3d pointA, Vector3d pointB, Vector3d pointC)
    {

        planeNormal = Vector3d.Zero;
        planePoint = Vector3d.Zero;

        //Make two vectors from the 3 input points, originating from point A
        Vector3d AB = pointB - pointA;
        Vector3d AC = pointC - pointA;

        //Calculate the normal
        planeNormal = Vector3d.Cross(AB, AC).Normalized;

        //Get the points in the middle AB and AC
        Vector3d middleAB = pointA + (AB / 2.0f);
        Vector3d middleAC = pointA + (AC / 2.0f);

        //Get vectors from the middle of AB and AC to the point which is not on that line.
        Vector3d middleABtoC = pointC - middleAB;
        Vector3d middleACtoB = pointB - middleAC;

        //Calculate the intersection between the two lines. This will be the center 
        //of the triangle defined by the 3 points.
        //We could use LineLineIntersection instead of ClosestPointsOnTwoLines but due to rounding errors 
        //this sometimes doesn't work.
        Vector3d temp;
        ClosestPointsOnTwoLines(out planePoint, out temp, middleAB, middleABtoC, middleAC, middleACtoB);
    }

    //This function finds out on which side of a line segment the point is located.
    //The point is assumed to be on a line created by linePoint1 and linePoint2. If the point is not on
    //the line segment, project it on the line using ProjectPointOnLine() first.
    //Returns 0 if point is on the line segment.
    //Returns 1 if point is outside of the line segment and located on the side of linePoint1.
    //Returns 2 if point is outside of the line segment and located on the side of linePoint2.
    public static int PointOnWhichSideOfLineSegment(Vector3d linePoint1, Vector3d linePoint2, Vector3d point)
    {

        Vector3d lineVec = linePoint2 - linePoint1;
        Vector3d pointVec = point - linePoint1;

        double dot = Vector3d.Dot(pointVec, lineVec);

        //point is on side of linePoint2, compared to linePoint1
        if (dot > 0)
        {

            //point is on the line segment
            if (pointVec.Length <= lineVec.Length)
            {

                return 0;
            }

            //point is not on the line segment and it is on the side of linePoint2
            else
            {

                return 2;
            }
        }

        //Point is not on side of linePoint2, compared to linePoint1.
        //Point is not on the line segment and it is on the side of linePoint1.
        else
        {

            return 1;
        }
    }

    //Returns true if a line segment (made up of linePoint1 and linePoint2) is fully or partially in a rectangle
    //made up of RectA to RectD. The line segment is assumed to be on the same plane as the rectangle. If the line is 
    //not on the plane, use ProjectPointOnPlane() on linePoint1 and linePoint2 first.
    public static bool IsLineInRectangle(Vector3d linePoint1, Vector3d linePoint2, Vector3d rectA, Vector3d rectB, Vector3d rectC, Vector3d rectD)
    {

        bool pointAInside = false;
        bool pointBInside = false;

        pointAInside = IsPointInRectangle(linePoint1, rectA, rectC, rectB, rectD);

        if (!pointAInside)
        {
            pointBInside = IsPointInRectangle(linePoint2, rectA, rectC, rectB, rectD);
        }

        //none of the points are inside, so check if a line is crossing
        if (!pointAInside && !pointBInside)
        {
            bool lineACrossing = AreLineSegmentsCrossing(linePoint1, linePoint2, rectA, rectB);
            bool lineBCrossing = AreLineSegmentsCrossing(linePoint1, linePoint2, rectB, rectC);
            bool lineCCrossing = AreLineSegmentsCrossing(linePoint1, linePoint2, rectC, rectD);
            bool lineDCrossing = AreLineSegmentsCrossing(linePoint1, linePoint2, rectD, rectA);

            if (lineACrossing || lineBCrossing || lineCCrossing || lineDCrossing)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return true;
        }
    }

    //Returns true if "point" is in a rectangle mad up of RectA to RectD. The line point is assumed to be on the same 
    //plane as the rectangle. If the point is not on the plane, use ProjectPointOnPlane() first.
    public static bool IsPointInRectangle(Vector3d point, Vector3d rectA, Vector3d rectC, Vector3d rectB, Vector3d rectD)
    {
        Vector3d vector;
        Vector3d linePoint;

        //get the center of the rectangle
        vector = rectC - rectA;
        double size = -(vector.Length / 2f);
        vector = vector.Normalized * size;
        Vector3d middle = rectA + vector;

        Vector3d xVector = rectB - rectA;
        double width = xVector.Length / 2f;

        Vector3d yVector = rectD - rectA;
        double height = yVector.Length / 2f;

        linePoint = ProjectPointOnLine(middle, xVector.Normalized, point);
        vector = linePoint - point;
        double yDistance = vector.Length;

        linePoint = ProjectPointOnLine(middle, yVector.Normalized, point);
        vector = linePoint - point;
        double xDistance = vector.Length;

        if ((xDistance <= width) && (yDistance <= height))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //Returns true if line segment made up of pointA1 and pointA2 is crossing line segment made up of
    //pointB1 and pointB2. The two lines are assumed to be in the same plane.
    public static bool AreLineSegmentsCrossing(Vector3d pointA1, Vector3d pointA2, Vector3d pointB1, Vector3d pointB2)
    {
        Vector3d closestPointA;
        Vector3d closestPointB;
        int sideA;
        int sideB;

        Vector3d lineVecA = pointA2 - pointA1;
        Vector3d lineVecB = pointB2 - pointB1;

        bool valid = ClosestPointsOnTwoLines(out closestPointA, out closestPointB, pointA1, lineVecA.Normalized, pointB1, lineVecB.Normalized);

        //lines are not parallel
        if (valid)
        {
            sideA = PointOnWhichSideOfLineSegment(pointA1, pointA2, closestPointA);
            sideB = PointOnWhichSideOfLineSegment(pointB1, pointB2, closestPointB);

            if ((sideA == 0) && (sideB == 0))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

}