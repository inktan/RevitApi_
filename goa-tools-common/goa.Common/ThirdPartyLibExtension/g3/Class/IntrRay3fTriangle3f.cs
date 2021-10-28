using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using goa.Common.g3InterOp;

namespace g3
{
    public class IntrRay3fTriangle3f
    {
        Ray3f ray;
        public Ray3f Ray
        {
            get { return ray; }
            set { ray = value; Result = IntersectionResult.NotComputed; }
        }

        Triangle3f triangle;
        public Triangle3f Triangle
        {
            get { return triangle; }
            set { triangle = value; Result = IntersectionResult.NotComputed; }
        }

        public int Quantity = 0;
        public IntersectionResult Result = IntersectionResult.NotComputed;
        public IntersectionType Type = IntersectionType.Empty;

        public bool IsSimpleIntersection
        {
            get { return Result == IntersectionResult.Intersects && Type == IntersectionType.Point; }
        }


        public float RayParameter;
        public Vector3f TriangleBaryCoords;


        public IntrRay3fTriangle3f(Ray3f r, Triangle3f t)
        {
            ray = r; triangle = t;
        }


        public IntrRay3fTriangle3f Compute()
        {
            Find();
            return this;
        }


        public bool Find()
        {
            if (Result != IntersectionResult.NotComputed)
                return (Result != g3.IntersectionResult.NoIntersection);

            // Compute the offset origin, edges, and normal.
            Vector3f diff = ray.Origin - triangle.V0;
            Vector3f edge1 = triangle.V1 - triangle.V0;
            Vector3f edge2 = triangle.V2 - triangle.V0;
            Vector3f normal = edge1.Cross(edge2);

            // Solve Q + t*D = b1*E1 + b2*E2 (Q = kDiff, D = ray direction,
            // E1 = kEdge1, E2 = kEdge2, N = Cross(E1,E2)) by
            //   |Dot(D,N)|*b1 = sign(Dot(D,N))*Dot(D,Cross(Q,E2))
            //   |Dot(D,N)|*b2 = sign(Dot(D,N))*Dot(D,Cross(E1,Q))
            //   |Dot(D,N)|*t = -sign(Dot(D,N))*Dot(Q,N)
            float DdN = ray.Direction.Dot(normal);
            float sign;
            if (DdN > MathUtil.ZeroTolerance)
            {
                sign = 1;
            }
            else if (DdN < -MathUtil.ZeroTolerance)
            {
                sign = -1;
                DdN = -DdN;
            }
            else
            {
                // Ray and triangle are parallel, call it a "no intersection"
                // even if the ray does intersect.
                Result = IntersectionResult.NoIntersection;
                return false;
            }

            float DdQxE2 = sign * ray.Direction.Dot(diff.Cross(edge2));
            if (DdQxE2 >= 0)
            {
                float DdE1xQ = sign * ray.Direction.Dot(edge1.Cross(diff));
                if (DdE1xQ >= 0)
                {
                    if (DdQxE2 + DdE1xQ <= DdN)
                    {
                        // Line intersects triangle, check if ray does.
                        float QdN = -sign * diff.Dot(normal);
                        if (QdN >= 0)
                        {
                            // Ray intersects triangle.
                            float inv = (1) / DdN;
                            RayParameter = QdN * inv;
                            float mTriBary1 = DdQxE2 * inv;
                            float mTriBary2 = DdE1xQ * inv;
                            TriangleBaryCoords = new Vector3f(1 - mTriBary1 - mTriBary2, mTriBary1, mTriBary2);
                            Type = IntersectionType.Point;
                            Quantity = 1;
                            Result = IntersectionResult.Intersects;
                            return true;
                        }
                        // else: t < 0, no intersection
                    }
                    // else: b1+b2 > 1, no intersection
                }
                // else: b2 < 0, no intersection
            }
            // else: b1 < 0, no intersection

            Result = IntersectionResult.NoIntersection;
            return false;
        }

