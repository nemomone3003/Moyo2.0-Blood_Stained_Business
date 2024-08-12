using Moyo2.Geometry;

namespace Moyo2
{
    public class CompAbility_LaminarGlaive : CompAbilityEffect
    {
        Pawn Caster => parent.pawn;

        public new CompAbilityProperties_LaminarGlaive Props => (CompAbilityProperties_LaminarGlaive)props;


        private readonly List<IntVec3> tmpCells = new();


        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            foreach (IntVec3 cell in tmpCells)
            {
                Thing instancedThing = ThingMaker.MakeThing(ThingDefOf.DiningChair, ThingDefOf.WoodLog);
                GenSpawn.Spawn(instancedThing, cell, Caster.Map);
            }
        }

        public override void DrawEffectPreview(LocalTargetInfo target)
        {
            GenDraw.DrawFieldEdges(RectangleCells(target), Color.blue);
        }

        private List<IntVec3> RectangleCells(LocalTargetInfo target)
        {
            tmpCells.Clear(); // So we don't have previous cells when calculation

            // We get the caster's position and the target's position, as Vector3 so we don't get weird results on angles
            // We flatten the Y axis to 0, otherwise the Vector's length get changed (Remember X is horizontal and Z is vertical in rimworld)
            Vector3 casterPosVector = Caster.Position.ToVector3Shifted().Yto0();
            IntVec3 targetPos = target.Cell.ClampInsideMap(Caster.Map);
            Vector3 targetPosVector = targetPos.ToVector3Shifted().Yto0();


            // The angle you get if you draw a line to the north,
            // another line from the caster's position to the target's position,
            // and check the angle between them
            float angleOffset = AngleToPoint(casterPosVector, targetPosVector);
            Log.Message(angleOffset);

            // 90 - Prior angle to get the rest of the angle.
            float angleDiff = 90 - angleOffset;
            Log.Message(angleDiff);

            // All the calculated tiles, starting from the target's position
            // The furthest cell of the middle row of the rectangle
            IntVec3 furthestCell = MiddleRowFurthestCell(angleOffset, Props.height, targetPos);

            // The topmost and bottommost tiles of the left side of the rectangle, 2 out of the 4 corners
            IntVec3 bottomLeftTile = CalculateLeftSide(angleOffset, Props.width) + targetPos;
            IntVec3 bottomRightTile = CalculateRightSide(angleDiff, Props.width) + targetPos;

            // Same thing but for the right side, the other 2 corners i need.
            IntVec3 topRightTile = CalculateRightSide(angleDiff, Props.width) + furthestCell;
            IntVec3 topLeftTile = CalculateLeftSide(angleOffset, Props.width) + furthestCell;

            Vector3 topRightVec = topRightTile.ToVector3().Yto0();
            Vector3 bottomRightVec = bottomRightTile.ToVector3().Yto0();
            Vector3 bottomLeftVec = bottomLeftTile.ToVector3().Yto0();
            Vector3 topLeftVec = topLeftTile.ToVector3().Yto0();

            Vector3[] vectorsRectangle = [topRightVec, bottomRightVec, bottomLeftVec, topLeftVec];
            ConvexPolygon rectangle = new(vectorsRectangle);

            int xMin = Mathf.Min(bottomLeftTile.x, topRightTile.x, bottomRightTile.x, topLeftTile.x);
            int xMax = Mathf.Max(bottomLeftTile.x, topRightTile.x, bottomRightTile.x, topLeftTile.x);
            int yMin = Mathf.Min(bottomLeftTile.z, topRightTile.z, bottomRightTile.z, topLeftTile.z);
            int yMax = Mathf.Max(bottomLeftTile.z, topRightTile.z, bottomRightTile.z, topLeftTile.z);
            // iterate over both the x and z-axes to look at each possible cell within the bounds of your rectangle
            // remember, Area = l x w, so we need to do that here to check the entire area of our rectangle
            for (int x = xMin; x <= xMax; x++)
            {
                for (int z = yMin; z <= yMax; z++)
                {
                    IntVec3 checkingCell = new(x, 0, z);
                    if (checkingCell.InBounds(Caster.Map) && rectangle.Contains(checkingCell.ToVector2()))
                    {
                        tmpCells.Add(checkingCell);
                    }
                }
            }

            return tmpCells;
        }

        private static float AngleToPoint(Vector3 pos, Vector3 point)
        {
            float xPrime = pos.x - point.x;
            float yPrime = pos.z - point.z;
            return (180 + Mathf.Atan2(xPrime, yPrime) * Mathf.Rad2Deg) % 360;
        }

        private static IntVec3 CalculateLeftSide(float angleDeg, float width)
        {
            float actualHypothenuse = width / 2; // If the rectangle is 3 of width, every side is roughly 1.5 long, 1 tile each
            int XCord = (int)(actualHypothenuse * Mathf.Cos(angleDeg * Mathf.Deg2Rad));
            int YCord = (int)(actualHypothenuse * Mathf.Sin(angleDeg * Mathf.Deg2Rad));

            return new(-XCord, 0, YCord);
        }


        private static IntVec3 CalculateRightSide(float angleDeg, float width)
        {
            float actualHypothenuse = width / 2; // If the rectangle is 3 of width, every side is roughly 1.5 long, 1 tile each
            int XCord = (int)(actualHypothenuse * Mathf.Sin(angleDeg * Mathf.Deg2Rad));
            int YCord = (int)(actualHypothenuse * Mathf.Cos(angleDeg * Mathf.Deg2Rad));

            return new(XCord, 0, -YCord);
        }

        private static IntVec3 MiddleRowFurthestCell(float angleDeg, float height, IntVec3 targetCell)
        {
            int XCord = (int)(height * Mathf.Sin(angleDeg * Mathf.Deg2Rad));
            int YCord = (int)(height * Mathf.Cos(angleDeg * Mathf.Deg2Rad));
            IntVec3 calculatedCell = new(XCord, 0, YCord);
            return targetCell + calculatedCell;
        }
    }
}