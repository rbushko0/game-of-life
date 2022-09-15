// Copyright 2017 Igor' Leonidov
// Licensed under the Apache License, Version 2.0

namespace UltimateQuadTree
{
    /// <summary>Stores a set of four values of a Double that represent the location and size of a rectangle</summary>
    public struct QuadTreeRect
    {
        /// <summary>Gets the x-coordinate of the upper-left corner of this <see cref="T:UltimateQuadTree.QuadTreeRect"></see> structure.</summary>
        /// <returns>The x-coordinate of the upper-left corner of this <see cref="T:UltimateQuadTree.QuadTreeRect"></see> structure.</returns>
        public readonly ulong X;
        /// <summary>Gets the y-coordinate of the upper-left corner of this <see cref="T:UltimateQuadTree.QuadTreeRect"></see> structure.</summary>
        /// <returns>The y-coordinate of the upper-left corner of this <see cref="T:UltimateQuadTree.QuadTreeRect"></see> structure.</returns>
        public readonly ulong Y;
        /// <summary>Gets the width of this <see cref="T:UltimateQuadTree.QuadTreeRect"></see> structure.</summary>
        /// <returns>The width of this <see cref="T:UltimateQuadTree.QuadTreeRect"></see> structure.</returns>
        public readonly ulong Width;
        /// <summary>Gets the height of this <see cref="T:UltimateQuadTree.QuadTreeRect"></see> structure.</summary>
        /// <returns>The height of this <see cref="T:UltimateQuadTree.QuadTreeRect"></see> structure.</returns>
        public readonly ulong Height;
        
        /// <summary>Gets the y-coordinate of the top edge of this <see cref="T:UltimateQuadTree.QuadTreeRect"></see> structure.</summary>
        /// <returns>The y-coordinate of the top edge of this <see cref="T:UltimateQuadTree.QuadTreeRect"></see> structure.</returns>
        public ulong Top => Y;
        /// <summary>Gets the y-coordinate that is the sum of the <see cref="P:UltimateQuadTree.QuadTreeRect.Y"></see> and <see cref="P:UltimateQuadTree.QuadTreeRect.Height"></see> property values of this <see cref="T:UltimateQuadTree.QuadTreeRect"></see> structure.</summary>
        /// <returns>The y-coordinate that is the sum of <see cref="P:UltimateQuadTree.QuadTreeRect.Y"></see> and <see cref="P:UltimateQuadTree.QuadTreeRect.Height"></see> of this <see cref="T:UltimateQuadTree.QuadTreeRect"></see>.</returns>
        public ulong Bottom => Y + Height;
        /// <summary>Gets the x-coordinate of the left edge of this <see cref="T:UltimateQuadTree.QuadTreeRect"></see> structure.</summary>
        /// <returns>The x-coordinate of the left edge of this <see cref="T:UltimateQuadTree.QuadTreeRect"></see> structure.</returns>
        public ulong Left => X;
        /// <summary>Gets the x-coordinate that is the sum of <see cref="P:UltimateQuadTree.QuadTreeRect.X"></see> and <see cref="P:UltimateQuadTree.QuadTreeRect.Width"></see> property values of this <see cref="T:UltimateQuadTree.QuadTreeRect"></see> structure.</summary>
        /// <returns>The x-coordinate that is the sum of <see cref="P:UltimateQuadTree.QuadTreeRect.X"></see> and <see cref="P:UltimateQuadTree.QuadTreeRect.Width"></see> of this <see cref="T:UltimateQuadTree.QuadTreeRect"></see>.</returns>
        public ulong Right => X + Width;
        
        internal ulong MidX => X + HalfWidth;
        internal ulong MidY => Y + HalfHeight;

        internal QuadTreeRect LeftTopQuarter => new QuadTreeRect(X, Y, HalfWidth, HalfHeight);
        internal QuadTreeRect LeftBottomQuarter => new QuadTreeRect(X, MidY, HalfWidth, HalfHeight);
        internal QuadTreeRect RightTopQuarter => new QuadTreeRect(MidX, Y, HalfWidth, HalfHeight);
        internal QuadTreeRect RightBottomQuarter => new QuadTreeRect(MidX, MidY, HalfWidth, HalfHeight);

        private ulong HalfWidth => Width / 2;
        private ulong HalfHeight => Height / 2;

        /// <summary>Initializes a new instance of the <see cref="T:UltimateQuadTree.QuadTreeRect"></see> class with the specified location and size.</summary>
        /// <param name="x">The x-coordinate of the upper-left corner of the rectangle.</param>
        /// <param name="y">The y-coordinate of the upper-left corner of the rectangle.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        public QuadTreeRect(ulong x, ulong y, ulong width, ulong height)
        {
            X = x;
            Y = y;

            Width = width;
            Height = height;
        }
    }
}