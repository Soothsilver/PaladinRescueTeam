using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Auxiliary
{
    class FpsupsCounter
    {
        public int FPSSoFar;
        public int UPSSoFar;
        public int FPS;
        public int UPS;
        private string fpsUpsString;
        public DateTime SecondElapsesIn = DateTime.Now;

        public void DrawSelf(Vector2 where)
        {
            Auxiliary.Primitives.DrawAndFillRectangle(new Rectangle((int)where.X, (int)where.Y, 300, 30), Color.LightBlue, Color.Black, 2);
            Auxiliary.Primitives.DrawSingleLineText(fpsUpsString, new Vector2((int)where.X + 7, (int)where.Y + 5), Color.Black);
        }
        public void DrawCycle()
        {
            FPSSoFar++;
        }
        public void UpdateCycle()
        {
            UPSSoFar++;
            if (DateTime.Now > SecondElapsesIn)
            {
                UPS = UPSSoFar;
                FPS = FPSSoFar;
                UPSSoFar = 0;
                FPSSoFar = 0;
                fpsUpsString = "FPS: "+ FPS +"; UPS: "+ UPS;
                SecondElapsesIn = DateTime.Now.AddSeconds(1);
            }
        }
    }
}