        /// <summary>
        /// minimal intersection test, computes ray-t
        /// </summary>
        public static bool Intersects(ref Ray3f ray, ref Vector3f V0, ref Vector3f V1, ref Vector3f V2)
        {
            // Compute the offset origin, edges, and normal.
            Vector3f diff = ray.Origin - V0;
            Vector3f edge1 = V1 - V0;
            Vector3f edge2 = V2 - V0;
            Vector3f normal = edge1.Cross(ref edge2);

            // Solve Q + t*D = b1*E1 + b2*E2 (Q = kDiff, D = ray direction,
            // E1 = kEdge1, E2 = kEdge2, N = Cross(E1,E2)) by
            //   |Dot(D,N)|*b1 = sign(Dot(D,N))*Dot(D,Cross(Q,E2))
            //   |Dot(D,N)|*b2 = sign(Dot(D,N))*Dot(D,Cross(E1,Q))
            //   |Dot(D,N)|*t = -sign(Dot(D,N))*Dot(Q,N)
            float DdN = ray.Direction.Dot(ref normal);
            float sign;
            if (DdN > MathUtil.ZeroTolerance)
            {
                sign = 1;
            }
            else if (DdN < -MathUtil.ZeroTolerance)
            {
                sign = -1;
                DdN = -DdN;
            }
            else
            {
                // Ray and triangle are parallel, call it a "no intersection"
                // even if the ray does intersect.
                return false;
            }

            Vector3f cross = diff.Cross(ref edge2);
            float DdQxE2 = sign * ray.Direction.Dot(ref cross);
            if (DdQxE2 >= 0)
            {
                cross = edge1.Cross(ref diff);
                float DdE1xQ = sign * ray.Direction.Dot(ref cross);
                if (DdE1xQ >= 0)
                {
                    if (DdQxE2 + DdE1xQ <= DdN)
                    {
                        // Line intersects triangle, check if ray does.
                        float QdN = -sign * diff.Dot(ref normal);
                        if (QdN >= 0)
                        {
                            // Ray intersects triangle.
                            return true;
                        }
                        // else: t < 0, no intersection
                    }
                    // else: b1+b2 > 1, no intersection
                }
                // else: b2 < 0, no intersection
            }
            // else: b1 < 0, no intersection

            return false;
        }


        /// <summary>
        /// minimal intersection test, computes ray-t
        /// </summary>
        public static bool Intersects(ref Ray3f ray, ref Vector3f V0, ref Vector3f V1, ref Vector3f V2, out float rayT)
        {
            // Compute the offset origin, edges, and normal.
            Vector3f diff = ray.Origin - V0;
            Vector3f edge1 = V1 - V0;
            Vector3f edge2 = V2 - V0;
            Vector3f normal = edge1.Cross(ref edge2);

            rayT = float.MaxValue;

            // Solve Q + t*D = b1*E1 + b2*E2 (Q = kDiff, D = ray direction,
            // E1 = kEdge1, E2 = kEdge2, N = Cross(E1,E2)) by
            //   |Dot(D,N)|*b1 = sign(Dot(D,N))*Dot(D,Cross(Q,E2))
            //   |Dot(D,N)|*b2 = sign(Dot(D,N))*Dot(D,Cross(E1,Q))
            //   |Dot(D,N)|*t = -sign(Dot(D,N))*Dot(Q,N)
            float DdN = ray.Direction.Dot(ref normal);
            float sign;
            if (DdN > MathUtil.ZeroTolerance)
            {
                sign = 1;
            }
            else if (DdN < -MathUtil.ZeroTolerance)
            {
                sign = -1;
                DdN = -DdN;
            }
            else
            {
                // Ray and triangle are parallel, call it a "no intersection"
                // even if the ray does intersect.
                return false;
            }

            Vector3f cross = diff.Cross(ref edge2);
            float DdQxE2 = sign * ray.Direction.Dot(ref cross);
            if (DdQxE2 >= 0)
            {
                cross = edge1.Cross(ref diff);
                float DdE1xQ = sign * ray.Direction.Dot(ref cross);
                if (DdE1xQ >= 0)
                {
                    if (DdQxE2 + DdE1xQ <= DdN)
                    {
                        // Line intersects triangle, check if ray does.
                        float QdN = -sign * diff.Dot(ref normal);
                        if (QdN >= 0)
                        {
                            // Ray intersects triangle.
                            float inv = (1) / DdN;
                            rayT = QdN * inv;
                            return true;
                        }
                        // else: t < 0, no intersection
                    }
                    // else: b1+b2 > 1, no intersection
                }
                // else: b2 < 0, no intersection
            }
            // else: b1 < 0, no intersection

            return false;
        }

    }
}

