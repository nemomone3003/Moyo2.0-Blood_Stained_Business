
namespace Moyo2.Geometry
{
    public class ConvexPolygon
    {
        private readonly Plane[] edges;

        public ConvexPolygon(Vector3[] vertices)
        {
            if (vertices is null)
            {
                throw new ArgumentNullException(nameof(vertices));
            }

            int edgeCount = vertices.Length;
            edges = new Plane[edgeCount];

            // Assuming the vertices are given in clockwise order
            for (int i = 0; i < edgeCount; i++)
            {
                Vector3 p1 = vertices[i];
                Vector3 p2 = vertices[(i + 1) % edgeCount];

                // Calculate the edge's normal and distance to origin
                Vector3 edgeDirection = (p2 - p1).normalized;
                Vector3 edgeNormal = new(-edgeDirection.z, 0, edgeDirection.x); // Perpendicular to the edge
                float distance = Vector3.Dot(edgeNormal, p1);

                edges[i] = new Plane(edgeNormal, distance);
            }
        }

        public bool Contains(Vector2 point)
        {
            Vector3 point3D = new(point.x, 0, point.y); // Convert Vector2 to Vector3, assuming Y is 0

            // Check if the point is on the correct side of all edges
            foreach (Plane edge in edges)
            {
                if (!edge.GetSide(point3D))
                {
                    return false; // The point is outside of this edge
                }
            }
            return true; // The point is inside all edges
        }
    }
}
