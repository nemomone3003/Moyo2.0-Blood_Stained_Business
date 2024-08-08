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
        }

        public override void DrawEffectPreview(LocalTargetInfo target)
        {
            GenDraw.DrawFieldEdges(RectangleCells(target));
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

            // Prior angle - 90 to get the rest of the angle.
            float angleDiff = 90 - angleOffset;
            Log.Message(angleDiff);

            // First 
            // Then we divide 

            /*
            float lengthHorizontal = (targetPos - Caster.Position).LengthHorizontal;
            float normalizedX = (float)(targetPosVector.x - casterPosVector.x) / lengthHorizontal;
            float normalizedY = (float)(targetPosVector.z - casterPosVector.z) / lengthHorizontal;

            int EndOfLineXCell = Mathf.RoundToInt(normalizedX * Props.height);
            int EndOfLineYCell = Mathf.RoundToInt(normalizedY * Props.height);
            IntVec3 maxRangeTile = new(EndOfLineXCell, 0, EndOfLineYCell);
            */

            IntVec3 bottomLeftTile = CalculatePoint(angleOffset, Props.width) + target.Cell;
            IntVec3 bottomRightTile = CalculatePoint(angleDiff, Props.width) + target.Cell;
            IntVec3 topLeftTile = CalculatePoint(angleOffset, Props.width) + targetPos;
            IntVec3 topRightTile = CalculatePoint(angleDiff, Props.width) + targetPos;

            GenDraw.DrawLineBetween(casterPosVector, targetPosVector, SimpleColor.Red);
            GenDraw.DrawLineBetween(bottomLeftTile.ToVector3().Yto0(), bottomRightTile.ToVector3().Yto0(), SimpleColor.Red);
            GenDraw.DrawLineBetween(bottomRightTile.ToVector3().Yto0(), topLeftTile.ToVector3().Yto0(), SimpleColor.Red);
            GenDraw.DrawLineBetween(topLeftTile.ToVector3().Yto0(), topRightTile.ToVector3().Yto0(), SimpleColor.Red);
            GenDraw.DrawLineBetween(topRightTile.ToVector3().Yto0(), bottomLeftTile.ToVector3().Yto0(), SimpleColor.Red);

            List<IntVec3> bresenhamCells = GenSight.BresenhamCellsBetween(bottomLeftTile, topRightTile);
            foreach (IntVec3 cell in bresenhamCells)
            {
                tmpCells.Add(cell);
            }
            tmpCells.Add(targetPos);

            return tmpCells;
        }

        private static float AngleToPoint(Vector3 pos, Vector3 point)
        {
            float xPrime = pos.x - point.x;
            float yPrime = pos.z - point.z;
            return (180 + Mathf.Atan2(xPrime, yPrime) * Mathf.Rad2Deg) % 360;
        }

        private static IntVec3 CalculatePoint(float angle, float width)
        {
            int XCord = (int)(width / 2 * Mathf.Cos(angle));
            int YCord = (int)(width / 2 * Mathf.Sin(angle));
            return new(XCord, 0, YCord);
        }
    }
}
