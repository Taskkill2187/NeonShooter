using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace NeonShooter
{
    public class DynamicGrid
    {
        List<GridPoint[]> PointStrings = new List<GridPoint[]>();
        List<GridSpring> Springs = new List<GridSpring>();

        Rectangle Field;
        int PointSpacing;

        public DynamicGrid(Rectangle Field, int PointSpacing)
        {
            PointStrings.Clear();
            // Create the arrays and fill them
            for (int i = 0; i < Field.Height / PointSpacing + 2; i++)
            {
                GridPoint[] A = new GridPoint[Field.Width / PointSpacing + 2];

                for (int iA = 0; iA < A.GetLength(0); iA++)
                {
                    if (iA == 0 || iA == A.GetLength(0) - 1 || i == 0 || i == Field.Height / PointSpacing + 1)
                    {
                        A[iA] = new GridPoint(new Vector2(iA * PointSpacing + Field.X, i * PointSpacing + Field.Y), false);
                    }
                    else
                    {
                        A[iA] = new GridPoint(new Vector2(iA * PointSpacing + Field.X, i * PointSpacing + Field.Y), true);
                    }
                }

                PointStrings.Add(A);
            }

            // Create the horz Springs
            for (int iy = 0; iy < PointStrings.Count; iy++)
            {
                for (int ix = 0; ix < PointStrings[iy].GetLength(0) - 1; ix++)
                {
                    Springs.Add(new GridSpring(PointStrings[iy][ix], PointStrings[iy][ix + 1]));
                }
            }

            // Create the vert Springs
            for (int iy = 0; iy < PointStrings.Count - 1; iy++)
            {
                for (int ix = 0; ix < PointStrings[iy].GetLength(0); ix++)
                {
                    Springs.Add(new GridSpring(PointStrings[iy][ix], PointStrings[iy + 1][ix]));
                }
            }

            this.Field = Field;
            this.PointSpacing = PointSpacing;
        }

        public void ApplyForce(Vector2 Pos, float Strength)
        {
            lock (PointStrings)
            {
                for (int iy = 0; iy < PointStrings.Count; iy++)
                {
                    for (int ix = 0; ix < PointStrings[iy].GetLength(0); ix++)
                    {
                        if ((PointStrings[iy][ix].Pos - Pos).LengthSquared() < 150 * 150)
                        {
                            PointStrings[iy][ix].GetPulledBy(Pos, Strength * 25);
                        }
                    }
                }
            }
        }

        public void Update(object stateInfo)
        {
            lock (PointStrings)
            {
                for (int iy = 0; iy < PointStrings.Count; iy++)
                {
                    for (int ix = 0; ix < PointStrings[iy].GetLength(0); ix++)
                    {
                        PointStrings[iy][ix].Update();
                        //PointStrings[iy][ix].Pos.Y += 1;
                    }
                }

                if (PointStrings[0][0].Pos.Y == PointSpacing)
                {
                    // Create new Point row
                    GridPoint[] A = new GridPoint[Field.Width / PointSpacing + 2];
                    for (int iA = 0; iA < A.GetLength(0); iA++)
                    {
                        A[iA] = new GridPoint(new Vector2(iA * PointSpacing + Field.X, PointStrings[0][0].Pos.Y - PointSpacing), false);
                    }
                    PointStrings.Insert(0, A);

                    // Make old Point row moveable again
                    for (int i = 1; i < PointStrings[1].GetLength(0) - 1; i++)
                    {
                        PointStrings[1][i].Moveable = true;
                    }

                    // Create the horz Springs
                    for (int ix = 0; ix < PointStrings[0].GetLength(0) - 1; ix++)
                    {
                        Springs.Add(new GridSpring(PointStrings[0][ix], PointStrings[0][ix + 1]));
                    }

                    // Create the vert Springs
                    for (int ix = 0; ix < PointStrings[0].GetLength(0); ix++)
                    {
                        Springs.Add(new GridSpring(PointStrings[0][ix], PointStrings[1][ix]));
                    }

                    // Delete bottom row
                    PointStrings.RemoveAt(PointStrings.Count - 1);

                    // Make the new bottom row not moveable again
                    for (int i = 1; i < PointStrings[1].GetLength(0) - 1; i++)
                    {
                        PointStrings[PointStrings.Count - 1][i].Moveable = false;
                    }

                    // Recreate all Springs
                    lock (Springs)
                    {
                        Springs.Clear();
                        for (int iy = 0; iy < PointStrings.Count; iy++)
                        {
                            for (int ix = 0; ix < PointStrings[iy].GetLength(0) - 1; ix++)
                            {
                                Springs.Add(new GridSpring(PointStrings[iy][ix], PointStrings[iy][ix + 1]));
                            }
                        }

                        for (int iy = 0; iy < PointStrings.Count - 1; iy++)
                        {
                            for (int ix = 0; ix < PointStrings[iy].GetLength(0); ix++)
                            {
                                Springs.Add(new GridSpring(PointStrings[iy][ix], PointStrings[iy + 1][ix]));
                            }
                        }
                    }
                }
            }

            lock (Springs)
            {
                for (int i = 0; i < Springs.Count; i++)
                {
                    Springs[i].Update();
                }
            }
        }
        public void Draw(SpriteBatch SB)
        {
            lock (Springs) { for (int i = 0; i < Springs.Count; i++) { Springs[i].Draw(SB); } }
        }
    }
}
