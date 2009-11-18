// <copyright file="AbstractRational.cs" company="Taasss">Copyright (c) 2009 All Right Reserved</copyright>
// <author>Ben Vincent</author>
// <date>2009-11-17</date>
// <summary>AbstractRational</summary>
namespace FotoFly
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public abstract class AbstractRational
    {
        protected int numerator;
        protected int denominator;

        public AbstractRational()
        {
        }

        /// <summary>
        /// Creates an Exif Rational
        /// </summary>
        /// <param name="numerator">The value you want to store in the Rational</param>
        /// <param name="accuracy">The number of decimal places of accuracy</param>
        public AbstractRational(double numerator, int accuracy)
        {
            accuracy = (int)Math.Pow(10, accuracy);

            this.numerator = Convert.ToInt32(Math.Abs(numerator * accuracy));
            this.denominator = accuracy;
        }

        public int Numerator
        {
            get { return this.numerator; }
        }

        public int Denominator
        {
            get { return this.denominator; }
        }

        /// <summary>
        /// Returns the Rational as a Double
        /// </summary>
        /// <returns>Double, accurate to four decimal places</returns>
        public double ToDouble()
        {
            return this.ToDouble(4);
        }

        public double ToDouble(int decimalPlaces)
        {
            return Math.Round(Convert.ToDouble(this.numerator) / Convert.ToDouble(this.denominator), decimalPlaces);
        }

        /// <summary>
        /// Returns the Rational as an Integer
        /// </summary>
        /// <returns>Int</returns>
        public int ToInt()
        {
            return Convert.ToInt32(Math.Round(Convert.ToDouble(this.numerator) / Convert.ToDouble(this.denominator)));
        }

        /// <summary>
        /// Returns the Rational as a Ulong, typically used to write back to exif metadata
        /// </summary>
        /// <returns>Ulong</returns>
        public ulong ToUInt64()
        {
            return ((ulong)this.numerator) | (((ulong)this.denominator) << 32);
        }

        public string ToDoubleString()
        {
            return this.ToDouble().ToString();
        }

        public string ToFractionString()
        {
            return this.numerator.ToString() + " / " + this.denominator.ToString() + " (" + this.ToDouble() + ")";
        }

        /// <summary>
        /// Returns the Rational as a string
        /// </summary>
        /// <returns>A string in the format numerator/denominator</returns>
        public new string ToString()
        {
            return this.ToFractionString();
        }
    }
}