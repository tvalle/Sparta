//using FarseerPhysics.Dynamics;
//using FarseerPhysics.Factories;
//using Microsoft.Xna.Framework;

//namespace Sparta.Physics
//{
//    public static class SpartaPhysics
//    {
//        public static float MeterPerPixel;

//        public static World world;

//        public static Vector2 ConvertUnits(Vector2 unit)
//        {
//            return unit * MeterPerPixel;
//        }

//        public static Body AddBox2DRectangle(int width, int height, float density)
//        {
//            return AddBox2DRectangle(width, height, density, Vector2.Zero);
//        }

//        public static Body AddBox2DRectangle(int width, int height, float density, Vector2 position)
//        {
//            Body body = BodyFactory.CreateRectangle(world, width / MeterPerPixel, height / MeterPerPixel, density);
//            body.BodyType = BodyType.Dynamic;
//            body.Position = position / MeterPerPixel;
//            return body;
//        }

//        public static Body AddBox2DRoundedRectangle(float width, float height, float xRadius, float yRadius, int segments, float density)
//        {
//            Body body = BodyFactory.CreateRoundedRectangle(world, width / MeterPerPixel, height / MeterPerPixel, xRadius / MeterPerPixel, yRadius / MeterPerPixel, segments, density);
//            body.BodyType = BodyType.Dynamic;
//            return body;
//        }
//    }
//}
