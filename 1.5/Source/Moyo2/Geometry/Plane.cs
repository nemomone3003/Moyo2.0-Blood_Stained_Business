namespace Moyo2.Geometry
{
	public readonly struct Plane
	{
		private readonly Vector3 normal;
		private readonly float distance;

		public Plane(Vector3 normal, float distance)
		{
			this.normal = normal;
			this.distance = distance;
		}

		// Check which side of the plane the point is on
		public readonly bool GetSide(Vector3 point)
		{
			return Vector3.Dot(normal, point) <= distance;
		}
	}
}
