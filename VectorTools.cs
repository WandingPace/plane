using UnityEngine;
using System.Collections;

public class VectorTools
{
	public static float DistanceBetween_XZ(Vector3 av3_Vector1, Vector3 av3_Vector2)
	{
		return Vector3ToXZVector2(av3_Vector2 - av3_Vector1).magnitude;
	}

	public static float DistanceBetween(Vector3 av3_Vector1, Vector3 av3_Vector2)
	{
		return (av3_Vector2 - av3_Vector1).magnitude;
	}

    public static Vector2 Vector3ToXZVector2(Vector3 av3_Vector3)
    {
        return new Vector2(av3_Vector3.x, av3_Vector3.z);
    }

    public static Vector3 XZVector2ToVector3(Vector2 av2_XZVector2)
    {
        return new Vector3(av2_XZVector2.x, 0, av2_XZVector2.y);
    }

    public static Vector2 Rotate(Vector2 av2_Vector, float af_Angle)
    {
        float f_SinAngle = Mathf.Sin(Mathf.Deg2Rad * af_Angle);
        float f_CosAngle = Mathf.Cos(Mathf.Deg2Rad * af_Angle);

        return new Vector2(f_CosAngle * av2_Vector.x - f_SinAngle * av2_Vector.y, f_CosAngle * av2_Vector.y + f_SinAngle * av2_Vector.x);
    }

    public static float Cross(Vector2 av2_u, Vector2 av2_v)
    {
        return av2_u.x * av2_v.y - av2_v.x * av2_u.y;
    }

    public static Vector2 Project(Vector2 av2_u, Vector2 av2_Target)
    {
        return (Vector2.Dot(av2_u, av2_Target) / av2_Target.sqrMagnitude) * av2_Target;
    }

    // Right and Up vector must be normalized
    public static Vector2 ProjectPointOnPlane(Vector3 av3_Point, Vector3 av3_PlaneRight, Vector3 av3_PlaneUp)
    {
        Vector2 v2_ProjectedPoint = Vector2.zero;
        v2_ProjectedPoint.x = Vector3.Dot(av3_Point, av3_PlaneRight);
        v2_ProjectedPoint.y = Vector3.Dot(av3_Point, av3_PlaneUp);

        return v2_ProjectedPoint;
    }

    ////////////////////////////////////////////////////////////
    // 2D Intersection tests
    ////////////////////////////////////////////////////////////
    #region 2D Intersection tests

    public static Vector2 ClosestPointOnLineSegmentToPoint(Vector2 av2_SegmentStart, Vector2 av2_SegmentEnd, Vector2 av2_OutsidePoint)
    {
        Vector2 v2_Segment = av2_SegmentEnd - av2_SegmentStart;
        Vector2 v2_StartToPoint = av2_OutsidePoint - av2_SegmentStart;

        float f_Ratio = Vector2.Dot(v2_StartToPoint, v2_Segment) / v2_Segment.sqrMagnitude;

        f_Ratio = Mathf.Max(0, f_Ratio);
        f_Ratio = Mathf.Min(1, f_Ratio);

        return av2_SegmentStart + f_Ratio * v2_Segment;
    }

    public static bool LinesIntersect(Vector2 av2_Line1Point, Vector2 av2_Line1Direction, Vector2 av2_Line2Point, Vector2 av2_Line2Direction, out Vector2 av2_Intersection, out float af_Line1Ratio, out float af_Line2Ratio)
    {
        float f_DirectionsCrossProduct = VectorTools.Cross(av2_Line1Direction, av2_Line2Direction);
        if (f_DirectionsCrossProduct != 0)
        {
            // to avoid doing the division twice
            float f_InverseDirectionsCrossProduct = 1.0f / f_DirectionsCrossProduct;

            af_Line1Ratio = VectorTools.Cross(av2_Line2Point - av2_Line1Point, av2_Line2Direction) * f_InverseDirectionsCrossProduct;
            af_Line2Ratio = VectorTools.Cross(av2_Line2Point - av2_Line1Point, av2_Line1Direction) * f_InverseDirectionsCrossProduct;
            av2_Intersection = av2_Line1Point + af_Line1Ratio * av2_Line1Direction;
            return true;
        }
        // the two lines are parallel, so no intersection is possible
        else
        {
            af_Line1Ratio = 0;
            af_Line2Ratio = 0;
            av2_Intersection = Vector2.zero;
            return false;
        }
    }

    public static bool RaysIntersect(Vector2 av2_Ray1Start, Vector2 av2_Ray1Direction, Vector2 av2_Ray2Start, Vector2 av2_Ray2Direction, out Vector2 av2_Intersection)
    {
        av2_Intersection = Vector2.zero;

        float f_Ray1Ratio;
        float f_Ray2Ratio;
        if (LinesIntersect(av2_Ray1Start, av2_Ray1Direction, av2_Ray2Start, av2_Ray2Direction, out av2_Intersection, out f_Ray1Ratio, out f_Ray2Ratio))
        {
            return f_Ray1Ratio >= 0 && f_Ray2Ratio >= 0;
        }
        else
        {
            return false;
        }
    }

    public static bool LineSegmentsIntersect(Vector2 av2_Segment1Start, Vector2 av2_Segment1End, Vector2 av2_Segment2Start, Vector2 av2_Segment2End, out Vector2 av2_Intersection)
    {
        av2_Intersection = Vector2.zero;

        float f_Segment1Ratio;
        float f_Segment2Ratio;
        if (LinesIntersect(av2_Segment1Start, av2_Segment1End - av2_Segment1Start, av2_Segment2Start, av2_Segment2End - av2_Segment2Start, out av2_Intersection, out f_Segment1Ratio, out f_Segment2Ratio))
        {
            return (f_Segment1Ratio >= 0 && f_Segment1Ratio <= 1) && (f_Segment2Ratio >= 0 && f_Segment2Ratio <= 1);
        }
        else
        {
            return false;
        }
    }

    public static bool CircleLineSegmentIntersect(Vector2 av2_circlePosition, float af_CircleRadius, Vector2 av2_SegmentStart, Vector2 av2_SegmentEnd, out Vector2 av2_Intersection)
    {
        av2_Intersection = VectorTools.ClosestPointOnLineSegmentToPoint(av2_SegmentStart, av2_SegmentEnd, av2_circlePosition);
        
        if ((av2_Intersection - av2_circlePosition).sqrMagnitude <= Mathf.Pow(af_CircleRadius, 2))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool RayPlaneIntersect(Vector3 av3_RayStart, Vector3 av3_RayDirection, Vector3 av3_PlanePoint, Vector3 av3_PlaneNormal, out Vector3 av3_Intersection)
    {
        av3_Intersection = Vector3.zero;

        float f_PlaneNormaleDotRayDirection = Vector3.Dot(av3_PlaneNormal, av3_RayDirection);
        if (f_PlaneNormaleDotRayDirection == 0)
        {
            // Ray is parallel to the plane
            return false;
        }
        else
        {
            float f_IntersectionRatio = Vector3.Dot(-av3_PlaneNormal, av3_RayStart - av3_PlanePoint) / f_PlaneNormaleDotRayDirection;
            if (f_IntersectionRatio < 0)
            {
                // ray is going away from the plane
                return false;
            }
            else
            {
                av3_Intersection = av3_RayStart + av3_RayDirection * f_IntersectionRatio;
                return true;
            }
        }
    }

    #endregion
    ////////////////////////////////////////////////////////////

}
